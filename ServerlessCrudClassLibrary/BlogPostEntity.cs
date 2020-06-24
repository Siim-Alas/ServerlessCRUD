using Microsoft.Azure.Cosmos.Table;
using System;

namespace ServerlessCrudClassLibrary
{
    public class BlogPostEntity : TableEntity
    {
        public BlogPostEntity(string title, string author, string text = "")
        {
            PartitionKey = title;
            RowKey = author;
            Text = text;
        }

        public string Text { get; set; }
    }
}
