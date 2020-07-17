using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;

namespace ServerlessCrudFunctions.Images
{
    public class GetImage
    {
        public GetImage()
        {

        }

        [FunctionName("GetImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetImage/{name}")] HttpRequest req,
            [Blob("images/{name}", FileAccess.Read)] Stream image,
            string name,
            ILogger log)
        {
            try
            {
                FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(name, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                byte[] fileContents = new byte[image.Length];
                await image.ReadAsync(fileContents);

                return new FileContentResult(fileContents, contentType);
            }
            catch (Exception e)
            {
                log.LogError($"function GetImage -- caught exception {e} {e.Message} {e.StackTrace}");
                return new OkObjectResult(e);
            }
        }
    }
}
