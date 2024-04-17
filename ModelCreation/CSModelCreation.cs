using System.Text.Json;

namespace ModelsCreation
{
    public class CSModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            { "INT", "int" },
            { "BIGINT", "long" },
            { "SMALLINT", "short" },
            { "DOUBLE", "double" },
            { "DECIMAL", "decimal" },
            { "VARCHAR", "string?" },
            { "VARCHAR2", "string?" },
            { "TEXT", "string?" },
            { "TIME", "string?" },
            { "DATETIME", "DateTime?" },
            { "TIMESTAMP", "string?" },
            { "BOOLEAN", "bool" },
            { "BLOB", "byte[]?" },
            { "ENUM", "string?" },
            { "JSON", "string?" },
            { "LONG", "string?" },
            { "DATE", "string?" },
            { "URITYPE", "string?" },
            { "CHAR", "string?" },
            { "NVARCHAR2", "string?" },
            { "XMLTYPE", "string? "},
            { "NCHAR", "string?" },
            { "TIMESTAMP(6) WITH TIME ZONE", "string?" },
            { "RAW", "byte[]?" },
            { "NUMBER", "int" },
            { "CLOB", "string?" },
            { "FLOAT", "float" },
            { "MEDIUMTEXT", "string?" },
            { "LONGTEXT", "string?" },
            { "SET", "string?" },
            { "TINYINT", "int" },
            { "VARBINARY", "byte[]?" },
            { "LONGBLOB", "byte[]?" },
            
        };

        private static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        
        private static string CriarAnotacoes(Dictionary<string, string> coluna)
        {
            List<string> anotacoes = [];

            if (coluna["IS_NULLABLE"] == "NO")
            {
                anotacoes.Add("[Required]");
            }

            if (coluna["DATA_TYPE"] == "varchar")
            {
                anotacoes.Add($"[StringLength({coluna["CHARACTER_MAXIMUM_LENGTH"]})]");
            }

            if (coluna["COLUMN_KEY"] == "PRI")
            {
                anotacoes.Add("[Key]");
            }

            return anotacoes.Count == 0 ? "" : "    " + string.Join("\n        ", anotacoes);
        }

        public string StringModelo(string nomeTabela, Dictionary<string, object> infoTabela)
        {
            string nomeTabelaFormatado = FirstCharToUpper(nomeTabela);
              string classe = $@"
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{{
    public class {nomeTabelaFormatado}
    {{
";
    foreach (KeyValuePair<string, object> entrada in infoTabela)
    {
        JsonElement detalhesColuna = (JsonElement)entrada.Value;
        Dictionary<string, string> coluna = [];

        foreach (var property in detalhesColuna.EnumerateObject())
        {
            coluna.Add(property.Name, property.Value.ToString());
        }

        string anotacoes = CriarAnotacoes(coluna);
        string tipoDeDado = tipo_de_dado_map[coluna["DATA_TYPE"].ToUpper()];
        string comentario = coluna.TryGetValue("COLUMN_NAME", out string? value) ? value : "";
        string columnName = FirstCharToUpper(entrada.Key);
        classe += $@"
    {anotacoes}
        public {tipoDeDado} {columnName} {{ get; set; }}
        /// {comentario}";
    }

    classe += @"
    }
}
";
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
            Dictionary<string, object> detalhesTabela = [];

            foreach (JsonProperty property in tabela.Value.GetProperty("columns").EnumerateObject())
            {
                detalhesTabela.Add(property.Name, property.Value);
            }

            string classe = StringModelo(nomeTabela, detalhesTabela);
            File.WriteAllText($"{modelosPath}/{nomeTabela}.cs", classe);
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
