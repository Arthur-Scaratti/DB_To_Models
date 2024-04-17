using System.Text.Json;

namespace ModelsCreation
{
    public class NODEModelCreation
    {
        private readonly Dictionary<string, string> tipo_de_dado_map = new()
        {
            { "INT", "DataTypes.INTEGER" },
            { "BIGINT", "DataTypes.BIGINT" },
            { "SMALLINT", "DataTypes.SMALLINT" },
            { "DOUBLE", "DataTypes.DOUBLE" },
            { "DECIMAL", "DataTypes.DECIMAL" },
            { "VARCHAR", "DataTypes.STRING" },
            { "VARCHAR2", "DataTypes.STRING" },
            { "TEXT", "DataTypes.TEXT" },
            { "TIME", "DataTypes.TIME" },
            { "DATETIME", "DataTypes.DATE" },
            { "TIMESTAMP", "DataTypes.DATE" },
            { "BOOLEAN", "DataTypes.BOOLEAN" },
            { "BLOB", "DataTypes.BLOB" },
            { "ENUM", "DataTypes.ENUM" },
            { "JSON", "DataTypes.JSON" },
            { "LONG", "DataTypes.STRING" },
            { "DATE", "DataTypes.DATEONLY" },
            { "URITYPE", "DataTypes.STRING" },
            { "CHAR", "DataTypes.CHAR" },
            { "NVARCHAR2", "DataTypes.STRING" },
            { "XMLTYPE", "DataTypes.STRING" },
            { "NCHAR", "DataTypes.STRING" },
            { "TIMESTAMP(6) WITH TIME ZONE", "DataTypes.STRING" },
            { "RAW", "DataTypes.BLOB" },
            { "NUMBER", "DataTypes.INTEGER" },
            { "CLOB", "DataTypes.TEXT" },
            { "FLOAT", "DataTypes.FLOAT" },
            { "MEDIUMTEXT", "DataTypes.TEXT" },
            { "LONGTEXT", "DataTypes.TEXT" },
            { "SET", "DataTypes.STRING" },
            { "TINYINT", "DataTypes.INTEGER" },
            { "VARBINARY", "DataTypes.BLOB" },
            { "LONGBLOB", "DataTypes.BLOB" },
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

            string modelo = $@"
const Sequelize = require('sequelize');
const DataTypes = Sequelize.DataTypes;

const {nomeTabelaFormatado} = Sequelize.define('{nomeTabela}', {{
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
                string primaryKey = coluna["COLUMN_KEY"] == "PRI" ? "primaryKey: true," : "";
                string columnName = FirstCharToUpper(entrada.Key);
                modelo += $@"
    {columnName}: {{
        type: {tipoDeDado},
        allowNull: {(coluna["IS_NULLABLE"] == "NO" ? "false" : "true")},
        {primaryKey}
    }},
";
            }

            modelo += @"
}, {
    timestamps: false,

});

module.exports = " + nomeTabelaFormatado + ";";
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

                    string modelo = StringModelo(nomeTabela, detalhesTabela);
                    File.WriteAllText($"{modelosPath}/{nomeTabela}.js", modelo);
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
