using System;

namespace BreadPlayer.Parsers.LRCParser
{
    public class OneLineLyric : ObservableObject, IOneLineLyric
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneLineLyric"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="content">The content.</param>
        public OneLineLyric(TimeSpan timestamp, string content)
        {
            Timestamp = timestamp;
            Content = content;
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public TimeSpan Timestamp { get; internal set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; internal set; }

        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            set => Set(ref isActive, value);
        }
    }
}