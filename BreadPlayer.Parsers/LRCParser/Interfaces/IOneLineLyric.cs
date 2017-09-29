using System;

namespace BreadPlayer.Parsers.LRCParser
{
    public interface IOneLineLyric
    {
        bool IsActive { get; set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        TimeSpan Timestamp { get; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        string Content { get; }
    }
}