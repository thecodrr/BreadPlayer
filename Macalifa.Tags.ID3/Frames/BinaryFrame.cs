/* 
	BreadPlayer. A music player made for Windows 10 store.
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
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
/*
 * This namespace contain frames that have binary information
 * like: pictures, files and etc
 * for storing Binary information in all classes i have used MemoryStream
 */
namespace BreadPlayer.Tags.ID3.ID3v2Frames.BinaryFrames
{
	/// <summary>
	/// A class for frame that only include Data(binary)
	/// </summary>
	public class BinaryFrame : Frame
    {
        /// <summary>
        /// Contains data of current frame
        /// </summary>
        protected MemoryStream _Data;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// New BinaryFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flag</param>
        /// <param name="Data">FileStream contain frame data</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public BinaryFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// New BinaryFrame from specific information
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">Data of BinaryFrame</param>
        public BinaryFrame(string FrameID, FrameFlags Flags, MemoryStream Data, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _Data = Data;
        }

        /// <summary>
        /// New BinaryFrame for inherited classes
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        protected BinaryFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// Get or Set Data of current frame
        /// </summary>
        public MemoryStream Data
        {
            get
            {
                if (_Data == null)
                    return null;

                // Go to begining of stream
                _Data.Seek(0, SeekOrigin.Begin);

                return _Data;
            }
            set
            {
                if (value == null)
                    throw (new ArgumentNullException("Data can't set to null"));

                if (FrameID == "MCDI" && value.Length > 804)
                    throw (new ArgumentException("Music CD Identifier(MCDI) length must be equal or less than 804 byte"));

                _Data = value;
            }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            return Convert.ToInt32(_Data.Length);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            
            Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_Data == null)
                return false;

            if (_Data.Length == 0)
                return false;

            return true;
        }

