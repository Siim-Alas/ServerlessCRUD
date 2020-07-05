using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
        /// <param name="author">The author of the post, saved in RowKey (REVERSETICKS_AUTHOROID_TITLE).</param>
        /// <param name="title">The title of the post, saved in the RowKey (REVERSETICKS_AUTHOROID_TITLE).</param>
        /// <param name="text">The text of the post.</param>
        public BlogPostEntity(ClaimsPrincipal author, string title, string text)
        {
            PartitionKey = $"{9999 - DateTime.UtcNow.Year}{99 - DateTime.UtcNow.Month}";
            SetRowKey(author.Claims.Where(claim => claim.Type == "oid").First().Value, title);
            Author = author.Identity.Name;
            Text = text;
        }
        /// <summary>
        /// Gets or sets the display name for the author of the post.
        /// </summary>
        [Required]
        public string Author { get; set; }
        /// <summary>
        /// The text of the blog post.
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the OID of the author (GUID used for the user object), saved in RowKey (REVERSETICKS_AUTHOROID_TITLE).
        /// </summary>
        [Required]
        [IgnoreProperty]
        [JsonIgnore]
        public string AuthorOID 
        {
            // There are 19 digits in REVERSETICKS and 1 underscore, so the (0-based) AUTHORID starts at 20.
            // A GUID is in the form xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx (32 hexa-decimal digits [0-9a-f] and 4 hyphens).
            get { return RowKey?.Substring(20, 36); }
            set { SetRowKey(value, Title); }
        }
        /// <summary>
        /// Gets or sets the title of the post, saved in the RowKey (REVERSETICKS_AUTHOROID_TITLE).
        /// </summary>
        [Required]
        [IgnoreProperty]
        [JsonIgnore]
        public string Title 
        { 
            // There are 19 + 36 digits and 2 underscores before, so the (0-based) title starts at 57.
            get { return RowKey?.Substring(57); }
            set { SetRowKey(AuthorOID, value); }
        }
        /// <summary>
        /// Checks if the PartitionKey and RowKey values are valid and that Timestamp, Author, and Text are not null.
        /// </summary>
        [IgnoreProperty]
        [JsonIgnore]
        public bool IsValid 
        { 
            get 
            {
                return 
                    Regex.IsMatch(PartitionKey, @"^[0-9]{6}$") && 
                    Regex.IsMatch(RowKey, @"^[0-9]{19}_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}_.+$") && 
                    (Timestamp != null) &&
                    (Author != null) &&
                    (Text != null);
            }
        }
        /// <summary>
        /// Sets the RowKey in the format (REVERSETICKS_AUTHOROID_TITLE).
        /// </summary>
        /// <param name="authorOID">The oid of the author (GUID of the user object), to be saved in RowKey.</param>
        /// <param name="title">The title to be saved in RowKey.</param>
        private void SetRowKey(string authorOID, string title)
        {
            RowKey = string.Format("{0}_{1}_{2}",
                    RowKey?.Substring(0, 19) ?? (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19"),
                    RowKey?.Substring(20, 36) ?? authorOID,
                    title);
        }
    }
}
