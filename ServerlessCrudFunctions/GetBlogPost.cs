using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Web.Http;
using ServerlessCrudClassLibrary;

namespace ServerlessCrudFunctions
{
    public class GetBlogPost
    {
        public GetBlogPost()
        {

        }

        [FunctionName("GetBlogPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                TableResult result = await table.ExecuteAsync(TableOperation.Retrieve<BlogPostEntity>(
                    req.Query["partitionkey"], 
                    req.Query["rowkey"]));

                log.LogInformation($"function GetBlogPost -- got response '{result.HttpStatusCode}' from table '{table.Name}'.");

                return new OkObjectResult(result.Result);
            }
            catch (Exception e)
            {
                log.LogError($"function GetBlogPost -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
