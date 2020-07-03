using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class QueryBlogPostEntitiesRequest
    {
        public QueryBlogPostEntitiesRequest(int skip = 0, 
                                           string filterString = null, 
                                           IList<string> selectColumns = null,
                                           int takeCount = 4,
                                           TableContinuationToken continuationToken = null,
                                           List<BlogPostEntity> blogPosts = null)
        {
            Skip = skip;

            FilterString = filterString;
            SelectColumns = selectColumns;
            TakeCount = takeCount;
            ContinuationToken = continuationToken;

            BlogPosts = blogPosts ?? new List<BlogPostEntity>();
        }
        public int Skip { get; set; }
        public string FilterString { get; set; }
        public IList<string> SelectColumns { get; set; }
        public int TakeCount { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }
        public List<BlogPostEntity> BlogPosts { get; set; }
    }
}
