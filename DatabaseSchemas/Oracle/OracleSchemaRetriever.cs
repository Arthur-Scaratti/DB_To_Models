using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DatabaseConnections;
using Models;

namespace Schema
{
    public class OracleSchemaRetriever
    {
        private readonly OracleDBContext _context;
        public OracleSchemaRetriever()
        {
            var oracleConnectionString = "User Id=compiereprod;Password=DIEB3G5PPRT;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.0.3)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = diementz)))"; // Sua string de conexão aqui
            var options = new DbContextOptionsBuilder<OracleDBContext>()
                .UseOracle(oracleConnectionString)
                .Options;
            _context = new OracleDBContext(options);
        }

        public async Task RetrieveSchemaAsync()
        {
            var schema = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            var tables = _context.ALL_TABLES.Where(a => a.OWNER == "COMPIEREPROD").ToList();
            

            foreach (var table in tables)
            {
                
                
                var tableName = table.TABLE_NAME;
                Console.WriteLine(tableName);
                if (tableName != null)
                {
                    schema[tableName] = new Dictionary<string, Dictionary<string, object>>
                    {
                        ["columns"] = []
                    };

                    var columnsWithPrimaryKey = from col in _context.ALL_TAB_COLUMNS
                            where col.TABLE_NAME == table.TABLE_NAME
                            join consCol in _context.ALL_CONS_COLUMNS on new { col.TABLE_NAME, col.COLUMN_NAME } equals new { consCol.TABLE_NAME, consCol.COLUMN_NAME } into gj
                            from subconsCol in gj.DefaultIfEmpty()
                            let isPrimaryKey = _context.ALL_CONSTRAINTS.
                            Any(ac => ac.OWNER == "COMPIEREPROD" && ac.TABLE_NAME == subconsCol.TABLE_NAME && ac.CONSTRAINT_NAME == subconsCol.CONSTRAINT_NAME && ac.CONSTRAINT_TYPE == "P")
                            select new
                            {
                                col.COLUMN_NAME,
                                col.DATA_TYPE,
                                col.NULLABLE,
                                IsPrimaryKey = isPrimaryKey ? "PRI" : "N",
                                col.DATA_LENGTH
                            };

                    foreach (var column in columnsWithPrimaryKey)
                    {
                        var columnName = column.COLUMN_NAME;
                        Console.WriteLine(columnName);
                        Console.WriteLine(column.IsPrimaryKey);
                        if (columnName != null)
                        {
                            var columnInfo = new Dictionary<string, object?>
                            {
                                {"COLUMN_NAME", column.COLUMN_NAME},
                                {"DATA_TYPE", column.DATA_TYPE},
                                {"IS_NULLABLE", column.NULLABLE},
                                {"COLUMN_KEY", column.IsPrimaryKey},
                                {"CHARACTER_MAXIMUM_LENGTH", column.DATA_LENGTH},
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
