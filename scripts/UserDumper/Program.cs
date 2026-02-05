using System.Data;
using Dapper;
using MySqlConnector;
using System.Text.RegularExpressions;

namespace Scripts.UserDumper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sqlFile = @"e:\Pasantias\SBTC\Sistema Gabriel\Backend\Backend-Consulcon\scripts\database\data\Bosques\syscons1_bdbosquescolina.sql";

            Console.WriteLine($"Reading {sqlFile}...");

            if (!File.Exists(sqlFile))
            {
                Console.WriteLine("File not found!");
                return;
            }

            using var reader = new StreamReader(sqlFile);
            string line;
            int count = 0;
            bool insideUsuarioInsert = false;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Contains("INSERT INTO `usuario`", StringComparison.OrdinalIgnoreCase) || 
                    line.Contains("INSERT INTO usuario", StringComparison.OrdinalIgnoreCase))
                {
                    insideUsuarioInsert = true;
                    Console.WriteLine("--- Found INSERT block start ---");
                }

                if (insideUsuarioInsert)
                {
                    // Simple regex to find (...) groups. 
                    // Warning: This ignores nested parens or string escaping, but might be enough.
                    // We look for patterns like ('username', 'password') or similar numbers/strings.
                    
                    var matches = Regex.Matches(line, @"\(([^)]+)\)");
                    foreach (Match match in matches)
                    {
                        try
                        {
                            var val = match.Groups[1].Value;
                            // Simple CSV split, respecting single quotes is hard without real parser.
                            // But let's assume standard format: 1, 1, '2023-01-01', 'admin', 'pass', ...
                            // Splitting by ',' might break if date or string has comma.
                            // But typically username/pass are simple.
                            
                            var parts = val.Split(',');
                            
                            // We expect at least 5 columns.
                            // Col 3: usuario (username)
                            // Col 4: contrasena (password)
                            // They might be quoted, e.g. 'admin'.
                            
                            if (parts.Length >= 5)
                            {
                                var user = parts[3].Trim().Trim('\'');
                                var pass = parts[4].Trim().Trim('\'');
                                
                                Console.WriteLine($"User: {user}, Pass: {pass}");
                                count++;
                            }
                        }
                        catch {}
                    }

                    if (line.Trim().EndsWith(";"))
                    {
                        insideUsuarioInsert = false;
                        Console.WriteLine("--- End of INSERT block ---");
                    }
                }
            }

            Console.WriteLine($"Done. Initial extraction count: {count}");
        }
    }
}
