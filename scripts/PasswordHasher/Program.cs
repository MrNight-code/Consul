using System.Data;
using Dapper;
using MySqlConnector;
using BCrypt.Net;

namespace Scripts.PasswordHasher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Allow passing DB connection info via args, or default
            var host = "localhost";
            var port = "3310";
            var user = "admin";
            var pass = "root";

            if (args.Length > 0)
            {
                // Minimal arg parsing for integration
                // usage: PasswordHasher.exe [host] [port] [user] [pass]
                if (args.Length >= 1) host = args[0];
                if (args.Length >= 2) port = args[1];
                if (args.Length >= 3) user = args[2];
                if (args.Length >= 4) pass = args[3];
            }

            var connectionString = $"Server={host};Port={port};User={user};Password={pass};TreatTinyAsBoolean=true";

            Console.WriteLine($"Connecting to {host}:{port}...");

            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("Connected!");

                var databases = await connection.QueryAsync<string>("SHOW DATABASES");
                
                foreach(var db in databases)
                {
                    // Target only tenant databases, e.g. db_condominio_* or specific ones
                    // Adjust filter as needed, or maybe pass target DB as arg
                    if (db.StartsWith("db_condominio_") || db.Contains("bosques") || db.Contains("colina"))
                    {
                         Console.WriteLine($"--- Processing Database: {db} ---");
                         try 
                         {
                             var dbConnStr = $"Server={host};Port={port};Database={db};User={user};Password={pass};TreatTinyAsBoolean=true";
                             using var dbConn = new MySqlConnection(dbConnStr);
                             await dbConn.OpenAsync();
                             
                             // Check if table 'usuario' exists
                             var tables = await dbConn.QueryAsync<string>("SHOW TABLES");
                             if (!tables.Any(t => t.Equals("usuario", StringComparison.OrdinalIgnoreCase)))
                             {
                                 Console.WriteLine($"Table 'usuario' not found in {db}. Skipping.");
                                 continue;
                             }

                             // Select users where PasswordHash is empty OR looks like a plain text (not starting with $2)
                             // Or we can just iterate all and check.
                             // Assuming columns: pk_usuario/Id, username, password_hash
                             
                             // We need to support different schema versions if any.
                             // Let's inspect columns first to be safe.
                             var columns = await dbConn.QueryAsync<string>("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'usuario'", new { db });
                             var colList = columns.ToList();
                             
                             bool hasPassHash = colList.Contains("password_hash", StringComparer.OrdinalIgnoreCase) || 
                                                colList.Contains("PasswordHash", StringComparer.OrdinalIgnoreCase);
                                                
                             if (!hasPassHash)
                             {
                                  Console.WriteLine("Column 'PasswordHash' not found. Skipping.");
                                  continue; 
                             }
                             
                             // Normalize column name
                             string? hashCol = colList.FirstOrDefault(c => c.Equals("password_hash", StringComparison.OrdinalIgnoreCase) || c.Equals("PasswordHash", StringComparison.OrdinalIgnoreCase));
                             string? idCol = colList.FirstOrDefault(c => c.Equals("pk_usuario", StringComparison.OrdinalIgnoreCase) || c.Equals("Id", StringComparison.OrdinalIgnoreCase) || c.Equals("IdUsuario", StringComparison.OrdinalIgnoreCase));
                             
                             if (hashCol == null) // Should already be covered by check above but explicit is better
                             {
                                  Console.WriteLine("Column 'PasswordHash' not found (unexpected). Skipping.");
                                  continue; 
                             }
                             idCol = idCol ?? "Id";

                             var query = $"SELECT {idCol} as Id, {hashCol} as Hash FROM usuario";
                             var users = await dbConn.QueryAsync(query);
                             
                             int updatedCount = 0;
                             
                             foreach(var u in users)
                             {
                                 string currentHash = u.Hash?.ToString();
                                 var userId = u.Id;
                                 
                                 if (string.IsNullOrEmpty(currentHash)) continue;
                                 
                                 // Check if it's already a valid BCrypt hash
                                 // BCrypt hashes start with $2a$, $2b$, $2y$ and are 60 chars long.
                                 bool isHashed = currentHash.StartsWith("$2") && currentHash.Length == 60;
                                 
                                 if (!isHashed)
                                 {
                                     // It is plain text
                                     string newHash = BCrypt.Net.BCrypt.HashPassword(currentHash);
                                     
                                     await dbConn.ExecuteAsync($"UPDATE usuario SET {hashCol} = @newHash WHERE {idCol} = @userId", new { newHash, userId });
                                     Console.WriteLine($"   User ID {userId}: Hashed successfully.");
                                     updatedCount++;
                                 }
                             }
                             
                             if (updatedCount > 0)
                                Console.WriteLine($"   Updated {updatedCount} users in {db}.");
                             else
                                Console.WriteLine($"   No users needed update in {db}.");
                         }
                         catch(Exception inner) 
                         {
                             Console.WriteLine($"   Error processing {db}: {inner.Message}");
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
    }
}
