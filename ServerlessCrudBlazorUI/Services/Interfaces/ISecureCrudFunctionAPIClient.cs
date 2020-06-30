using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.Interfaces
{
    public interface ISecureCrudFunctionAPIClient : IAnnonymousCrudFunctionAPIClient
    {
        Task<HttpResponseMessage> PostBlogPostAsync(BlogPostEntity blogPost);
        Task<HttpResponseMessage> PostDeleteBlogPostEntityAsync(BlogPostEntity blogPost);
    }
}
