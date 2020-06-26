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
        /// <summary>
        /// Creates a new BlogPostEntity.
        /// </summary>
        /// <param name="author">The author of the post, used as PartitionKey.</param>
        /// <param name="title">The title of the post, saved in the RowKey after '<TICKS>_'.</param>
        /// <param name="text">The text of the post.</param>
        public BlogPostEntity(string author, string title, string text = "")
        {
            Author = author;
            Title = title;
            Text = text;
        }
        /// <summary>
        /// The text of the blog post.
        /// </summary>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the author of the post -- the PartitionKey.
        /// </summary>
        [Required]
        [IgnoreProperty]
        public string Author { get { return PartitionKey; } set { PartitionKey = value; } }
        /// <summary>
        /// Gets or sets the title of the post -- the subsrting of RowKey after '<TICKS>_'.
        /// </summary>
        [Required]
        [IgnoreProperty]
        public string Title { 
            get { return RowKey.Substring(RowKey.IndexOf('_') + 1); } 
            set { RowKey = string.Format("{0}_{1}",
                    (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    value);
            } }
    }
}
