﻿using Microsoft.JSInterop;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class CrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public CrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> PostBlogPostAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "https://serverlesscrud.azurewebsites.net/api/InsertOrMergeBlogPostEntity?code=daYVnhnXf3c4fNZJF3DLbRinnLuI8aAxpw9t2gfb7aVasuCFq6I4RQ==",
                    blogPost);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
