using System;

namespace BreadPlayer.Parsers.LRCParser
{
    public interface ILrcMetadata
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the artist.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        string Artist { get; }

        /// <summary>
        /// Gets the album.
        /// </summary>
        /// <value>
        /// The album.
        /// </value>
        string Album { get; }

        /// <summary>
        /// Gets the lyrics maker.
        /// </summary>
        /// <value>
        /// The lyrics maker.
        /// </value>
        string Maker { get; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        TimeSpan? Offset { get; }
    }
}