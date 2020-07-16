using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Web.Http;
using ServerlessCrudFunctions.Services;

namespace ServerlessCrudFunctions.Images
{
    public class UploadImage
    {
        private readonly AADJwtService _jwtService;

        public UploadImage(AADJwtService service)
        {
            _jwtService = service;
        }

        [FunctionName("UploadImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadImage/{name}")] HttpRequest req,
            [Blob("images/{name}", FileAccess.Write)] Stream image,
            ILogger log)
        {
            try
            {
                await req.Body.CopyToAsync(image);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError($"function UploadImage -- caught exception {e} {e.Message} {e.StackTrace}");
                return new InternalServerErrorResult();
            }
        }
    }
}
