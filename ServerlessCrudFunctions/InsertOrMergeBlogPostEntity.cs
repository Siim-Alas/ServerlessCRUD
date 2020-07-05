using System;
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
using ServerlessCrudFunctions.Services;
using System.Linq;

namespace ServerlessCrudFunctions
{
    public class InsertOrMergeBlogPostEntity
    {
        private readonly JwtService _jwtService;

        public InsertOrMergeBlogPostEntity(JwtService service)
        {
            _jwtService = service;
        }

        [FunctionName("InsertOrMergeBlogPostEntity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                BlogPostEntity blogPost = JsonConvert.DeserializeObject<BlogPostEntity>(
                    await req.ReadAsStringAsync()
                    );

                if (!blogPost.IsValid)
                {
                    return new BadRequestErrorMessageResult("The BlogPost.IsValid check failed.");
                }
                else if (
                    ((await _jwtService.GetClaimsPrincipalAsync(req))
                    .Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")
                    .First().Value != blogPost.AuthorOID) || 
                    string.IsNullOrEmpty(blogPost.AuthorOID))
                {
                    // "oid" claim is invalid.
                    return new UnauthorizedResult();
                }

                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(blogPost);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError($"function InsertOrMergeBlogPostEntity -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
