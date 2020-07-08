using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCrudFunctions.Services;
using Microsoft.Azure.Cosmos.Table;
using ServerlessCrudClassLibrary;
using System.Web.Http;
using System.Linq;

namespace ServerlessCrudFunctions
{
    public class InsertOrMergeComment
    {
        private readonly JwtService _jwtService;
        private readonly IdentityAPIClient _client;

        public InsertOrMergeComment(JwtService service, IdentityAPIClient client)
        {
            _jwtService = service;
            _client = client;
        }

        [FunctionName("InsertOrMergeComment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("comments", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                PostCommentEntityRequest request = JsonConvert.DeserializeObject<PostCommentEntityRequest>(
                    await req.ReadAsStringAsync()
                    );

                switch (request.IdentityProvider)
                {
                    case PostCommentEntityRequest.IdentityProviders.Facebook:
                        if (request.UserID != (await _client.GetFacebookMeResponseAsync(request.AccessToken)).Id)
                        {
                            return new UnauthorizedResult();
                        }
                        break;
                    default:
                        return new BadRequestErrorMessageResult("No identity provider was specified.");
                }

                if (!request.Comment.IsValid)
                {
                    return new BadRequestErrorMessageResult("The CommentEntity.IsValid check failed.");
                }

                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(request.Comment);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError($"function InsertOrMergeComment -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
