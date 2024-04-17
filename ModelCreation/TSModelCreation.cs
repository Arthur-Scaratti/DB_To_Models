using System.Text.Json;

namespace ModelsCreation
{
    public class TSModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            { "INT", "number" },
            { "BIGINT", "number" },
            { "SMALLINT", "number" },
            { "DOUBLE", "number" },
            { "DECIMAL", "number" },
            { "VARCHAR", "string" },
            { "VARCHAR2", "String" },
            { "TEXT", "string" },
            { "TIME", "string" },
            { "DATETIME", "Date" },
            { "TIMESTAMP", "string" },
            { "BOOLEAN", "boolean" },
            { "BLOB", "any" },
            { "ENUM", "string" },
            { "JSON", "any" },
            { "LONG", "string" },
            { "DATE", "string" },
            { "URITYPE", "string" },
            { "CHAR", "string" },
            { "NVARCHAR2", "string" },
            { "XMLTYPE", "string" },
            { "NCHAR", "string" },
            { "TIMESTAMP(6) WITH TIME ZONE", "string" },
            { "RAW", "any" },
            { "NUMBER", "number" },
            { "CLOB", "string" },
            { "FLOAT", "number" },
            { "MEDIUMTEXT", "string" },
            { "LONGTEXT", "string" },
            { "SET", "string" },
            { "TINYINT", "number" },
            { "VARBINARY", "any" },
            { "LONGBLOB", "any" },
        };

        private static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        public string StringModelo(string nomeTabela, Dictionary<string, object> infoTabela)
        {
            string nomeTabelaFormatado = FirstCharToUpper(nomeTabela);
            string classe = $"export interface {nomeTabelaFormatado} {{\n";

            foreach (KeyValuePair<string, object> entrada in infoTabela)
            {
                JsonElement detalhesColuna = (JsonElement)entrada.Value;
                Dictionary<string, string> coluna = [];

                foreach (var property in detalhesColuna.EnumerateObject())
                {
                    coluna.Add(property.Name, property.Value.ToString());
                }

                string tipoDeDado = tipo_de_dado_map[coluna["DATA_TYPE"].ToUpper()];
                string columnName = FirstCharToUpper(entrada.Key);
                classe += $"    {columnName}: {tipoDeDado};\n\n";
            }

            classe += "}";
            return classe;
        }

        public void ModelCreate()
        {
            string databasePath = "./database_schema.json";
            if (!File.Exists(databasePath))
            {
                Console.WriteLine("Arquivo do banco de dados não encontrado.");
                return;
            }

            try
            {
                string json = File.ReadAllText(databasePath);
                Dictionary<string, JsonElement>? database = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (database == null)
                {
                    Console.WriteLine("Erro ao desserializar o arquivo do banco de dados.");
                    return;
                }

                string modelosPath = "../modelos";
                if (!Directory.Exists(modelosPath))
                {
                    Directory.CreateDirectory(modelosPath);
                }

                foreach (KeyValuePair<string, JsonElement> tabela in database)
                {
                    string nomeTabela = tabela.Key;
                    Dictionary<string, object> detalhesTabela = new();

                    foreach (JsonProperty property in tabela.Value.GetProperty("columns").EnumerateObject())
                    {
                        detalhesTabela.Add(property.Name, property.Value);
                    }

                    string classe = StringModelo(nomeTabela, detalhesTabela);
                    File.WriteAllText($"{modelosPath}/{nomeTabela}.ts", classe);
                }
                Console.WriteLine("Modelos gerados com sucesso!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao ler o arquivo do banco de dados: {e.Message}");
            }
        }
    }
}
