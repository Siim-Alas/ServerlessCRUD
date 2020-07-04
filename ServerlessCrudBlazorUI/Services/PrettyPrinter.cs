using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public static class PrettyPrinter
    {
        #region Constants
        /// <summary>
        /// Represents different .md formatting options supported.
        /// </summary>
        private enum MdEnum
        {
            Italic,
            Bold,
            Heading3,
            Heading4,
            Link,
            Image,
            BlockQuote,
            // Unordered lists not supported.
            // Ordered lists not supported.
            HorizontalRule,
            Code
        }
        /// <summary>
        /// Provides the markdown option for each supported opening syntax.
        /// </summary>
        private static readonly Dictionary<string, MdEnum> openingMdStrings = 
            new Dictionary<string, MdEnum>() 
            {
                { "*", MdEnum.Italic },
                { "**", MdEnum.Bold }, 
                { "# ", MdEnum.Heading3 }, 
                { "## ", MdEnum.Heading4 },
                { "](", MdEnum.Link },
                { "![Image](", MdEnum.Image }, 
                { "> ", MdEnum.BlockQuote },
                // Unordered lists not supported.
                // Ordered lists not supported.
                { "---", MdEnum.HorizontalRule },
                { "`", MdEnum.Code }
            };
        /// <summary>
        /// Provides the closing syntax for each supported markdown option.
        /// </summary>
        private static readonly Dictionary<MdEnum, string> closingMdStrings =
            new Dictionary<MdEnum, string>()
            {
                { MdEnum.Italic, "*" },
                { MdEnum.Bold, "**" },
                { MdEnum.Heading3, "\n" },
                { MdEnum.Heading4, "\n" },
                { MdEnum.Link, ")" },
                { MdEnum.Image, ")" },
                { MdEnum.BlockQuote, "\n" },
                // Unordered lists not supported.
                // Ordered lists not supported.
                { MdEnum.HorizontalRule, "\n" },
                { MdEnum.Code, "`" }
            };
        /// <summary>
        /// Provices the opening and closing tags for each supported tag type.
        /// </summary>
        private static readonly Dictionary<MdEnum, (string opening, string closing)> htmlTags =
            new Dictionary<MdEnum, (string, string)>()
            {
                { MdEnum.Italic, ("<i>", "</i>") },
                { MdEnum.Bold, ("<b>", "</b>") },
                { MdEnum.Heading3, ("<h3>", "</h3>") },
                { MdEnum.Heading4, ("<h4>", "</h4>") },
                { MdEnum.Link, ("<a target=\"_blank\" href=\"", "</a>") },
                { MdEnum.Image, ("<img src=\"", "\"/>") },
                { MdEnum.BlockQuote, ("<blockquote class=\"blockquote\">", "</blockquote>") },
                // Unordered lists not supported.
                // Ordered lists not supported.
                { MdEnum.HorizontalRule, ("<hr />", "") },
                { MdEnum.Code, ("<code>", "</code>") }
            };
        #endregion

        
        /// <summary>
        /// Converts the 6-digit PartitionKey into a Date string in the format "yyyy_MM".
        /// </summary>
        /// <param name="partitionKey">The 6-digit PartitionKey.</param>
        /// <returns>The Date string in the format "yyyy_MM".</returns>
        public static string DateStringFromBlogPostPartitionKey(string partitionKey)
        {
            return $"{9999 - Convert.ToInt32(partitionKey.Substring(0, 4)):d4}_{99 - Convert.ToInt32(partitionKey.Substring(4, 2)):d2}";
        }
        /// <summary>
        /// Converts the Date string (in the format "yyyy_MM") into the corresponding 6-digit PartitionKey.
        /// </summary>
        /// <param name="dateString">The Date string in the format "yyyy_MM".</param>
        /// <returns>The 6-digit PartitionKey.</returns>
        public static string BlogPostPartitionKeyFromDateString(string dateString)
        {
            return $"{9999 - Convert.ToInt32(dateString.Substring(0, 4)):d4}{99 - Convert.ToInt32(dateString.Substring(5, 2)):d2}";
        }
        /// <summary>
        /// Parses a markdown (all listed <see href="https://commonmark.org/help/">here</see> under "Type") string into raw HTML.
        /// </summary>
        /// <param name="markdown">The markdown string.</param>
        /// <returns>The raw HTML from said markdown string.</returns>
        public static MarkupString ParseMarkdownToHTML(string markdown)
        {
            Stack<MdEnum> mdStack = new Stack<MdEnum>();
            StringBuilder rawHTMLBuilder = new StringBuilder();

            int i = 0;
            int j;
            int squareBracketIndex = -1;
            string currentSyntax;
            string linkName = "";
            StringBuilder syntaxBuilder = new StringBuilder();
            IEnumerable<string> candidateTags;

            HashSet<char> openingChars = new HashSet<char>();
            HashSet<char> closingChars = new HashSet<char>();

            foreach (string key in openingMdStrings.Keys)
            {
                openingChars.Add(key[0]);
            }
            foreach (string value in closingMdStrings.Values)
            {
                closingChars.Add(value[0]);
            }

            while (i < markdown.Length)
            {
                if (closingChars.Contains(markdown[i]) && mdStack.Count > 0)
                {
                    Console.WriteLine("true");
                    // Check if the current tag is being closed.
                    currentSyntax = closingMdStrings[mdStack.Peek()];
                    if (markdown.Substring(i, currentSyntax.Length) == currentSyntax)
                    {
                        // Links are a special case since they have 2 arguments instead of 1.
                        if (mdStack.Peek() == MdEnum.Link)
                        {
                            rawHTMLBuilder.Append($"\">{linkName}");
                        }
                        // The current tag is being closed.
                        rawHTMLBuilder.Append(htmlTags[mdStack.Pop()].closing);
                        i += currentSyntax.Length;
                        continue;
                    }
                }
                if (openingChars.Contains(markdown[i]))
                {
                    // Check if a new tag is being opened.
                    j = 0;
                    candidateTags = openingMdStrings.Keys.Where(k => k[0] == markdown[i]);
                    do
                    {
                        j++;
                        candidateTags = candidateTags
                            .Where(k => j < k.Length)
                            .Where(k => k[j] == markdown[i + j]);
                    } while (candidateTags.Count() > 0);

                    if (openingMdStrings.TryGetValue(markdown.Substring(i, j), out MdEnum tag))
                    {
                        // Links are a special case since they have 2 arguments instead of 1.
                        if (tag == MdEnum.Link)
                        {
                            // Check for false positives.
                            if (squareBracketIndex == -1)
                            {
                                continue;
                            }

                            linkName = markdown.Substring(squareBracketIndex + 1, i - squareBracketIndex - 1);
                            rawHTMLBuilder.Remove(rawHTMLBuilder.Length - linkName.Length - 1, linkName.Length + 1);
                        }
                        
                        // A new tag is being opened.
                        mdStack.Push(tag);
                        rawHTMLBuilder.Append(htmlTags[tag].opening);
                        i += j;
                        continue;
                    }
                }
                // Special for links
                if (markdown[i] == '[')
                {
                    squareBracketIndex = i;
                }

                rawHTMLBuilder.Append(markdown[i]);
                i++;
            }

            return (MarkupString)rawHTMLBuilder.ToString();
        }
    }
}
