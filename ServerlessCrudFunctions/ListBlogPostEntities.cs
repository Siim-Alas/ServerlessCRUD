using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCrudClassLibrary;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;
using System.Web.Http;

namespace ServerlessCrudFunctions
{
    public static class ListBlogPostEntities
    {
        [FunctionName("ListBlogPostEntities")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("function ListBlogPostEntities -- started processing request.");

            try
            {
                ListBlogPostEntitiesRequest request = JsonConvert.DeserializeObject<ListBlogPostEntitiesRequest>(
                    await req.ReadAsStringAsync()
                    );

                var result = await table.ExecuteQuerySegmentedAsync(
                    new TableQuery<BlogPostEntity>() { TakeCount = request.TakeCount },
                    request.ContinuationToken);

                request.BlogPosts = result.Results;
                request.ContinuationToken = result.ContinuationToken;

                log.LogInformation($"function ListBlogPostEntities -- got response with '{result.Results.Count}' entities from table '{table.Name}'.");

                return new OkObjectResult(request);
            }
            catch (Exception e)
            {
                log.LogInformation($"function ListBlogPostEntities -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
