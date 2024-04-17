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






        //Oracle
        //string oracleConnectionString = 
        //"User Id=compiereprod;Password=DIEB3G5PPRT;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.0.3)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = diementz)))";
        
        //bool connectedToOracle = DatabaseConnections.OracleConn.TestConnection(oracleConnectionString);
        //if (connectedToOracle)
        //{
        // var schema = DatabaseConnections.OracleSchemaRetriever.RetrieveSchema(oracleConnectionString);
        //}