using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using System.Linq;
using System.Collections;

namespace ServerlessCrudFunctions.Images
{
    class ListImageNames
    {
        public ListImageNames()
        {

        }

        [FunctionName("ListImageNames")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("images", FileAccess.Read)] CloudBlobContainer container,
            ILogger log)
        {
            try
            {
                IEnumerable blobNames = container.ListBlobs().OfType<CloudBlockBlob>().Select(b => b.Name);
                return new OkObjectResult(blobNames);
            }
            catch (Exception e)
            {
                log.LogError($"function ListImageNames -- caught exception {e} {e.Message} {e.StackTrace}");
                return new OkObjectResult(e);
            }
        }
    }
}
