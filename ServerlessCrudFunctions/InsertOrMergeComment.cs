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
using System.Web.Http;
using ServerlessCrudClassLibrary.HttpRequestModels;
using ServerlessCrudClassLibrary.HttpResponseModels;

namespace ServerlessCrudFunctions
{
    public class InsertOrMergeComment
    {
        private readonly IdentityAPIClient _client;

        public InsertOrMergeComment(IdentityAPIClient client)
        {
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
                        FacebookMeResponse fr = await _client.GetFacebookMeResponseAsync(request.Token);
                        if ((fr.Name != request.Comment.Author) ||
                            (fr.Id != request.UserID))
                        {
                            return new UnauthorizedResult();
                        }
                        break;
                    case PostCommentEntityRequest.IdentityProviders.Google:
                        GoogleIdTokenResponse gr = await _client.GetGoogleIdTokenResponseAsync(request.Token);
                        if ((gr.Iss != "accounts.google.com") ||
                            (gr.Sub != request.UserID) ||
                            (gr.Aud != "704066166279-dunk2o8blaedb7l149qnu5mphsm1jo69.apps.googleusercontent.com") || 
                            (DateTimeOffset.FromUnixTimeSeconds(gr.Exp).UtcDateTime < DateTime.UtcNow) || 
                            (gr.Name != request.Comment.Author))
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
