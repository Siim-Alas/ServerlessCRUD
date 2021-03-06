using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Web.Http;
using ServerlessCrudFunctions.Services;
using ServerlessCrudClassLibrary.TableEntities;

namespace ServerlessCrudFunctions.BlogPosts
{
    public class DeleteBlogPost
    {
        private readonly AADJwtService _jwtService;

        public DeleteBlogPost(AADJwtService service)
        {
            _jwtService = service;
        }

        [FunctionName("DeleteBlogPost")]
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
                    return new BadRequestErrorMessageResult("The BlogPost.IsValid check failed.");
                }
                else if (
                    ((await _jwtService.GetClaimsPrincipalAsync(req))
                    .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value != blogPost.AuthorOID) ||
                    string.IsNullOrEmpty(blogPost.AuthorOID))
                {
                    // "oid" claim is invalid.
                    return new UnauthorizedResult();
                }

                // Delete the blog post.
                TableOperation deleteOperation = TableOperation.Delete(blogPost);
                TableResult result = await blogPostsTable.ExecuteAsync(deleteOperation);

                // Update metadata.
                TableOperation insertOrMergeMetadata = TableOperation.InsertOrMerge(new TableMetadataEntity("blogposts"));
                await metadataTable.ExecuteAsync(insertOrMergeMetadata);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError($"function DeleteBlogPost -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
