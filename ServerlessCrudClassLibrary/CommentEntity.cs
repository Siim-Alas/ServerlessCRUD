using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
        public CommentEntity(BlogPostEntity blogPost, ClaimsPrincipal author, string text)
        {
            PartitionKey = $"{blogPost.PartitionKey}_{blogPost.RowKey}";
            SetRowKey(
                author.FindFirst("oid").Value, 
                author.Identity.Name);
            Text = text;
        }
        /// <summary>
        /// The text of the comment.
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Gets the OID of the author (GUID used for the user object), saved in RowKey (REVERSETICKS_AUTHOROID_AUTHORNAME).
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public string AuthorOID
        {
            // There are 19 digits in REVERSETICKS and 1 underscore, so the (0-based) AUTHORID starts at 20.
            // A GUID is in the form xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx (32 hexa-decimal digits [0-9a-f] and 4 hyphens).
            get { return RowKey.Substring(20, 36); }
        }
        /// <summary>
        /// Gets the display name for the author of the post, saved in RowKey (REVERSETICKS_AUTHOROID_AUTHORNAME).
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public string Author 
        {
            // There are 19 + 36 digits and 2 underscores before, so the (0-based) author starts at 57.
            get { return RowKey.Substring(57); } 
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
                    Regex.IsMatch(RowKey, @"^[0-9]{19}_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}_.+$") &&
                    (Timestamp != null) &&
                    (Text != null);
            }
        }
        /// <summary>
        /// Sets the RowKey of the comment in the format (REVERSETICKS_AUTHOROID_AUTHORNAME).
        /// </summary>
        /// <param name="authorOID">The oid of the author (GUID of the user object), to be saved in RowKey.</param>
        /// <param name="authorName">The display name of the author, to be saved in RowKey.</param>
        private void SetRowKey(string authorOID, string authorName)
        {
            RowKey = string.Format("{0}_{1}_{2}",
                    RowKey?.Substring(0, 19) ?? (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    authorOID,
                    authorName);
        }
    }
}
