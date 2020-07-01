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
using System.Collections.Generic;

namespace ServerlessCrudFunctions
{
    public class GetBlogPostsTableMetadata
    {
        public GetBlogPostsTableMetadata()
        {

        }

        [FunctionName("GetBlogPostsTableMetadata")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                TableQuerySegment<BlogPostEntity> result;
                TableQuery<BlogPostEntity> query = new TableQuery<BlogPostEntity>() 
                { 
                    SelectColumns = new List<string>() { "PartitionKey" } 
                };
                TableContinuationToken token = null;

                int numberOfEntities = 0;

                do
                {
                    result = await table.ExecuteQuerySegmentedAsync(query, token);

                    token = result.ContinuationToken;
                    numberOfEntities += result.Results.Count;
                } while (token != null);

                return new OkObjectResult(new TableMetadata(numberOfEntities));
            }
            catch (Exception e)
            {
                log.LogError($"function GetDlogPostsMetadata -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
