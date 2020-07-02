using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary.Utilities
{
    public static class PrettyPrinter
    {
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
    }
}
