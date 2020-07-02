using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        /// <param name="title">The title of the post, saved in the RowKey (REVERSETICKS_TITLE).</param>
        /// <param name="author">The author of the post.</param>
        /// <param name="text">The text of the post.</param>
        public BlogPostEntity(string title, string author, string text = "")
        {
            PartitionKey = $"{9999 - DateTime.UtcNow.Year}{99 - DateTime.UtcNow.Month}";
            SetRowKey(title);
            Author = author;
            Text = text;
        }
        /// <summary>
        /// Gets or sets the author of the post.
        /// </summary>
        [Required]
        public string Author { get; set; }
        /// <summary>
        /// The text of the blog post.
        /// </summary>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the title of the post, saved in the RowKey (REVERSETICKS_TITLE).
        /// </summary>
        [Required]
        [IgnoreProperty]
        [JsonIgnore]
        public string Title 
        { 
            // There are 19 digits and 1 underscore, the (0-based) title starts at 20
            get { return RowKey.Substring(20); } 
            set { SetRowKey(value); }
        }
        /// <summary>
        /// Checks if the PartitionKey and RowKey values are valid and that Timestamp and Text are not null.
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public bool IsValid 
        { 
            get 
            {
                return 
                    Regex.IsMatch(PartitionKey, @"^[0-9]{6}$") && 
                    Regex.IsMatch(RowKey, @"^[0-9]{19}_.+$") && 
                    Timestamp != null &&
                    Author != null &&
                    Text != null;
            }
        }
        /// <summary>
        /// Sets the RowKey in the format (REVERSETICKS_TITLE_AUTHOR).
        /// </summary>
        /// <param name="title">The title to be saved in RowKey.</param>
        /// <param name="author">The author to be saved in RowKey.</param>
        private void SetRowKey(string title)
        {
            RowKey = string.Format("{0}_{1}",
                    RowKey?.Substring(0, 19) ?? (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    title);
        }
        public BlogPostEntity Clone()
        {
            return new BlogPostEntity() { 
                PartitionKey = PartitionKey, 
                RowKey = RowKey, 
                Timestamp = Timestamp,
                ETag = ETag,
                Author = Author,
                Text = Text
            };
        }
    }
}
