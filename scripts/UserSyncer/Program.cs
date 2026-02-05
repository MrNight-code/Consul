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
            var host = "127.0.0.1";
            var port = "3310"; // Default to external port for local dev
            var user = "root"; // Needs root to access multiple DBs
            var pass = "root";

            if (args.Length >= 1) host = args[0];
            if (args.Length >= 2) port = args[1];
            if (args.Length >= 3) user = args[2];
            if (args.Length >= 4) pass = args[3];

            var connectionStringBase = $"Server={host};Port={port};User={user};Password={pass};TreatTinyAsBoolean=true";
            Console.WriteLine($"Connecting to {host}:{port}...");

            try
            {
                using var conn = new MySqlConnection(connectionStringBase);
                await conn.OpenAsync();
                Console.WriteLine("Connected to MySQL Host.");

                // 1. Identify Master DB
                var databases = await conn.QueryAsync<string>("SHOW DATABASES");
                string masterDbName = "db_consulcon_master"; // Hardcoded for now, or could search
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
                            await SyncTenantUsers(host, port, user, pass, db, masterDbName);
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

        static async Task SyncTenantUsers(string host, string port, string user, string pass, string tenantDb, string masterDb)
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
                string defaultConnStr = $"Server=db;Database={tenantDb};User=root;Password=root";
                
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
                string username = GetValue(d, "username", "usuario");
                string passwordHash = GetValue(d, "password_hash", "PasswordHash") ?? GetValue(d, "contrasena", "password");
                
                if (string.IsNullOrEmpty(username)) continue;
                
                // 3. Sync to UsuariosMaster
                var masterUser = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    $"SELECT * FROM {masterDb}.UsuariosMaster WHERE Username = @Username", new { Username = username });
                    
                int idUsuarioMaster;
                
                if (masterUser == null)
                {
                    Console.WriteLine($"   Creating User '{username}' in Master...");
                    // Schema: Id, Username, PasswordHash, Email, FechaCreacion, EsSuperAdmin
                    var insertUserSql = $@"
                        INSERT INTO {masterDb}.UsuariosMaster (Username, PasswordHash, EsSuperAdmin, FechaCreacion)
                        VALUES (@Username, @PasswordHash, 0, NOW());
                        SELECT LAST_INSERT_ID();";
                        
                    idUsuarioMaster = await conn.ExecuteScalarAsync<int>(insertUserSql, new { Username = username, PasswordHash = passwordHash });
                }
                else
                {
                    idUsuarioMaster = masterUser.Id; // Column is Id
                }
                
                // 4. Link User to Condominio (UsuarioCondominio)
                // Schema: Id, UsuarioId, CondominioId, RolInicial
                var link = await conn.QueryFirstOrDefaultAsync(
                    $"SELECT * FROM {masterDb}.UsuarioCondominio WHERE UsuarioId = @u AND CondominioId = @c",
                    new { u = idUsuarioMaster, c = idCondominio });
                    
                if (link == null)
                {
                     Console.WriteLine($"   Linking User '{username}' to '{identifier}'.");
                     await conn.ExecuteAsync(
                         $"INSERT INTO {masterDb}.UsuarioCondominio (UsuarioId, CondominioId, RolInicial) VALUES (@u, @c, 'Usuario')",
                         new { u = idUsuarioMaster, c = idCondominio });
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
