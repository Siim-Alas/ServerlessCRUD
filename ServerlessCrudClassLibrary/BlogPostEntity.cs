using Microsoft.Azure.Cosmos.Table;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServerlessCrudClassLibrary
{
    public class BlogPostEntity : TableEntity
    {
        public BlogPostEntity() : base()
        {
            
        }
        public BlogPostEntity(string title, string author, string text = "")
        {
            PartitionKey = title;
            RowKey = author;
            Text = text;
        }
        [Required]
        public string Text { get; set; }
    }
}
