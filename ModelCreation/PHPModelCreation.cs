using System.Text.Json;

namespace ModelsCreation
{
    public class PHPModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            { "INT", "int" },
            { "BIGINT", "long" },
            { "SMALLINT", "short" },
            { "DOUBLE", "double" },
            { "DECIMAL", "decimal" },
            { "VARCHAR", "string" },
            { "TEXT", "string" },
            { "TIME", "string" },
            { "DATETIME", "DateTime" },
            { "TIMESTAMP", "string" },
            { "BOOLEAN", "bool" },
            { "BLOB", "byte[]" },
            { "ENUM", "string" },
            { "JSON", "string" },
            { "LONG", "string" },
            { "DATE", "string" },
            { "URITYPE", "string" },
            { "CHAR", "string" },
            { "NVARCHAR2", "string" },
            { "XMLTYPE", "string"},
            { "NCHAR", "string" },
            { "TIMESTAMP(6) WITH TIME ZONE", "string" },
            { "RAW", "byte[]" },
            { "NUMBER", "int" },
            { "CLOB", "string" },
            { "FLOAT", "float" },
            { "MEDIUMTEXT", "string" },
            { "LONGTEXT", "string" },
            { "SET", "string" },
            { "TINYINT", "int" },
            { "VARBINARY", "byte[]" },
            { "LONGBLOB", "byte[]" },
        };

        private static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        public static string CriarClasse(string nomeTabela, Dictionary<string, object> infoTabela)
        {
            string nomeTabelaFormatado = FirstCharToUpper(nomeTabela);
            string classe = $@"
<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class {nomeTabelaFormatado} extends Model
{{
    protected $table = '{nomeTabela}';
   
";
    List<string> primaryKeyColumns = [];
    foreach (KeyValuePair<string, object> entrada in infoTabela)
            {
                JsonElement detalhesColuna = (JsonElement)entrada.Value;
                Dictionary<string, string> coluna = [];

                foreach (var property in detalhesColuna.EnumerateObject())
                {
                    coluna.Add(property.Name, property.Value.ToString());
                }
                if (coluna["COLUMN_KEY"] == "PRI")
                {
                string columnName = FirstCharToUpper(entrada.Key);
                primaryKeyColumns.Add($"'{columnName}'");
                
                }
            }
            string resultadoFormatado = "[" + string.Join(", ",primaryKeyColumns) + "]";
            classe += $"    protected $primaryKey = {resultadoFormatado};\n";

    classe += $@"
    public $timestamps = false;

    protected $fillable = [
";
            foreach (KeyValuePair<string, object> entrada in infoTabela)
            {
                JsonElement detalhesColuna = (JsonElement)entrada.Value;
                Dictionary<string, string> coluna = [];

                foreach (var property in detalhesColuna.EnumerateObject())
                {
                    coluna.Add(property.Name, property.Value.ToString());
                }

                string columnName = FirstCharToUpper(entrada.Key);
                classe += $"        '{columnName}',\n";
            }

            classe += @"
    ];
}
";
            return classe;
        }

        public static void ModelCreate()
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

                    string classe = CriarClasse(nomeTabela, detalhesTabela);
                    File.WriteAllText($"{modelosPath}/{nomeTabela}.php", classe);
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
