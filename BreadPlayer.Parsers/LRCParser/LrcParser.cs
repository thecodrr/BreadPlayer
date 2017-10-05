using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BreadPlayer.Parsers.LRCParser
{
    public class LrcParser : ILrcParser
    {
        private readonly List<IOneLineLyric> _lyrics;

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public ILrcMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <value>
        /// The lyrics.
        /// </value>
        public List<IOneLineLyric> Lyrics { get { return _lyrics; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LrcFile"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="lyrics">The lyrics.</param>
        /// <param name="applyOffset">if set to <c>true</c> apply the offset in metadata, otherwise ignore the offset.</param>
        public LrcParser(ILrcMetadata metadata, IEnumerable<IOneLineLyric> lyrics, bool applyOffset)
        {
            if (lyrics == null) throw new ArgumentNullException("lyrics");
            Metadata = metadata ?? throw new ArgumentNullException("metadata");
            var array = lyrics.OrderBy(l => l.Timestamp).ToList();
            for (var i = 0; i < array.Count; ++i)
            {
                if (applyOffset && metadata.Offset.HasValue)
                {
                    if (array[i] is OneLineLyric oneLineLyric)
                    {
                        oneLineLyric.Timestamp -= metadata.Offset.Value;
                    }
                    else
                    {
                        array[i] = new OneLineLyric(array[i].Timestamp - metadata.Offset.Value, array[i].Content);
                    }
                }
                if (i > 0 && array[i].Timestamp == array[i - 1].Timestamp)
                {
                    //ignore.
                    //throw new FormatException(string.Format("Found duplicate timestamp '{0}' with lyric '{1}' and '{2}'.", array[i].Timestamp, array[i - 1].Content, array[i].Content));
                }
            }
            _lyrics = array;
        }

        private static readonly Regex TimestampRegex = new Regex(@"^(?'minutes'\d+):(?'seconds'\d+(\.\d+)?)$");
        private static readonly Regex MetadataRegex = new Regex(@"^(?'title'[A-Za-z]+?):(?'content'.*)$");

        /// <summary>
        /// Checks if the text is a valid LRC
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsLrc(string text)
        {
            return !string.IsNullOrEmpty(text) ? Regex.Match(text, @"\[\d+:\d+.\d+\]|\[[a-zA-Z]+:[a-zA-Z\s]+\]").Success : false;
        }

        /// <summary>
        /// Create a new new instance of the <see cref="ILrcFile"/> interface with the specified LRC text.
        /// </summary>
        /// <param name="lrcText">The LRC text.</param>
        /// <returns></returns>
        public static ILrcParser FromText(string lrcText)
        {
            if (lrcText == null) throw new ArgumentNullException("lrcText");

            var pairs = new List<KeyValuePair<string, string>>();
            var titles = new List<string>();
            var sb = new StringBuilder();

            // 0: Line start. Expect line ending or [.
            // 1: Reading title. Expect ] or all characters except line ending.
            // 2: Reading content. Expect line ending or [ or other charactors.
            var state = 0;
            for (var i = 0; i <= lrcText.Length; ++i)
            {
                var ended = i >= lrcText.Length;
                var ch = ended ? (char)0 : lrcText[i];
                // ReSharper disable once IdentifierTypo
                var unescaped = false;
                if (ch == '\\')
                {
                    ++i;
                    ended = i >= lrcText.Length;
                    if (ended)
                    {
                        throw new FormatException("Expect one charactor after '\\' but reaches the end.");
                    }
                    ch = lrcText[i];
                    unescaped = true;
                }

                switch (state)
                {
                    case 0:
                        if (!unescaped && ch == '[')
                        {
                            state = 1;
                        }
                        else if (!unescaped && (ch == '\r' || ch == '\n') || ended)
                        {
                            state = 0;
                        }
                        else
                        {
                            throw new FormatException(string.Format("Expect '[' at position {0}", i));
                        }
                        break;

                    case 1:
                        if (!unescaped && ch == ']')
                        {
                            state = 2;
                            titles.Add(sb.ToString());
                            sb.Clear();
                        }
                        else if (!unescaped && (ch == '\r' || ch == '\n') || ended)
                        {
                            throw new FormatException(string.Format("Expect ']' at position {0}", i));
                        }
                        else
                        {
                            sb.Append(ch); // append to title
                        }
                        break;

                    case 2:
                        if (!unescaped && (ch == '\r' || ch == '\n') || ended)
                        {
                            state = 0;
                            var content = sb.ToString();
                            pairs.AddRange(titles.Select(t => new KeyValuePair<string, string>(t, content)));
                            sb.Clear();
                            titles.Clear();
                        }
                        else if (!unescaped && ch == '[')
                        {
                            if (sb.Length > 0)
                            {
                                state = 1;
                                var content = sb.ToString();
                                pairs.AddRange(titles.Select(t => new KeyValuePair<string, string>(t, content)));
                                sb.Clear();
                                titles.Clear();
                            }
                            else
                            {
                                state = 1;
                            }
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }

            var lyrics = new List<IOneLineLyric>();
            var metadata = new LrcMetadata();
            string offsetString = null;
            foreach (var pair in pairs)
            {
                // Parse timestamp
                var match = TimestampRegex.Match(pair.Key);
                if (match.Success)
                {
                    if(double.TryParse(match.Groups["seconds"].Value, out double seconds)
                       && int.TryParse(match.Groups["minutes"].Value, out int minutes))
                    {
                        var timestamp = TimeSpan.FromSeconds(minutes * 60 + seconds);
                        lyrics.Add(new OneLineLyric(timestamp, pair.Value));
                        continue;
                    }                    
                }

                // Parse metadata
                match = MetadataRegex.Match(pair.Key);
                if (match.Success)
                {
                    var title = match.Groups["title"].Value.ToLower();
                    var content = match.Groups["content"].Value;
                    if (title == "ti")
                    {
                        if (metadata.Title != null && content != metadata.Title)
                        {
                            throw new FormatException(string.Format("Duplicate LRC metadata found. Metadata name: '{0}', Values: '{1}', '{2}'", "ti", metadata.Title, content));
                        }
                        metadata.Title = content;
                    }
                    else if (title == "ar")
                    {
                        if (metadata.Artist != null && content != metadata.Artist)
                        {
                            throw new FormatException(string.Format("Duplicate LRC metadata found. Metadata name: '{0}', Values: '{1}', '{2}'", "ar", metadata.Artist, content));
                        }
                        metadata.Artist = content;
                    }
                    else if (title == "al")
                    {
                        if (metadata.Album != null && content != metadata.Album)
                        {
                            throw new FormatException(string.Format("Duplicate LRC metadata found. Metadata name: '{0}', Values: '{1}', '{2}'", "al", metadata.Album, content));
                        }
                        metadata.Album = content;
                    }
                    else if (title == "by")
                    {
                        if (metadata.Maker == null && content == metadata.Maker)
                        {
                            metadata.Maker = content;
                        }
                    }
                    else if (title == "offset")
                    {
                        if (offsetString != null && content != offsetString)
                        {
                            throw new FormatException(string.Format("Duplicate LRC metadata found. Metadata name: '{0}', Values: '{1}', '{2}'", "offset", offsetString, content));
                        }
                        offsetString = content;
                        if(double.TryParse(content, out double offset))
                        {
                            metadata.Offset = TimeSpan.FromMilliseconds(offset);
                        }
                    }
                    // ReSharper disable once RedundantIfElseBlock
                    else
                    {
                        // Ignore unsupported tag
                    }
                }
            }

            if (lyrics.Count == 0)
            {
                throw new FormatException("Invalid or empty LRC text. Can't find any lyrics.");
            }
            return new LrcParser(metadata, lyrics, true);
        }
    }
}