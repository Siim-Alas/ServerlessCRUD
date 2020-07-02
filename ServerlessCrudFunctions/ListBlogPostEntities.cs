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
using System.Security.Claims;
using System.Runtime.CompilerServices;
using System.IdentityModel.Tokens.Jwt;
using ServerlessCrudFunctions.Services;

namespace ServerlessCrudFunctions
{
    public class ListBlogPostEntities
    {
        public ListBlogPostEntities()
        {
            
        }

        [FunctionName("ListBlogPostEntities")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                ListBlogPostEntitiesRequest request = JsonConvert.DeserializeObject<ListBlogPostEntitiesRequest>(
                    await req.ReadAsStringAsync()
                    );

                while (request.Skip > 0)
                {
                    TableQuerySegment<BlogPostEntity> r = await table.ExecuteQuerySegmentedAsync(
                        new TableQuery<BlogPostEntity>() 
                        { 
                            TakeCount = request.Skip, 
                            SelectColumns = new List<string>() { "PartitionKey" }
                        }, 
                        request.ContinuationToken
                        );
                    request.Skip -= r.Results.Count;
                    request.ContinuationToken = r.ContinuationToken;
                }

                TableQuerySegment<BlogPostEntity> result = await table.ExecuteQuerySegmentedAsync(
                    new TableQuery<BlogPostEntity>() { TakeCount = request.TakeCount },
                    request.ContinuationToken);

                request.BlogPosts = result.Results;
                request.ContinuationToken = result.ContinuationToken;

                return new OkObjectResult(request);
            }
            catch (Exception e)
            {
                log.LogError($"function ListBlogPostEntities -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
