using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Cosmos;

/**
 * Authors: Theo van Kraay & Chris Joakim, Microsoft, June 2020 
 */

namespace CosmosSqlChangeFeedV3
{
    public static class CosmosSqlChangeFeedV3
    {
        // Environment Variable:                Example Value:
        // AZURE_COSMOSDB_SQLDB_CONN_STRING     <your-cosmosdb-connection-string>

        private static readonly string databaseId = "dev";
        private static readonly string outputContainerId = "changes";
        private static readonly string connString =
            System.Environment.GetEnvironmentVariable("AZURE_COSMOSDB_SQLDB_CONN_STRING");

        private static CosmosClient cosmosClient = new CosmosClient(connString);

        [FunctionName("CosmosSqlChangeFeedV3")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "dev",
            collectionName: "events",
            ConnectionStringSetting = "AZURE_COSMOSDB_SQLDB_CONN_STRING",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> inputList, ILogger log)
        {
            var outputContainer = cosmosClient.GetContainer(databaseId, outputContainerId);

            foreach (Document doc in inputList)
            {
                try
                {
                    doc.SetPropertyValue("_originalId", doc.Id);
                    doc.SetPropertyValue("id", Guid.NewGuid().ToString());
                    await outputContainer.CreateItemAsync<Document>(doc);
                    log.LogInformation("doc saved to outputContainer: " + doc);
                }
                catch (Exception e)
                {
                    log.LogInformation("Exception pushing doc outputContainer: " + e);
                    log.LogInformation("doc NOT saved to outputContainer: " + doc);
                }
            }
        }
    }
}
