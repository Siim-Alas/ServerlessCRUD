using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Web.Http;
using ServerlessCrudClassLibrary;

namespace ServerlessCrudFunctions
{
    public static class ReadBlogPostEntity
    {
        [FunctionName("ReadBlogPostEntity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("function ReadBlogPostEntity -- started processing request.");

            try
            {
                await table.CreateIfNotExistsAsync();

                TableResult result = await table.ExecuteAsync(TableOperation.Retrieve<BlogPostEntity>(
                    req.Query["partitionkey"], 
                    req.Query["rowkey"]));

                log.LogInformation($"function ReadBlogPostEntity -- got response '{result.HttpStatusCode}' from table '{table.Name}'.");

                return new OkObjectResult(JsonConvert.SerializeObject((BlogPostEntity)result.Result));
            }
            catch (Exception e)
            {
                log.LogInformation($"function ReadBlogPostEntity -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
