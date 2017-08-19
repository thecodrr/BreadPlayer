using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Parsers.LRCParser
{
    public interface ILrcParser
    {
       
        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        ILrcMetadata Metadata { get; }
        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <value>
        /// The lyrics.
        /// </value>
        List<IOneLineLyric> Lyrics { get; }
    }
}
