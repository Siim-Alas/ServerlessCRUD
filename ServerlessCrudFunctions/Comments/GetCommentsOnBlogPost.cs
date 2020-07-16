using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Web.Http;
using Microsoft.Azure.Cosmos.Table;
using ServerlessCrudClassLibrary.TableEntities;

namespace ServerlessCrudFunctions.Comments
{
    public class GetCommentsOnBlogPost
    {
        public GetCommentsOnBlogPost()
        {

        }

        [FunctionName("GetCommentsOnBlogPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("comments", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                TableQuerySegment<CommentEntity> result = await table.ExecuteQuerySegmentedAsync(
                    new TableQuery<CommentEntity>().Where(
                        TableQuery.GenerateFilterCondition(
                            "PartitionKey", 
                            QueryComparisons.Equal, 
                            req.Query["commentpartitionkey"])
                        ),
                    null);

                return new OkObjectResult(result.Results);
            }
            catch (Exception e)
            {
                log.LogError($"function GetCommentsOnBlogPost -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
