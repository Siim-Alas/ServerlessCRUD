using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class CommentEntity : TableEntity
    {
        public CommentEntity() : base()
        {

        }
        public CommentEntity(BlogPostEntity blogPost, string author, string text = "")
        {
            PartitionKey = $"{blogPost.PartitionKey}{blogPost.RowKey}";
            SetRowKey(author);
            Text = text;
        }

        [Required]
        public string Text { get; set; }

        [IgnoreProperty]
        [JsonIgnore]
        public string Author 
        {
            // There are 19 digits and 1 underscore, the (0-based) author starts at 20
            get { return RowKey.Substring(20); } 
        }

        private void SetRowKey(string author)
        {
            RowKey = string.Format("{0}_{1}",
                    RowKey?.Substring(0, 19) ?? (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    author);
        }
    }
}
