using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ModelsCreation
{
    public class JavaModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            // Mapeamento de tipos de dados para Java
            { "INT", "int" },
            { "BIGINT", "long" },
            { "SMALLINT", "short" },
            { "DOUBLE", "double" },
            { "DECIMAL", "double" },
            { "VARCHAR", "String" },
            { "VARCHAR2", "String" },
            { "TEXT", "String" },
            { "TIME", "String" },
            { "DATETIME", "Date" },
            { "TIMESTAMP", "String" },
            { "BOOLEAN", "boolean" },
            { "BLOB", "byte[]" },
            { "ENUM", "String" },
            { "JSON", "String" },
            { "LONG", "String" },
            { "DATE", "String" },
            { "URITYPE", "String" },
            { "CHAR", "String" },
            { "NVARCHAR2", "String" },
            { "XMLTYPE", "String" },
            { "NCHAR", "String" },
            { "TIMESTAMP(6) WITH TIME ZONE", "String" },
            { "RAW", "byte[]" },
            { "NUMBER", "int" },
            { "CLOB", "String" },
            { "FLOAT", "float" },
            { "MEDIUMTEXT", "String" },
            { "LONGTEXT", "String" },
            { "SET", "String" },
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

        public string StringModelo(string nomeTabela, Dictionary<string, object> infoTabela)
        {
            string nomeTabelaFormatado = FirstCharToUpper(nomeTabela);
            string classe = $"import javax.persistence.Entity;\nimport javax.persistence.Id;\n";
            classe += $"import javax.persistence.Column;\n\n@Entity\n";
            classe += $"public class {nomeTabela} {{\n";

            foreach (KeyValuePair<string, object> entrada in infoTabela)
            {
                JsonElement detalhesColuna = (JsonElement)entrada.Value;
                Dictionary<string, string> coluna = new();

                foreach (var property in detalhesColuna.EnumerateObject())
                {
                    coluna.Add(property.Name, property.Value.ToString());
                }

                string tipoDeDado = tipo_de_dado_map[coluna["DATA_TYPE"].ToUpper()];
                string columnName = FirstCharToUpper(entrada.Key);
                string anotacoes = "";

                if (coluna["COLUMN_KEY"] == "PRI")
                {
                    anotacoes += "    @Id\n";
                }

                classe += anotacoes;
                classe += $"    private {tipoDeDado} {columnName};\n\n ";
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
                    File.WriteAllText($"{modelosPath}/{nomeTabela}.java", classe);
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
