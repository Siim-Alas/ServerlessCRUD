using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using Microsoft.Azure.Cosmos.Table;
using ServerlessCrudClassLibrary;
using Microsoft.Azure.Cosmos.Table.Queryable;
using System.Collections.Generic;

namespace ServerlessCrudFunctions
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
                //TableQuery<CommentEntity> commentQuery = table.CreateQuery<CommentEntity>();
                //TableQuery<CommentEntity> query = (from comment in commentQuery
                //                                   where comment.PartitionKey == req.Query["commentpartitionkey"]
                //                                   select comment).AsTableQuery();

                // Large numbers (over 1000) are not expected.
                //return new OkObjectResult(query.Execute());

                TableQuerySegment<BlogPostEntity> result = await table.ExecuteQuerySegmentedAsync(
                    new TableQuery<BlogPostEntity>().Where(
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
