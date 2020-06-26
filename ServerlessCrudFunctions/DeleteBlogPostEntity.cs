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

namespace ServerlessCrudFunctions
{
    public static class DeleteBlogPostEntity
    {
        [FunctionName("DeleteBlogPostEntity")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("function DeleteBlogPostEntity -- started processing request.");

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

                TableOperation deleteOperation = TableOperation.Delete(blogPost);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                log.LogInformation($"function DeleteBlogPostEntity -- got response '{result.HttpStatusCode}' from table '{table.Name}'.");

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogInformation($"function DeleteBlogPostEntity -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
