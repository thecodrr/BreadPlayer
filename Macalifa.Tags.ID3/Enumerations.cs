/* 
	Macalifa. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Indicates diffrent types of text encoding use for texts
    /// </summary>
    public enum TextEncodings
    {
        /// <summary>
        /// Use Asccii as text encoding
        /// </summary>
        Ascii = 0,
        /// <summary>
        /// Use UTF16 (little endian) as text encoding
        /// </summary>
        UTF_16 = 1,
        /// <summary>
        /// Use UTF16 (big endian) as text encoding
        /// </summary>
        UTF_16BE = 2,
        /// <summary>
        /// Use UTF8 as text encoding
        /// </summary>
        UTF8 = 3
    }

    /// <summary>
    /// Provide frame flags for all frames
    /// </summary>
    [Flags]
    public enum FrameFlags
    {
        /// <summary>
        /// Tag Alter if not valid
        /// </summary>
        TagAlterPreservation = 0x8000,
        /// <summary>
        /// File Alter if not valid
        /// </summary>
        FileAlterPreservation = 0x4000,
        /// <summary>
        /// Readonly
        /// </summary>
        ReadOnly = 0x2000,
        /// <summary>
        /// Compressed
        /// </summary>
        Compression = 0x0080,
        /// <summary>
        /// Encrypted
        /// </summary>
        Encryption = 0x0040,
        /// <summary>
        /// Grouped by another tags
        /// </summary>
        GroupingIdentity = 0x0020
    }

    /// <summary>
    /// Specifies types of time stamps
    /// </summary>
    public enum TimeStamps
    {
        /// <summary>
        /// Use mpeg frame as timestamp
        /// </summary>
        MpegFrame = 1,
        /// <summary>
        /// using millisecond as timestamp
        /// </summary>
        Milliseconds
    }

    /// <summary>
    /// Specifies incrementing or dcrementing must use for relative volume
    /// </summary>
    public enum IncrementDecrement
    {
        /// <summary>
        /// To decrement value
        /// </summary>
        Dcrement = 0,
        /// <summary>
        /// To increment value
        /// </summary>
        Increment
    }

    /// <summary>
    /// Enum for ID3v2 header flags
    /// </summary>
    [Flags]
    public enum ID3v2HeaderFlags
    {
        /// <summary>
        /// Indicates whether or not unsynchronisation is used 
        /// </summary>
        Unsynchronisation = 128,
        /// <summary>
        /// Indicate ID3v2 contains exnted header data
        /// </summary>
        ExtendedHeader = 64,
        /// <summary>
        /// Means ID3v2 tag is experimental
        /// </summary>
        Experimental = 32
    }

    /// <summary>
    /// Diffrent types of frame filter usage
    /// </summary>
    public enum FilterTypes
    {
        /// <summary>
        /// No filter
        /// </summary>
        NoFilter = 0,
        /// <summary>
        /// Load filtered frames only
        /// </summary>
        LoadFiltersOnly = 1,
        /// <summary>
        /// Do not load filtered frames
        /// </summary>
        NotLoadFilters = 2
    }

    /// <summary>
    /// Indicates diffrent versions of ID3
    /// </summary>
    public enum ID3Versions
    {
        /// <summary>
        /// ID3 version 1
        /// </summary>
        ID3v1,
        /// <summary>
        /// ID3 version 2
        /// </summary>
        ID3v2
    }
}
