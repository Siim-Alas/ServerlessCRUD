using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class ListBlogPostEntitiesRequest
    {
        public ListBlogPostEntitiesRequest(int takeCount = 4, 
                                           TableContinuationToken continuationToken = null,
                                           List<BlogPostEntity> blogPosts = null)
        {
            TakeCount = takeCount;
            ContinuationToken = continuationToken;
            BlogPosts = blogPosts ?? new List<BlogPostEntity>();
        }
        public int TakeCount { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }
        public List<BlogPostEntity> BlogPosts { get; set; }
    }
}
