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
using System.IO;
using System.Collections;
using Macalifa.Tags.ID3.ID3v2Frames;
using System.Text.RegularExpressions;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// The main class for any type of frame to inherit
    /// </summary>
    public abstract class Frame : ILengthable
    {
        private string _FrameID; // Contain FrameID of current Frame
        private FrameFlags _FrameFlags; // Contain Flags of current frame
        // After reading frame if must drop value were true it means frame is not readable
        private bool _IsLinked; // indicate is current frame a linked frame or not
        private ID3Exception _Exception;
        Macalifa.Tags.TagStreamUWP Tag;
        /// <summary>
        /// Create a new Frame class
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">Frame Falgs</param>
        protected Frame(string FrameID, FrameFlags Flags, Stream FS)
        {
            // All FrameID letters must be capital
            FrameID = FrameID.ToUpper();

            if (!ValidatingFrameID(FrameID, ExceptionLevels.Error))
                return;
            Tag = new Macalifa.Tags.TagStreamUWP(FS);
            _FrameFlags = Flags;
            _FrameID = FrameID;
            _IsLinked = false;
        }

        /// <summary>
        /// Get or Set flags of current frame
        /// </summary>
        protected FrameFlags FrameFlag
        {
            get
            { return _FrameFlags; }
            set
            { _FrameFlags = value; }
        }

        /// <summary>
        /// Exception Occured while reading file
        /// </summary>
        /// <param name="Ex">Exception to add to list</param>
        protected void ExceptionOccured(ID3Exception Ex)
        {
            _Exception = Ex;
        }

        #region -> Static Get Members <-

        /// <summary>
        /// Get length of Specific string according to Encoding
        /// </summary>
        /// <param name="Text">Text to get length</param>
        /// <param name="TEncoding">TextEncoding to use for Length calculation</param>
        /// <param name="AddNullCharacter">Indicate Add null character at the end of string or not</param>
        /// <returns>Length of text</returns>
        protected static int GetTextLength(string Text, TextEncodings TEncoding, bool AddNullCharacter)
        {
            int StringLength;

            StringLength = Text.Length;
            if (TEncoding == TextEncodings.UTF_16 || TEncoding == TextEncodings.UTF_16BE)
                StringLength *= 2; // in UTF-16 each character is 2 bytes

            if (AddNullCharacter)
            {
                if (TEncoding == TextEncodings.UTF_16 || TEncoding == TextEncodings.UTF_16BE)
                    StringLength += 2;
                else
                    StringLength++;
            }

            return StringLength;
        }

        #endregion

        #region -> Validating Methods <-

        /// <summary>
        /// Indicate is value of Enumeration valid for that enum
        /// </summary>
        /// <param name="Enumeration">Enumeration to control value for</param>
        /// <param name="ErrorType">if not valid how error occur</param>
        /// <param name="FrameID">FrameID just for using in Exception message</param>
        /// <returns>true if valid otherwise false</returns>
        protected bool IsValidEnumValue(Enum Enumeration, ExceptionLevels ErrorType, string FrameID)
        {
            if (IsValidEnumValue(Enumeration))
                return true;
            else
            {
                ID3Exception FEx = new ID3Exception(Enumeration.ToString() +
                        " is out of range of " + Enumeration.GetType().ToString(), FrameID, ErrorType);
                if (ErrorType != ExceptionLevels.Error)
                    ExceptionOccured(FEx);
                //else
                    //throw FEx;

                return false;
            }
        }

        /// <summary>
        /// Indicate if value of enumeration is valid
        /// </summary>
        /// <param name="Enumeration">Enum to check</param>
        /// <returns>true if valid otherwise false</returns>
        protected bool IsValidEnumValue(Enum Enumeration)
        {
            return (Enum.IsDefined(Enumeration.GetType(), Enumeration));                
        }

        /// <summary>
        /// Indicate is specific FrameID valid or not
        /// </summary>
        /// <param name="FrameIdentifier">FrameID to check</param>
        /// <param name="ErrorType">Error type that must occur if frame identifier was not valid</param>
        /// <returns>true if frame identifer was valid otherwise false</returns>
        protected bool ValidatingFrameID(string FrameIdentifier, ExceptionLevels ErrorType)
        {
            bool IsValid = FramesInfo.IsValidFrameID(FrameIdentifier);

            if (!IsValid)
            {
                ID3Exception Ex = new ID3Exception(FrameIdentifier + " is not valid FrameID", FrameIdentifier, ErrorType);
                if (ErrorType == ExceptionLevels.Error)
                    throw Ex;
                else if (ErrorType == ExceptionLevels.Error)
                    ExceptionOccured(Ex);
            }

            return IsValid;
        }

        #endregion

        /// <summary>
        /// Indicate if current frame isvalid according to ExceptionLevel and validating data
        /// </summary>
        public bool IsValid
        {
            get
            {
                try
                {
                    if (_Exception == null)
                        return this.OnValidating();
                    else
                        return (_Exception.Level == ExceptionLevels.Error) ||
                            this.OnValidating();
                }
                catch{ return false; }
                
            }
        }

        /// <summary>
        /// Write data of current frame to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data to</param>
        /// <param name="MinorVersion">MinorVersion of ID3</param>
        public void WriteData(int MinorVersion)
        {
            WriteFrameHeader(MinorVersion);

            OnWritingData(MinorVersion);
        }

        private void WriteFrameHeader(int MinorVersion)
        {
            byte[] Buf;
            int FrameIDLength = MinorVersion == 2 ? 3 : 4; // Length of FrameID according to version
            string Temp = _FrameID;

            // if minor version of ID3 were 2, the frameID is 3 character length
            if (MinorVersion == 2)
            {
                Temp = FramesInfo.Get3CharID(Temp);
                if (Temp == null) // This frame is not availabe in this version
                    return;
            }

            Tag.WriteText(Temp, TextEncodings.Ascii, false); // Write FrameID
            Buf = BitConverter.GetBytes(Length);
            Array.Reverse(Buf);
            if (MinorVersion == 2)
                Tag.FS.Write(Buf, 1, Buf.Length - 1); // Write Frame Size
            else
                Tag.FS.Write(Buf, 0, Buf.Length); // Write Frame Size

            if (MinorVersion != 2)
            {
                // If newer than version 2 it have Flags
                Buf = BitConverter.GetBytes((ushort)_FrameFlags);
                Array.Reverse(Buf);
                Tag.FS.Write(Buf, 0, Buf.Length); // Write Frame Flag
            }
        }

        #region -> Virtual method and properties <-

        /// <summary>
        /// Call when frame need to write it's data to stream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected virtual void OnWritingData(int MinorVersion)
        { }

        /// <summary>
        /// Occur when want to validate frame information
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected virtual bool OnValidating()
        {
            return true;
        }

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected virtual int OnGetLength()
        { return 0; }

        #endregion

        #region -> Frame Flags Properties <-

        /// <summary>
        /// Get FrameID of current frame
        /// </summary>
        public string FrameID
        {
            get
            { return _FrameID; }
        }

        /// <summary>
        /// Gets or sets if current frame is ReadOnly
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                if ((_FrameFlags & FrameFlags.ReadOnly)
                    == FrameFlags.ReadOnly)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.ReadOnly;
                else
                    _FrameFlags &= ~FrameFlags.ReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets if current frame is Encrypted
        /// </summary>
        public bool Encryption
        {
            get
            {
                if ((_FrameFlags & FrameFlags.Encryption)
                    == FrameFlags.Encryption)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.Encryption;
                else
                    _FrameFlags &= ~FrameFlags.Encryption;
            }
        }

        /// <summary>
        /// Gets or sets whether or not frame belongs in a group with other frames
        /// </summary>
        public bool GroupIdentity
        {
            get
            {
                if ((_FrameFlags & FrameFlags.GroupingIdentity)
                    == FrameFlags.GroupingIdentity)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.GroupingIdentity;
                else
                    _FrameFlags &= ~FrameFlags.GroupingIdentity;
            }
        }

        /// <summary>
        /// Gets or sets whether or not this frame was compressed
        /// </summary>
        public bool Compression
        {
            get
            {
                if ((_FrameFlags & FrameFlags.Compression)
                   == FrameFlags.Compression)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.Compression;
                else
                    _FrameFlags &= ~FrameFlags.Compression;
            }
        }

        /// <summary>
        /// Gets or sets if it's unknown frame it should be preserved or discared
        /// </summary>
        public bool TagAlterPreservation
        {
            get
            {
                if ((_FrameFlags & FrameFlags.TagAlterPreservation)
                   == FrameFlags.TagAlterPreservation)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.TagAlterPreservation;
                else
                    _FrameFlags &= ~FrameFlags.TagAlterPreservation;
            }
        }

        /// <summary>
        /// Gets or sets what to do if file excluding frame, Preseved or discared
        /// </summary>
        public bool FileAlterPreservation
        {
            get
            {
                if ((_FrameFlags & FrameFlags.FileAlterPreservation)
                   == FrameFlags.FileAlterPreservation)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    _FrameFlags |= FrameFlags.FileAlterPreservation;
                else
                    _FrameFlags &= ~FrameFlags.FileAlterPreservation;
            }
        }

        /// <summary>
        /// Gets or sets is current frame a linked frame
        /// </summary>
        public bool IsLinked
        {
            get
            { return _IsLinked; }
            set
            { _IsLinked = value; }
        }

        #endregion

        /// <summary>
        /// Retrun a string that represent FrameID of current Frame
        /// </summary>
        /// <returns>FrameID of current Frame</returns>
        public override string ToString()
        {
            return _FrameID;
        }

        /// <summary>
        /// Exception that occured in reading frame
        /// </summary>
        public ID3Exception Exception
        {
            get
            { return _Exception; }
        }

        /// <summary>
        /// Get Length of current frame
        /// </summary>
        public int Length
        {
            get
            { return OnGetLength(); }
        }
    }
}