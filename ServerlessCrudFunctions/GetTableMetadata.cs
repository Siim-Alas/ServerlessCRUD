using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using ServerlessCrudClassLibrary.TableEntities;
using System.Web.Http;

namespace ServerlessCrudFunctions
{
    public class GetTableMetadata
    {
        public GetTableMetadata()
        {

        }

        [FunctionName("GetTableMetadata")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("metadata", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                TableResult result = await table.ExecuteAsync(TableOperation.Retrieve<TableMetadataEntity>(
                    req.Query["tablename"],
                    "metadata"));

                log.LogInformation($"function GetTableMetadata -- got response '{result.HttpStatusCode}' from table '{table.Name}'.");

                return new OkObjectResult(result.Result);
            }
            catch (Exception e)
            {
                log.LogError($"function GetTableMetadata -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
