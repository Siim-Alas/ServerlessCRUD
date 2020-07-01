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
using ServerlessCrudFunctions.Services;
using System.Security.Claims;
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
                else if (blogPost.Author != (await _jwtService.GetClaimsPrincipalAsync(req))
                    .Claims.Where(claim => claim.Type == "name").First().Value)
                {
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
