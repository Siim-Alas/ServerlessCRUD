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
        /// <param name="title">The title of the post, saved in the RowKey (REVERSETICKS_TITLE_AUTHOR).</param>
        /// <param name="author">The author of the post, saved in RowKey (REVERSETICKS_TITLE_AUTHOR).</param>
        /// <param name="text">The text of the post.</param>
        public BlogPostEntity(string title, string author, string text = "")
        {
            PartitionKey = DateTime.UtcNow.ToString("yyyy_MM");
            SetRowKey(title, author);
            Text = text;
        }
        /// <summary>
        /// The text of the blog post.
        /// </summary>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the title of the post, saved in the RowKey (REVERSETICKS_TITLE_AUTHOR).
        /// </summary>
        [Required]
        [IgnoreProperty]
        [JsonIgnore]
        public string Title 
        { 
            get { return RowKey.Substring(RowKey.IndexOf('_') + 1, RowKey.LastIndexOf('_') - RowKey.IndexOf('_') - 1); } 
            set { SetRowKey(value, Author); }
        }
        /// <summary>
        /// Gets or sets the author of the post, saved in the RowKey (REVERSETICKS_TITLE_AUTHOR).
        /// </summary>
        [Required]
        [IgnoreProperty]
        [JsonIgnore]
        public string Author
        {
            get { return RowKey.Substring(RowKey.LastIndexOf('_') + 1); }
            set { SetRowKey(Title, value); }
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
                    Regex.IsMatch(PartitionKey, @"^[0-9]{4}_[0-9]{2}$") && 
                    Regex.IsMatch(RowKey, @"^[0-9]{19}_.+_.+$") && 
                    Timestamp != null &&
                    Text != null;
            }
        }
        /// <summary>
        /// Sets the RowKey in the format (REVERSETICKS_TITLE_AUTHOR).
        /// </summary>
        /// <param name="title">The title to be saved in RowKey.</param>
        /// <param name="author">The author to be saved in RowKey.</param>
        private void SetRowKey(string title, string author)
        {
            RowKey = string.Format("{0}_{1}_{2}",
                    (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    title,
                    author);
        }
        public BlogPostEntity Clone()
        {
            return new BlogPostEntity() { 
                PartitionKey = PartitionKey, 
                RowKey = RowKey, 
                Timestamp = Timestamp,
                ETag = ETag,
                Text = Text
            };
        }
    }
}
