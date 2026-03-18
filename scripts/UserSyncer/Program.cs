using System.Data;
using Dapper;
using MySqlConnector;

namespace Scripts.UserSyncer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Usage: UserSyncer.exe [host] [port] [user] [pass]
            string? host = null;
            string? port = null;
            string? user = null;
            string? pass = null;
            string? masterDbName = null;
            string? internalHost = null;

            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                while (currentDir != null)
                {
                    var envPath = Path.Combine(currentDir, ".env");
                    if (File.Exists(envPath))
                    {
                        Console.WriteLine($"Loading defaults from .env...");
                        foreach (var line in File.ReadAllLines(envPath))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                            var parts = line.Split('=', 2);
                            if (parts.Length == 2)
                            {
                                var key = parts[0].Trim();
                                var val = parts[1].Trim();
                                if (key == "DB_HOST") 
                                {
                                    host ??= val;
                                    internalHost ??= val;
                                }
                                if (key == "DB_PORT") port ??= val;
                                if (key == "DB_USER") user ??= val;
                                if (key == "DB_PASSWORD") pass ??= val;
                                if (key == "DB_NAME") masterDbName ??= val;
                            }
                        }
                        break;
                    }
                    currentDir = Directory.GetParent(currentDir)?.FullName;
                }
            }
            catch { /* Ignore .env parsing errors */ }

            if (args.Length >= 1) host = args[0];
            if (args.Length >= 2) port = args[1];
            if (args.Length >= 3) user = args[2];
            if (args.Length >= 4) pass = args[3];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || 
                string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || 
                string.IsNullOrEmpty(masterDbName) || string.IsNullOrEmpty(internalHost))
            {
                Console.WriteLine("Error: Missing database configuration. Ensure .env file exists or parameters are provided.");
                Console.WriteLine("Usage: UserSyncer.exe [host] [port] [user] [pass]");
                Environment.Exit(1);
            }

            var connectionStringBase = $"Server={host};Port={port};User={user};Password={pass};TreatTinyAsBoolean=true";
            Console.WriteLine($"Connecting to {host}:{port}...");

            try
            {
                using var conn = new MySqlConnection(connectionStringBase);
                await conn.OpenAsync();
                Console.WriteLine("Connected to MySQL Host.");

                // 1. Identify Master DB
                var databases = await conn.QueryAsync<string>("SHOW DATABASES");
                if (!databases.Contains(masterDbName))
                {
                    Console.WriteLine($"Master Database '{masterDbName}' not found. Aborting.");
                    return;
                }

                // 2. Iterate Tenant Databases
                foreach (var db in databases)
                {
                    if (db.StartsWith("db_condominio_") && db != "db_condominio_template")
                    {
                        Console.WriteLine($"--- Processing Tenant DB: {db} ---");
                        try 
                        {
                            await SyncTenantUsers(host, port, user, pass, db, masterDbName, internalHost);
                        }
                        catch (Exception inner)
                        {
                            Console.WriteLine($"Error processing {db}: {inner.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static async Task SyncTenantUsers(string host, string port, string user, string pass, string tenantDb, string masterDb, string internalHost)
        {
            var connStr = $"Server={host};Port={port};User={user};Password={pass};TreatTinyAsBoolean=true"; // Connect to server, switch DBs in query
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            // 1. Ensure Condominio exists in Master
            // Identifier convention: db_condominio_NAME -> TenantId = NAME
            string identifier = tenantDb.Replace("db_condominio_", "");
            string nombre = identifier.Replace("_", " "); // Simple formatting
            
            // Check if Condominio exists (Schema: Id, TenantId, Nombre...)
            var condominio = await conn.QueryFirstOrDefaultAsync<dynamic>(
                $"SELECT * FROM {masterDb}.CondominiosMaster WHERE TenantId = @identifier", 
                new { identifier });

            int idCondominio;
            if (condominio == null)
            {
                Console.WriteLine($"   Creating Condominio '{identifier}' in Master...");
                // Insert
                string defaultConnStr = $"Server={internalHost};Database={tenantDb};User={user};Password={pass}";
                
                var insertSql = $@"
                    INSERT INTO {masterDb}.CondominiosMaster (Nombre, TenantId, ConnectionString, FechaRegistro) 
                    VALUES (@Nombre, @Identificador, @ConnectionString, NOW());
                    SELECT LAST_INSERT_ID();";
                    
                idCondominio = await conn.ExecuteScalarAsync<int>(insertSql, new { Nombre = nombre, Identificador = identifier, ConnectionString = defaultConnStr });
            }
            else
            {
                idCondominio = condominio.Id; // Column is Id
                Console.WriteLine($"   Condominio '{identifier}' exists (ID: {idCondominio}).");
            }

            // 2. Read Users from Tenant DB
            var users = await conn.QueryAsync<dynamic>($"SELECT * FROM {tenantDb}.usuario");
            
            foreach(var u in users)
            {
                var d = (IDictionary<string, object>)u;
                string? username = GetValue(d, "username", "usuario");
                string? passwordHash = GetValue(d, "password_hash", "PasswordHash") ?? GetValue(d, "contrasena", "password");
                string? idRolPrincipalStr = GetValue(d, "idrolprincipal", "IdRolPrincipal");
                bool isSuperAdmin = idRolPrincipalStr == "1";
                
                if (string.IsNullOrEmpty(username)) continue;
                
                // 3. Sync to UsuariosMaster
                var masterUser = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    $"SELECT * FROM {masterDb}.UsuariosMaster WHERE Username = @Username", new { Username = username });
                    
                int idUsuarioMaster;
                
                int parsedIdRol = 3; // Operador Default
                if (int.TryParse(idRolPrincipalStr, out int tempIdRol)) 
                {
                    parsedIdRol = tempIdRol;
                }

                if (masterUser == null)
                {
                    Console.WriteLine($"   Creating User '{username}' in Master...");
                    // Schema: Id, Username, PasswordHash, Email, FechaCreacion, EsSuperAdmin, IdRolPrincipal
                    var insertUserSql = $@"
                        INSERT INTO {masterDb}.UsuariosMaster (Username, PasswordHash, EsSuperAdmin, IdRolPrincipal, FechaCreacion)
                        VALUES (@Username, @PasswordHash, @EsSuperAdmin, @IdRolPrincipal, NOW());
                        SELECT LAST_INSERT_ID();";
                        
                    idUsuarioMaster = await conn.ExecuteScalarAsync<int>(insertUserSql, new { Username = username, PasswordHash = passwordHash, EsSuperAdmin = isSuperAdmin, IdRolPrincipal = parsedIdRol });
                }
                else
                {
                    idUsuarioMaster = masterUser.Id; // Column is Id
                }
                
                // 4. Link User to Condominio (UsuarioCondominio)
                // Schema: Id, UsuarioId, CondominioId, IdRol
                var link = await conn.QueryFirstOrDefaultAsync(
                    $"SELECT * FROM {masterDb}.UsuarioCondominio WHERE UsuarioId = @u AND CondominioId = @c",
                    new { u = idUsuarioMaster, c = idCondominio });
                    
                if (link == null)
                {
                     Console.WriteLine($"   Linking User '{username}' to '{identifier}' with Role {parsedIdRol}.");
                     await conn.ExecuteAsync(
                         $"INSERT INTO {masterDb}.UsuarioCondominio (UsuarioId, CondominioId, IdRol) VALUES (@u, @c, @r)",
                         new { u = idUsuarioMaster, c = idCondominio, r = parsedIdRol });
                }
            }
        }

        static string? GetValue(IDictionary<string, object> row, params string[] keys)
        {
            foreach(var k in keys)
            {
                if (row.ContainsKey(k)) return row[k]?.ToString();
                // Case insensitive? Dapper dynamic might be case sensitive keys depending on DB? 
                // Iterate keys to match
                var match = row.Keys.FirstOrDefault(rk => rk.Equals(k, StringComparison.OrdinalIgnoreCase));
                if (match != null) return row[match]?.ToString();
            }
            return null;
        }
    }
}
