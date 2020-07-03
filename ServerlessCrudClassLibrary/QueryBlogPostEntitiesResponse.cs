using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class QueryBlogPostEntitiesResponse
    {
        public QueryBlogPostEntitiesResponse()
        {

        }
        public QueryBlogPostEntitiesResponse(
            TableContinuationToken continuationToken = null, 
            List<BlogPostEntity> blogPosts = null)
        {
            ContinuationToken = continuationToken;
            BlogPosts = blogPosts ?? new List<BlogPostEntity>();
        }
        public TableContinuationToken ContinuationToken { get; set; }
        public List<BlogPostEntity> BlogPosts { get; set; }
    }
}
