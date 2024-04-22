using Microsoft.EntityFrameworkCore;
using DatabaseConnections;
using Schema;
using ModelsCreation;
namespace index
{
    class Program
    {
        static void Main(string[] args)
        {

            // MariaDBSchemaRetriever mariaDBSchemaRetriever = new();
            // await mariaDBSchemaRetriever.RetrieveSchemaAsync();
            // OracleSchemaRetriever oracleSchemaRetriever = new();
            //await oracleSchemaRetriever.RetrieveSchemaAsync();

            CSModelCreation cSModelCreation = new();
            TSModelCreation tSModelCreation = new();
            JavaModelCreation javaModelCreation = new();
            NODEModelCreation nodeModelsCreation = new();
            PYModelCreation pyModelCreation = new();
            PHPModelCreation phpModelCreation = new();
            PHPModelCreation.ModelCreate();
            cSModelCreation.ModelCreate();
            tSModelCreation.ModelCreate();
            javaModelCreation.ModelCreate();
            nodeModelsCreation.ModelCreate();
            pyModelCreation.ModelCreate();

        }
    }
}

