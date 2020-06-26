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
using ServerlessCrudClassLibrary;
using System.Web.Http;
using System.ComponentModel;

namespace ServerlessCrudFunctions
{
    public static class InsertOrMergeBlogPostEntity
    {
        [FunctionName("InsertOrMergeBlogPostEntity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("function InsertOrMergeBlogPostEntity -- started processing request.");

            try
            {
                await table.CreateIfNotExistsAsync();

                BlogPostEntity blogPost = JsonConvert.DeserializeObject<BlogPostEntity>(
                    await req.ReadAsStringAsync()
                    );

                if (!blogPost.IsValid)
                {
                    throw new FormatException("The BlogPostEntity.Isvalid check failed.");
                }

                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(blogPost);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

                log.LogInformation($"function InsertOrMergeBlogPostEntity -- got response '{result.HttpStatusCode}' from table '{table.Name}'");

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogInformation($"function InsertOrMergeBlogPostEntity -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