        #endregion
    }

    /// <summary>
    // A class for frame that include Data, Owner
    /// </summary>
    public class PrivateFrame : BinaryFrame
    {
        // Private Frames can repeat with same Owner Identifier in one tag \\

        /// <summary>
        /// Contains Owner name
        /// </summary>
        protected string _Owner;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// New PrivateFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">FileStream to read frame data from</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public PrivateFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _Owner = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _Data = TStream.ReadData(Length); // Read Data
        }

        /// <summary>
        /// New Private Frame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Owner">Owner of data</param>
        /// <param name="Data">Data</param>
        public PrivateFrame(string FrameID, FrameFlags Flags, string Owner,
            MemoryStream Data, Stream FS)
            : base(FrameID, Flags, FS)
        {
            if (FrameID != "UFID" && FrameID != "PRIV")
                throw (new ArgumentException("FrameID can only be UFID(Unique file Identifier) or PRIV(Private Frame)"));

            this.OwnerIdentifier = Owner;
            this.Data = Data;
        }

        /// <summary>
        /// Create new PrivateFrame for inherited classes
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        protected PrivateFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// Get/Set OwnerIdentifier of current frame in ascii encoding
        /// </summary>
        public string OwnerIdentifier
        {
            get
            { return _Owner; }
            set
            {
                if (value == null)
                    throw (new ArgumentNullException("Owner can't set to null"));

                _Owner = value;
            }
        }

        /// <summary>
        /// Get or Set Data of current frame
        /// </summary>
        public new MemoryStream Data
        {
            get
            { return _Data; }
            set
            {
                if (value == null)
                {
                    _Data = null;
                    return;
                }

                if (this.FrameID == "UFID" && value.Length > 64)
                    throw (new ArgumentException("For Unique File Identifier(UFID) the Data length must be less than 64 bytes"));

                _Data = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            return base.OnGetLength() + 1 + _Owner.Length;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(_Owner, TextEncodings.Ascii, true);
            _Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_Owner != "" || _Data.Length > 0)
                return true;
            else
                return false;
        }

        #endregion

        /// <summary>
        /// Convert current PrivateFrame to System.String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _Owner;
        }
    }

    /// <summary>
    /// A class for frame that include Data, Owner, Symbol
    /// </summary>
    public class DataWithSymbolFrame : PrivateFrame
    {
        private byte _Symbol;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// New DataWithSymbolFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">FileStream to read frame data from</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public DataWithSymbolFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _Owner = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _Symbol = TStream.ReadByte(FS);
            Length--;

            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// New DataWithSymbol from specific information
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Owner">Owner identifier</param>
        /// <param name="Symbol">Symbol of owner</param>
        /// <param name="Data">Data of frame</param>
        public DataWithSymbolFrame(string FrameID, FrameFlags Flags, string Owner,
            byte Symbol, MemoryStream Data, Stream FS)
            : base(FrameID, Flags, FS)
        {
            if (FrameID != "ENCR" && FrameID != "GRID")
                throw (new ArgumentException("FrameID must be ENCR(Encryption method) or GRID(Group Identification)"));

            _Symbol = Symbol;
        }

        /// <summary>
        /// Gets or Sets Symbol of current frame
        /// </summary>
        public byte Symbol
        {
            get
            { return _Symbol; }
            set
            { _Symbol = value; }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            return base.OnGetLength() + 1;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(_Owner, TextEncodings.Ascii, true);
            TStream.FS.WriteByte(_Symbol);
            Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            return base.OnValidating();
        }

        #endregion

        /// <summary>
        /// Indicate if specific object is equal to this frame
        /// </summary>
        /// <returns>True if equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DataWithSymbolFrame))
                return false;

            if (((DataWithSymbolFrame)obj).OwnerIdentifier == this.OwnerIdentifier &&
                ((DataWithSymbolFrame)obj).FrameID == this.FrameID)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get hashcode for current frame
        /// </summary>
        /// <returns>int contains hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// A class for frame that include Data, Owner, PreviewStart, PreviewLength
    /// </summary>
    public class AudioEncryptionFrame : PrivateFrame
    {
        private int _PreviewStart;
        private int _PreviewLength;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new AudioEncryptionFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public AudioEncryptionFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _Owner = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _PreviewStart = Convert.ToInt32(TStream.ReadUInt(2));
            _PreviewLength = Convert.ToInt32(TStream.ReadUInt(2));
            Length -= 4;

            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// Create new AudioEncryptionFrame
        /// </summary>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Owner">Owner identifier</param>
        /// <param name="PreviewStart">PreviewStart time</param>
        /// <param name="PreviewLength">PreviewLength time</param>
        /// <param name="Data">Data that this frame must contain</param>
        public AudioEncryptionFrame(FrameFlags Flags, string Owner,
            int PreviewStart, int PreviewLength, MemoryStream Data, Stream FS)
            : base("AENC", Flags, FS)
        {
            _PreviewStart = PreviewStart;
            _PreviewLength = PreviewLength;
            this.Data = Data;
            this.OwnerIdentifier = Owner;
        }

        /// <summary>
        /// Gets or Sets PreviewStart of current frame
        /// </summary>
        public int PreviewStart
        {
            get
            {
                return _PreviewStart;
            }
            set
            {
                if (value > 0xFFFF || value < 0)
                    throw (new ArgumentOutOfRangeException("Preview Start must be less than 65,535(0xFFFF) and minimum be zero"));

                _PreviewStart = value;
            }
        }

        /// <summary>
        /// Gets or Sets PreviewLength of current frame
        /// </summary>
        public int PreviewLength
        {
            get
            {
                return _PreviewLength;
            }
            set
            {
                if (value > 0xFFFF || value < 0)
                    throw (new ArgumentOutOfRangeException("Preview Length must be less than 65,535(0xFFFF) and minimum be zero"));

                _PreviewLength = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            //4: PreviewStart and PreviewLength Length
            return base.OnGetLength() + 4;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(_Owner, TextEncodings.Ascii, true);
            ushort temp;
            byte[] Buf;
            temp = Convert.ToUInt16(_PreviewStart);
            Buf = BitConverter.GetBytes(temp);
            TStream.FS.Write(Buf, 0, 2);

            temp = Convert.ToUInt16(_PreviewLength);
            Buf = BitConverter.GetBytes(temp);
            TStream.FS.Write(Buf, 0, 2);

            Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            return base.OnValidating();
        }

        #endregion

        /// <summary>
        /// Indicate if specific object is equal to this frame
        /// </summary>
        /// <returns>True if equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AudioEncryptionFrame))
                return false;

            if (((AudioEncryptionFrame)obj).OwnerIdentifier == this.OwnerIdentifier)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get hashcode for current frame
        /// </summary>
        /// <returns>int contains hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// A class for frame that include TextEncoding, Description, MIMEType, Data
    /// </summary>
    public abstract class BaseFileFrame : BinaryFrame
    {
        /// <summary>
        /// TextEncoding value
        /// </summary>
        protected TextEncodings _TextEncoding;
        /// <summary>
        /// MimeType value
        /// </summary>
        protected string _MIMEType;
        /// <summary>
        /// Description value
        /// </summary>
        protected string _Description;

        /// <summary>
        /// New BaseFileFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        protected BaseFileFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// New BaseFileFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Description">Description</param>
        /// <param name="MIMEType">MimeType of Data</param>
        /// <param name="TextEncoding">TextEncoding for texts</param>
        /// <param name="Data">Data of frame</param>
        protected BaseFileFrame(string FrameID, FrameFlags Flags, string Description,
            string MIMEType, TextEncodings TextEncoding, MemoryStream Data, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _TextEncoding = TextEncoding;
            _MIMEType = MIMEType;
            _Description = Description;
            _Data = Data;
        }

        /// <summary>
        /// Gets or Sets current frame TextEncoding
        /// </summary>
        public TextEncodings TextEncoding
        {
            get
            { return _TextEncoding; }
            set
            { _TextEncoding = value; }
        }

        /// <summary>
        /// Gets or Sets current frame MIMEType
        /// </summary>
        public string MIMEType
        {
            get
            { return _MIMEType; }
            set
            { _MIMEType = value; }
        }

        /// <summary>
        /// Gets or sets Description of current frame
        /// </summary>
        public string Description
        {
            get
            { return _Description; }
            set
            {
                if (value == null)
                    throw (new ArgumentException("Description can't set to null"));
                _Description = value;
            }
        }
    }

    ///// <summary>
    ///// A class for frame that include TextEncoding, Description, FileName, MIMEType, Data
    ///// </summary>
    public class GeneralFileFrame : BaseFileFrame
    {
        private string _FileName;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new GeneralFileFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public GeneralFileFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(_TextEncoding, ExceptionLevels.Error, FrameID))
            {
                return;
            }

            _MIMEType = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _FileName = TStream.ReadText(Length, _TextEncoding, ref Length, true);

            _Description = TStream.ReadText(Length, _TextEncoding, ref Length, true);

            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// Create new GeneralFile frame
        /// </summary>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Description">Description of frame</param>
        /// <param name="MIMEType">MimeType of file</param>
        /// <param name="TextEncoding">TextEncoding for storing texts</param>
        /// <param name="FileName">Filename</param>
        /// <param name="Data">Data contain file</param>
        public GeneralFileFrame(FrameFlags Flags, string Description,
            string MIMEType, TextEncodings TextEncoding, string FileName, MemoryStream Data, Stream FS)
            : base("GEOB", Flags, Description, MIMEType, TextEncoding, Data, FS)
        {
            _FileName = FileName;
        }

        /// <summary>
        /// Get/Set FileName of current frame
        /// </summary>
        public string FileName
        {
            get
            { return _FileName; }
            set
            {
                if (value == null)
                    throw (new ArgumentNullException("FileName can't set to null"));

                _FileName = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            return base.OnGetLength() + GetTextLength(_Description, _TextEncoding, true) +
                    GetTextLength(_MIMEType, TextEncodings.Ascii, true) +
                    GetTextLength(_FileName, _TextEncoding, true) + 1;
            //1: Text Encoding
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)_TextEncoding);

            TStream.WriteText(_MIMEType, TextEncodings.Ascii, true);

            TStream.WriteText(_FileName, _TextEncoding, true);

            TStream.WriteText(_Description, _TextEncoding, true);

            _Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            return base.OnValidating();
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(FileName) && StaticMethods.IsAscii(_Description))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Indicate if specific object is equal to this frame
        /// </summary>
        /// <returns>True if equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(GeneralFileFrame))
                return false;

            if (this.FrameID != ((GeneralFileFrame)obj).FrameID)
                return false;

            if (((GeneralFileFrame)obj)._Description == this._Description)
                return true;

            return false;
        }

        /// <summary>
        /// Get hashcode for current frame
        /// </summary>
        /// <returns>int contains hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Convert current GeneralFileFrame to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Description + " [" + FileName + "]";
        }
    }

    /// <summary>
    /// A class for frame that include Text, Encoding, MIMEType, PictureType, Data
    /// </summary>
    public class AttachedPictureFrame : BaseFileFrame
    {
        private PictureTypes _PictureType;

        /// <summary>
        /// Indicates diffrent types of attached pictures
        /// </summary>
        public enum PictureTypes
        {
            /// <summary>
            /// Unknown image type
            /// </summary>
            Other = 0,
            /// <summary>
            /// The icon of file
            /// </summary>
            FileIcon,
            /// <summary>
            /// If file contains more than one icon this can use
            /// </summary>
            OtherFileIcon,
            /// <summary>
            /// Front cover image
            /// </summary>
            Cover_Front,
            /// <summary>
            /// Back cover image
            /// </summary>
            Cover_Back,
            /// <summary>
            /// Picture of Leaflet
            /// </summary>
            LeafletPage,
            /// <summary>
            /// Picture of media
            /// </summary>
            Media,
            /// <summary>
            /// Picture of soloist
            /// </summary>
            Soloist,
            /// <summary>
            /// Picture of band artist
            /// </summary>
            Artist,
            /// <summary>
            /// Picture of conductor
            /// </summary>
            Conductor,
            /// <summary>
            /// Picture of band
            /// </summary>
            Band,
            /// <summary>
            /// Picture of composer
            /// </summary>
            Composer,
            /// <summary>
            /// Picture of Lyricist
            /// </summary>
            Lyricist_TextWriter,
            /// <summary>
            /// Picture of recording location
            /// </summary>
            RecordingLocation,
            /// <summary>
            /// Picture taken while recording
            /// </summary>
            DuringRecording,
            /// <summary>
            /// Picture taken while performance
            /// </summary>
            DuringPerformance,
            /// <summary>
            /// Picture taken from movie, clip
            /// </summary>
            Movie,
            /// <summary>
            /// A Bright Coloued Fish
            /// </summary>
            ABrightColouredFish,
            /// <summary>
            /// Illustration picture
            /// </summary>
            Illustration,
            /// <summary>
            /// Logo of band
            /// </summary>
            BandLogo,
            /// <summary>
            /// Logo of publisher
            /// </summary>
            PublisherLogo
        }
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new AttachedPictureFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">MaxLength of frame</param>
        public AttachedPictureFrame(string FrameID, FrameFlags Flags,  int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(_TextEncoding, ExceptionLevels.Error, FrameID))
            {
                return;
            }

            _MIMEType = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _PictureType = (PictureTypes)TStream.ReadByte(FS);
            Length--;
            
            _Description = TStream.ReadText(Length, _TextEncoding, ref Length, true);

            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// Create new AttachedPicture frame
        /// </summary>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Description">Description of picture</param>
        /// <param name="TextEncoding">TextEncoding use for texts</param>
        /// <param name="MIMEType">MimeType of picture</param>
        /// <param name="PictureType">Picture type</param>
        /// <param name="Data">Data Contain picture</param>
        public AttachedPictureFrame(FrameFlags Flags, string Description,
            TextEncodings TextEncoding, string MIMEType, PictureTypes PictureType,
            MemoryStream Data, Stream FS)
            : base("APIC", Flags, Description, MIMEType, TextEncoding, Data, FS)
        {
            _PictureType = PictureType;
        }

        /// <summary>
        /// Get/Set PictureType of current frame
        /// </summary>
        public PictureTypes PictureType
        {
            get
            { return _PictureType; }
            set
            { _PictureType = value; }
        }

        /// <summary>
        /// Gets or Sets current frame Description
        /// </summary>
        public new string Description
        {
            get
            { return _Description; }
            set
            {
                if (value.Length > 64)
                    throw (new ArgumentException("Attached Picture Description length can't be more than 64 characters"));

                _Description = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            return base.OnGetLength() +
                    GetTextLength(_Description, _TextEncoding, true) +
                    GetTextLength(_MIMEType, TextEncodings.Ascii, true) + 2;
            //1 for Text Encoding and 1 for PictureType
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)_TextEncoding);

            TStream.WriteText(_MIMEType, _TextEncoding, true);

            TStream.FS.WriteByte((byte)_PictureType);

            TStream.WriteText(_Description, _TextEncoding, true);

            _Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            return base.OnValidating();
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(MIMEType) && StaticMethods.IsAscii(Description))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Indicate if specific object is equal to this frame
        /// </summary>
        /// <returns>True if equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AttachedPictureFrame))
                return false;

            // There can be only one Picture with type of
            // FileIcon and OtherFileIcon
            if (this._PictureType == PictureTypes.FileIcon ||
                this._PictureType == PictureTypes.OtherFileIcon)
                if (((AttachedPictureFrame)obj)._PictureType == this._PictureType)
                    return true;

            if (_Description == ((AttachedPictureFrame)obj)._Description)
                return true;

            return false;
        }

        /// <summary>
        /// Get hashcode for current frame
        /// </summary>
        /// <returns>int contains hash</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Convert current Attached Picture to String in format of Description [PictureType]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description + " [" + PictureType.ToString() + "]";
        }

        /// <summary>
        /// Contain Data of current PictureFrame as Image
        /// </summary>
        public Windows.UI.Xaml.Media.ImageSource Picture
        {
            get
            {
                Data.Position = 0;
                var image = new BitmapImage();
                image.SetSource(Data.AsRandomAccessStream());
                return image;
            }
        }
    }

    /// <summary>
    /// A class for frame that include TextEncoding, Price, ValidUntil, ContactUrl,
    /// RecievedAs, Seller, Description, MIMEType, Logo
    /// </summary>
    public class CommercialFrame : BaseFileFrame
    {
        /// <summary>
        /// Price Value
        /// </summary>
        protected Price _Price;
        /// <summary>
        /// Valid Until value
        /// </summary>
        protected SDate _ValidUntil;
        /// <summary>
        /// Contact URL value
        /// </summary>
        protected string _ContactUrl;
        /// <summary>
        /// Recieve as value
        /// </summary>
        protected RecievedAsEnum _RecievedAs;
        /// <summary>
        /// Seller Name value
        /// </summary>
        protected string _SellerName;

        /// <summary>
        /// Indicates diffrent types of recieving file
        /// </summary>
        public enum RecievedAsEnum
        {
            /// <summary>
            /// Other types of recieving
            /// </summary>
            Other = 0,
            /// <summary>
            /// Recieved as CD
            /// </summary>
            StandardCdAlbum,
            /// <summary>
            /// Recieved as Compressed Audio like mp3
            /// </summary>
            CompressedAudio,
            /// <summary>
            /// Downloaded from internet
            /// </summary>
            FileOverInternet,
            /// <summary>
            /// Recieved as stream over internet
            /// </summary>
            StreamOverInternet,
            /// <summary>
            /// Recieved as note sheet
            /// </summary>
            AsNoteSheet,
            /// <summary>
            /// Recieved as note sheet in book
            /// </summary>
            AsNoteSheetInBook,
            /// <summary>
            /// Recieved as music on other media type such as tape
            /// </summary>
            MusicOnOtherMedia,
            /// <summary>
            /// Not buyed
            /// </summary>
            NonMusicalMerchandise,
            /// <summary>
            /// Unknown recieving type
            /// </summary>
            Unknown
        }
        TagStreamUWP TStream;
        /// <summary>
        /// New CommercialFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">Data of frame</param>
        /// <param name="Length">MaxLength of frame</param>
        public CommercialFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            _TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(_TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            _Price = new Price(TStream, Length);
            Length -= _Price.Length;

            _ValidUntil = new SDate(TStream);
            Length -= 8;

            _ContactUrl = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _RecievedAs = (RecievedAsEnum)Data.ReadByte();
            Length--;

            _SellerName = TStream.ReadText(Length, _TextEncoding, ref Length, true);

            _Description = TStream.ReadText(Length, _TextEncoding, ref Length, true);

            if (Length < 1) // Data finished
                return;

            _MIMEType = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _Data = TStream.ReadData(Length);
        }

        /// <summary>
        /// Create new Commercial frame
        /// </summary>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Description">Description for current frame</param>
        /// <param name="TextEncoding">TextEncoding use for texts</param>
        /// <param name="Price">Price that payed for song</param>
        /// <param name="ValidUntil">Validation date</param>
        /// <param name="ContactURL">Contact URL to seller</param>
        /// <param name="RecievedAs">RecievedAd type</param>
        /// <param name="SellerName">SellerName</param>
        /// <param name="MIMEType">MimeType for seller Logo</param>
        /// <param name="Logo">Data Contain Seller Logo</param>
        public CommercialFrame(FrameFlags Flags, string Description,
            TextEncodings TextEncoding, Price Price, SDate ValidUntil, string ContactURL,
            RecievedAsEnum RecievedAs, string SellerName, string MIMEType, MemoryStream Logo, Stream FS)
            : base("COMR", Flags, Description, MIMEType, TextEncoding, Logo, FS)
        {
            _ValidUntil = ValidUntil;
            this.ContactUrl = ContactURL;
            this.SellerName = SellerName;
            this.RecievedAs = RecievedAs;
            _Price = Price;
        }

        /// <summary>
        /// Gets or sets Price payed
        /// </summary>
        public Price Price
        {
            get
            { return _Price; }
        }

        /// <summary>
        /// Gets or sets Validation date
        /// </summary>
        public SDate ValidUntil
        {
            get
            { return _ValidUntil; }
            set
            { _ValidUntil = value; }
        }

        /// <summary>
        /// Gets or sets Contact URL of seller
        /// </summary>
        public string ContactUrl
        {
            get
            { return _ContactUrl; }
            set
            {
                if (value == null)
                    throw (new ArgumentNullException("Can't set Contact url to null"));

                _ContactUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets Recieved As type
        /// </summary>
        public RecievedAsEnum RecievedAs
        {
            get
            { return _RecievedAs; }
            set
            {
                if (!Enum.IsDefined(typeof(RecievedAsEnum), value))
                    throw (new ArgumentException("This is not valid for RecievedAsEnum"));

                _RecievedAs = value;
            }
        }

        /// <summary>
        /// Gets or sets seller name
        /// </summary>
        public string SellerName
        {
            get
            { return _SellerName; }
            set
            {
                if (value == null)
                    throw (new ArgumentNullException("Seller name can't set to null"));

                _SellerName = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        protected override int OnGetLength()
        {
            // 1byte: PriceString seprator
            // 1byte: TextEncoding                
            // 8Byte: ValidUntil date
            // 1Byte: Recieved as
            //--------------------------
            // Sum: 11 Byte
            int RInt;
            RInt = _Price.Length +
                GetTextLength(_ContactUrl, TextEncodings.Ascii, true) +
                GetTextLength(_SellerName, _TextEncoding, true) +
                GetTextLength(_Description, _TextEncoding, true) + 11;

            if (_MIMEType != "" && _MIMEType != null)
            {
                RInt += GetTextLength(_MIMEType, TextEncodings.Ascii, true);
                RInt += Convert.ToInt32(_Data.Length);
            }

            return RInt;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)_TextEncoding);

            TStream.WriteText(_Price.ToString(), TextEncodings.Ascii, true);

            TStream.WriteText(_ValidUntil.String, TextEncodings.Ascii, false);

            TStream.WriteText(_ContactUrl, TextEncodings.Ascii, true);

            TStream.FS.WriteByte((byte)_RecievedAs);

            TStream.WriteText(_SellerName, _TextEncoding, true);

            TStream.WriteText(_Description, _TextEncoding, true);

            if (!LogoExists)
                return;

            TStream.WriteText(_MIMEType, TextEncodings.Ascii, true);

            _Data.WriteTo(TStream.FS);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_Price.Value == "")
                return false;
            return true;
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(SellerName) && StaticMethods.IsAscii(Description))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Indicate if logo exists for this frame
        /// </summary>
        public bool LogoExists
        {
            get
            {
                if (_Data == null || _Data.Length < 1)
                    return false;

                return true;
            }
        }
    }
}