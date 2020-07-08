using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ServerlessCrudClassLibrary
{
    public class CommentEntity : TableEntity
    {
        public CommentEntity() : base()
        {

        }
        /// <summary>
        /// Creates a new comment.
        /// </summary>
        /// <param name="blogPost">The blog post to which the comment is written.</param>
        /// <param name="author">The author of the comment, used for setting RowKey.</param>
        /// <param name="text">The text of the comment.</param>
        public CommentEntity(BlogPostEntity blogPost, string author, string text)
        {
            PartitionKey = $"{blogPost.PartitionKey}_{blogPost.RowKey}";
            SetRowKey(author);
            Text = text;
        }
        /// <summary>
        /// The text of the comment.
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Gets the display name for the author of the post, saved in RowKey (REVERSETICKS_AUTHORNAME).
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public string Author 
        {
            // There are 19 digits and 1 underscore, so the (0-based) name starts at 20.
            get { return RowKey.Substring(20); } 
        }
        /// <summary>
        /// Checks that PartitionKey and RowKey are valid and that Timestamp and Text are not null.
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return 
                    Regex.IsMatch(PartitionKey, @"^[0-9]{6}_[0-9]{19}_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}_.+$") &&
                    Regex.IsMatch(RowKey, @"^[0-9]{19}_.+$") &&
                    (Timestamp != null) &&
                    (Text != null);
            }
        }
        /// <summary>
        /// Sets the RowKey of the comment in the format (REVERSETICKS_AUTHORNAME).
        /// </summary>
        /// <param name="authorName">The display name of the author, to be saved in RowKey.</param>
        private void SetRowKey(string authorName)
        {
            RowKey = string.Format("{0}_{1}",
                    RowKey?.Substring(0, 19) ?? (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    authorName);
        }
    }
}
