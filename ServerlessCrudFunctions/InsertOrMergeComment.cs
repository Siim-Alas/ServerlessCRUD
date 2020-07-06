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

        public InsertOrMergeComment(JwtService service)
        {
            _jwtService = service;
        }

        [FunctionName("InsertOrMergeComment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("comments", "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            try
            {
                CommentEntity comment = JsonConvert.DeserializeObject<CommentEntity>(
                    await req.ReadAsStringAsync()
                    );

                if (!comment.IsValid)
                {
                    return new BadRequestErrorMessageResult("The CommentEntity.IsValid check failed.");
                }
                else if (
                    ((await _jwtService.GetClaimsPrincipalAsync(req))
                    .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value != comment.AuthorOID) ||
                    string.IsNullOrEmpty(comment.AuthorOID))
                {
                    // "oid" claim is invalid.
                    return new UnauthorizedResult();
                }

                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(comment);
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
