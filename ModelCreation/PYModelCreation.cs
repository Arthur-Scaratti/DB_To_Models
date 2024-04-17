using System.Text.Json;

namespace ModelsCreation
{
    public class PYModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            { "INT", "Integer" },
            { "BIGINT", "BigInteger" },
            { "SMALLINT", "SmallInteger" },
            { "DOUBLE", "Float" },
            { "DECIMAL", "Numeric" },
            { "VARCHAR", "String" },
            { "VARCHAR2", "String" },
            { "TEXT", "Text" },
            { "TIME", "Time" },
            { "DATETIME", "DateTime" },
            { "TIMESTAMP", "DateTime" },
            { "BOOLEAN", "Boolean" },
            { "BLOB", "LargeBinary" },
            { "ENUM", "Enum" },
            { "JSON", "JSON" },
            { "LONG", "Text" },
            { "DATE", "Date" },
            { "URITYPE", "String" },
            { "CHAR", "String" },
            { "NVARCHAR2", "String" },
            { "XMLTYPE", "String" },
            { "NCHAR", "String" },
            { "TIMESTAMP(6) WITH TIME ZONE", "String" },
            { "RAW", "LargeBinary" },
            { "NUMBER", "Integer" },
            { "CLOB", "Text" },
            { "FLOAT", "Float" },
            { "MEDIUMTEXT", "Text" },
            { "LONGTEXT", "Text" },
            { "SET", "String" },
            { "TINYINT", "Integer" },
            { "VARBINARY", "LargeBinary" },
            { "LONGBLOB", "LargeBinary" },
        };

        private string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        public string CriarModelo(string nomeTabela, Dictionary<string, object> infoTabela)
        {
            string nomeTabelaFormatado = FirstCharToUpper(nomeTabela);
            string modelo = $@"
from sqlalchemy import Integer, String, BigInteger, SmallInteger, Float, Numeric, Text
from sqlalchemy import Time, DateTime, Boolean, LargeBinary, Enum, JSON, Text, Date
from sqlalchemy import Column, ForeingKey
from sqlalchemy.orm import declarative_base
from sqlalchemy.orm import Mapped
from sqlalchemy.orm import mapped_column


class {nomeTabelaFormatado}(Base):
    __tablename__ = '{nomeTabela}'
";
            foreach (KeyValuePair<string, object> entrada in infoTabela)
            {
                JsonElement detalhesColuna = (JsonElement)entrada.Value;
                Dictionary<string, string> coluna = new();

                foreach (var property in detalhesColuna.EnumerateObject())
                {
                    coluna.Add(property.Name, property.Value.ToString());
                }

                string tipoDeDado = tipo_de_dado_map[coluna["DATA_TYPE"].ToUpper()];
                string primaryKey = coluna["COLUMN_KEY"] == "PRI" ? ", primary_key=True" : "";
                string nullable = coluna["IS_NULLABLE"] == "NO" ? ", nullable=False" : "";
                string columnName = FirstCharToUpper(entrada.Key);
                modelo += $@"
    {columnName} = Column({tipoDeDado}{primaryKey}{nullable})
";
            }

            modelo += @"
";
            return modelo;
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

                    string modelo = CriarModelo(nomeTabela, detalhesTabela);
                    File.WriteAllText($"{modelosPath}/{nomeTabela}.py", modelo);
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
