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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ServerlessCrudFunctions
{
    public class QueryBlogPostEntities
    {
        public QueryBlogPostEntities()
        {
            
        }

        [FunctionName("QueryBlogPostEntities")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                // Int default value isn't null so it is assumed to be always included.
                int skip = Convert.ToInt32(req.Query["skip"]);
                int takeCount = Convert.ToInt32(req.Query["takecount"]);

                req.Query.TryGetValue("filterstring", out var filterString);

                List<string> selectColumns = 
                    req.Query.TryGetValue("selectcolumns", out var selectColumnsString) ?
                    JsonConvert.DeserializeObject<List<string>>(selectColumnsString) : 
                    new List<string>() { "PartitionKey" };

                TableContinuationToken token = 
                    (req.Query.TryGetValue("newxtpartitionkey", out var nextPk) &&
                    req.Query.TryGetValue("nextrowkey", out var nextRk)) ?
                    new TableContinuationToken()
                    {
                        NextPartitionKey = nextPk,
                        NextRowKey = nextRk,
                        NextTableName = null,
                        TargetLocation = StorageLocation.Primary
                    } : null;

                while (skip > 0)
                {
                    TableQuerySegment<BlogPostEntity> r = await table.ExecuteQuerySegmentedAsync(
                        new TableQuery<BlogPostEntity>() 
                        { 
                            TakeCount = skip, 
                            SelectColumns = new List<string>() { "PartitionKey" }
                        }, 
                        token
                        );
                    skip -= r.Results.Count;
                    token = r.ContinuationToken;
                }
                
                TableQuerySegment<BlogPostEntity> result = await table.ExecuteQuerySegmentedAsync(
                    new TableQuery<BlogPostEntity>()
                    {
                        FilterString = filterString,
                        SelectColumns = selectColumns,
                        TakeCount = takeCount
                    },
                    token);

                return new OkObjectResult(new QueryBlogPostEntitiesResponse(token, result.Results));
            }
            catch (Exception e)
            {
                log.LogError($"function QueryBlogPostEntities -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
