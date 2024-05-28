using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DatabaseConnections;

namespace Schema
{
    public class MariaDBSchemaRetriever
    {
        private readonly MariaDBContext _context;

        public MariaDBSchemaRetriever()
        {
            var mariadbString = "Server=localhost;User Id=root;Password=123;Database=information_schema";
            var options = new DbContextOptionsBuilder<MariaDBContext>()
                .UseMySql(mariadbString, new MySqlServerVersion(new Version(8, 0, 21)))
                .Options;
            _context = new MariaDBContext(options);
        }

        public async Task RetrieveSchemaAsync()
        {
            var schema = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            var tables = _context.Tables.ToList();

            foreach (var table in tables)
            {
                var tableName = table.TABLE_NAME;
                if (tableName != null)
                {
                    schema[tableName] = new Dictionary<string, Dictionary<string, object>>
                    {
                        ["columns"] = []
                    };

                    var columns =  _context.Columns.Where(c => c.TABLE_NAME == table.TABLE_NAME).ToList();

                    foreach (var column in columns)
                    {
                        var columnName = column.COLUMN_NAME;
                        if (columnName != null)
                        {
                            var columnInfo = new Dictionary<string, object?>
                            {
                                {"COLUMN_NAME", column.COLUMN_NAME},
                                {"DATA_TYPE", column.DATA_TYPE},
                                {"IS_NULLABLE", column.IS_NULLABLE},
                                {"COLUMN_KEY", column.COLUMN_KEY},
                                {"CHARACTER_MAXIMUM_LENGTH", column.CHARACTER_MAXIMUM_LENGTH},
                            };

                            schema[tableName]["columns"][columnName] = columnInfo;
                        }
                    }
                }
            }

            var json = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync("database_schema.json", json);

            Console.WriteLine("Schema saved to database_schema.json");
        }
    }
}
