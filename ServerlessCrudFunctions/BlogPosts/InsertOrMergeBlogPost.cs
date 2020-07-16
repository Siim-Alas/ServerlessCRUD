using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using ServerlessCrudClassLibrary.TableEntities;
using System.Web.Http;
using ServerlessCrudFunctions.Services;

namespace ServerlessCrudFunctions.BlogPosts
{
    public class InsertOrMergeBlogPost
    {
        private readonly AADJwtService _jwtService;

        public InsertOrMergeBlogPost(AADJwtService service)
        {
            _jwtService = service;
        }

        [FunctionName("InsertOrMergeBlogPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("blogposts", "AzureWebJobsStorage")] CloudTable blogPostsTable,
            [Table("metadata", "AzureWebJobsStorage")] CloudTable metadataTable,
            ILogger log)
        {
            try
            {
                BlogPostEntity blogPost = JsonConvert.DeserializeObject<BlogPostEntity>(
                    await req.ReadAsStringAsync()
                    );

                if (!blogPost.IsValid)
                {
                    return new BadRequestErrorMessageResult("The BlogPostEntity.IsValid check failed.");
                }
                else if (
                    ((await _jwtService.GetClaimsPrincipalAsync(req))
                    .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value != blogPost.AuthorOID) || 
                    string.IsNullOrEmpty(blogPost.AuthorOID))
                {
                    // "oid" claim is invalid.
                    return new UnauthorizedResult();
                }

                // Insert or merge the blog post.
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(blogPost);
                await blogPostsTable.ExecuteAsync(insertOrMergeOperation);

                // Update metadata.
                TableOperation insertOrMergeMetadata = TableOperation.InsertOrMerge(new TableMetadataEntity("blogposts"));
                await metadataTable.ExecuteAsync(insertOrMergeMetadata);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError($"function InsertOrMergeBlogPost -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
