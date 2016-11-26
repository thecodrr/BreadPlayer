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
//using Tags.ID3.ID3v2Frames.OtherFrames;

/*
 * This namespace contain frames that their base information is text(string)
 */
namespace BreadPlayer.Tags.ID3.ID3v2Frames.TextFrames
{
	/// <summary>
	/// A class for frame that only include Text member
	/// </summary> 
	public abstract class TextOnlyFrame : Frame
    {
        private string _Text; // Contain text of current frame

        /// <summary>
        /// Create new TextOnlyFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">Flags of current frame</param>
        protected TextOnlyFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { TStream = new BreadPlayer.Tags.TagStreamUWP(FS); }

        public BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Get or Set current TextOnlyFrame text
        /// </summary>
        public string Text
        {
            get
            { return _Text; }
            set
            { _Text = value; }
        }

        #region -> Override Methods and properties <-

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (Text == null || Text == "")
                return false;
            return true;
        }

        /// <summary>
        /// Call when frame need to write it's data to stream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(_Text, TextEncodings.Ascii, false);
        }

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            // in Ascii Encoding each character is one byte
            return Text.Length;
        }

        #endregion
    }

    /// <summary>
    /// A Class for frames that include Text with TextEncoding
    /// </summary>
    public class TextFrame : TextOnlyFrame
    {
        /*
         * Note: This class support both URL and Text frames
         * the diffrence between these two types is: URL frame don't contain
         * TextEncoding and always use Ascii as Encoding but TextFrames contain
         * URLs start with 'W' texts with 'T'
         */
        private TextEncodings _TextEncoding;

        /// <summary>
        /// Create new TextFrame Class
        /// </summary>
        /// <param name="FrameID">4 Characters frame identifier</param>
        /// <param name="Flags">Flag of frame</param>
        /// <param name="Data">FileStream to read frame data from</param>
        /// <param name="Length">Maximum length of frame</param>
        public TextFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            // If it was URL frame the TextEncoding is ascii and must not read
            if (IsUrl)
                TextEncoding = TextEncodings.Ascii;
            else
            {
                TextEncoding = (TextEncodings)TStream.ReadByte(FS);
                Length--;
                if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                    return;
            }

            Text = TStream.ReadText(Length, _TextEncoding);
        }

        /// <summary>
        /// Create new TextFrame with specific information
        /// </summary>
        /// <param name="Text">Text of TextFrame</param>
        /// <param name="TextEncoding">TextEncoding of TextFrame</param>
        /// <param name="FrameID">FrameID of TextFrame</param>
        /// <param name="Flags">Flags of Frame</param>
        /// <param name="Ver">Minor version of ID3v2</param>
        public TextFrame(string FrameID, FrameFlags Flags, string Text, TextEncodings TextEncoding,
            int Ver, Stream FS)
            : base(FrameID, Flags, FS)
        {
            if (FramesInfo.IsTextFrame(FrameID, Ver) != 1)
                throw (new ArgumentException(FrameID + " is not valid TextFrame FrameID"));

            this.Text = Text;
            this.TextEncoding = TextEncoding;
        }

        /// <summary>
        /// Create new TextFrame for inherited classes
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">Frame Falgs</param>
        protected TextFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// Get or Set current frame TextEncoding
        /// </summary>
        public TextEncodings TextEncoding
        {
            get
            { return _TextEncoding; }
            set
            {
                if (IsValidEnumValue(value, ExceptionLevels.Error, FrameID))
                    _TextEncoding = value;
            }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            if (!IsUrl)
            {
                if (ID3v2.AutoTextEncoding)
                    SetEncoding();

                TStream.FS.WriteByte((byte)_TextEncoding); // Write Text Encoding
                TStream.WriteText(Text, _TextEncoding, false); // Write Text
            }
            else
                TStream.WriteText(Text, TextEncodings.Ascii, false);
        }

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            // 1: Encoding (Url Frames don't contain this
            // TextLength ( Ascii Or Unicode )
            // this frame don't contain text seprator
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            if (IsUrl)
                return GetTextLength(Text, TextEncodings.Ascii, false);
            else
                return (1 + GetTextLength(Text, _TextEncoding, false));

        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            // if TextEncoding and Text value is valid this frame is valid
            // otherwise not
            if (!IsValidEnumValue(_TextEncoding) ||
                Text == null || Text == "")
                return false;
            return true;
        }

        /// <summary>
        /// Set TextEncoding according to Data of current frame
        /// </summary>
        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(Text))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;

        }

        #endregion

        /// <summary>
        /// Determined if specific object is equal to current frame
        /// </summary>
        /// <param name="obj">Object to compare with current object</param>
        /// <returns>true if objects be equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            // if FrameID of two text frames were equal they are equal
            // ( the text is not important )
            return (this.FrameID == ((TextFrame)obj).FrameID);
        }

        /// <summary>
        /// Serves as hash function for particular type
        /// </summary>
        /// <returns>Hash for current frame</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Indicate if current frame contain URL information
        /// </summary>
        protected bool IsUrl
        {
            get
            {
                // first character of URL frames always is 'W'
                return (FrameID[0] == 'W');
            }
        }
    }

    /// <summary>
    /// A Class for frames that include Rating, Counter, Email
    /// </summary>
    public class PopularimeterFrame : TextOnlyFrame
    {
        private long _Counter;
        private byte _Rating;
        TagStreamUWP TStream;
        /// <summary>
        /// New PopularimeterFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">4 Characters tag identifier</param>
        /// <param name="Data">TagStream contain frame data</param>
        /// <param name="Length">Maximum available length for current frame in TagStream</param>
        public PopularimeterFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            EMail = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true); // Read Email Address

            _Rating = TStream.ReadByte(FS); // Read Rating
            Length--;

            if (Length > 8)
            {
                ExceptionOccured(new ID3Exception("Counter value for Popularimeter frame is more than 8 byte." +
                    " this is not supported by this program", FrameID, ExceptionLevels.Error));
                return;
            }

            byte[] LBuf = new byte[8];
            byte[] Buf = new byte[Length];

            TStream.FS.Read(Buf, 0, Length);
            Buf.CopyTo(LBuf, 8 - Buf.Length);
            Array.Reverse(LBuf);

            _Counter = BitConverter.ToInt64(LBuf, 0);
        }

        /// <summary>
        /// New PopulariMeter frame from specific information
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="EMail">Email of user</param>
        /// <param name="Rating">User Rated value</param>
        /// <param name="Counter">How many times user listened to audio</param>
        public PopularimeterFrame(FrameFlags Flags, string EMail,
            byte Rating, long Counter, Stream FS)
            : base("POPM", Flags, FS)
        {
            base.Text = EMail;
            _Rating = Rating;
            _Counter = Counter;
        }

        /// <summary>
        /// Get or Set Rating value for current Email Address
        /// </summary>
        public byte Rating
        {
            get
            { return _Rating; }
            set
            { _Rating = value; }
        }

        /// <summary>
        /// Get or Set Counter for current User (Mail Address)
        /// </summary>
        public long Counter
        {
            get
            { return _Counter; }
            set
            { _Counter = value; }
        }

        /// <summary>
        /// Gets or sets Email for current User
        /// </summary>
        public string EMail
        {
            get
            { return base.Text; }
            set
            { base.Text = value; }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            return 10 + EMail.Length;
            // 1:   Rating Length
            // 1:   Seprator
            // 8:   Counter
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(EMail, TextEncodings.Ascii, true);

            TStream.FS.WriteByte(_Rating);

            byte[] Buf = BitConverter.GetBytes(_Counter);
            Array.Reverse(Buf);
            TStream.FS.Write(Buf, 0, 8);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (EMail != "")
                return true;

            return false;
        }

        #endregion

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (((PopularimeterFrame)obj).EMail == this.EMail)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves Hash function for particular types
        /// </summary>
        /// <returns>HashCode for current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// This property is not usable for this class
        /// </summary>
        public new string Text
        {
            get
            { throw (new Exception("This property is not useable for this class")); }
        }
    }

    /// <summary>
    /// A Class for frames that include Text, Encoding and Description
    /// </summary>
    public class UserTextFrame : TextFrame
    {
        private string _Description;

        /// <summary>
        /// Create new UserTextFrameClass
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">Frame Flagsr</param>
        /// <param name="Data">TagStream to read information from</param>
        /// <param name="Length">Maximum available length of data for current frame in TagStream</param>
        public UserTextFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            _Description = TStream.ReadText(Length, TextEncoding, ref Length, true);

            if (!IsUrl) // is text frame
                Text = TStream.ReadText(Length, TextEncoding);
            else
                Text = TStream.ReadText(Length, TextEncodings.Ascii);

            // User URL frames use this class and use Text property as URL
            // URL property must be in AScii format
            // all URL frames start with W and text frames with T
        }

        /// <summary>
        /// Create new UserTextFrame from specific information
        /// </summary>
        /// <param name="FrameID">FrameID of frame</param>
        /// <param name="Flags">Frame flags</param>
        /// <param name="Text">Frame text</param>
        /// <param name="Description">Frame description</param>
        /// <param name="TextEncoding">TextEncoding of texts</param>
        /// <param name="Ver">Minor version of ID3v2</param>
        public UserTextFrame(string FrameID, FrameFlags Flags, string Text,
            string Description, TextEncodings TextEncoding, int Ver, Stream FS)
            : base(FrameID, Flags, FS)
        {
            if (FramesInfo.IsTextFrame(FrameID, Ver) != 2)
                throw (new ArgumentException(FrameID + " is not valid for UserTextFrame class"));

            this.Text = Text;
            this.TextEncoding = TextEncoding;
            this.Description = Description;
        }

        /// <summary>
        /// Create new UserTextFrame without set any default value
        /// </summary>
        /// <param name="FrameID">4 character frame identifier for current frame</param>
        /// <param name="Flags">Frame Flags</param>
        protected UserTextFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// Get/Set current frame Description
        /// </summary>
        public string Description
        {
            set
            {
                if (value == null)
                    throw (new ArgumentException("Description can't be null"));

                _Description = value;
            }
            get
            { return _Description; }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            // TextLength
            // Description Length ( + seprator )
            // 1: Encoding
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            int TextLen;

            if (!IsUrl)
                TextLen = GetTextLength(Text, TextEncoding, false);
            else
                TextLen = GetTextLength(Text, TextEncodings.Ascii, false); ;

            return 1 + TextLen + GetTextLength(_Description, TextEncoding, true);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)TextEncoding); // Write Encoding

            TStream.WriteText(_Description, TextEncoding, true);

            if (!IsUrl)
                TStream.WriteText(Text, TextEncoding, false);
            else // URL frames always use ascii encoding for text value
                TStream.WriteText(Text, TextEncodings.Ascii, false);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if ((_Description != "" || Text != "") && IsValidEnumValue(TextEncoding))
                return true;

            return false;
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(Text) && StaticMethods.IsAscii(Description))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (this.FrameID == ((UserTextFrame)obj).FrameID
                && this._Description == ((UserTextFrame)obj)._Description)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves Hash function for particular types
        /// </summary>
        /// <returns>HashCode for current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// A Class for frames that include Text, Encoding and Language
    /// </summary>
    public class TermOfUseFrame : TextFrame
    {
        private Language _Language;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new TermOfUseFrame class
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">TagStream to read data from</param>
        /// <param name="Length">Maximum available length for current frame is TagStream</param>
        public TermOfUseFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            _Language = new Language(TStream.FS);
            Length -= 3;

            Text = TStream.ReadText(Length, TextEncoding);
        }

        /// <summary>
        /// Create new TermOfUseFrame with specific information
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Text">Text of current frame</param>
        /// <param name="TextEncoding">Encoding of text</param>
        /// <param name="Lang">Language that text wrote in</param>
        public TermOfUseFrame(FrameFlags Flags, string Text,
            TextEncodings TextEncoding, string Lang, Stream FS)
            : base("USER", Flags, FS)
        {
            this.Text = Text;
            this.TextEncoding = TextEncoding;
            Language = new Language(Lang);
        }

        /// <summary>
        /// Create new empty TermOfUseFrame
        /// </summary>
        /// <param name="FrameID">4 character frame identifier</param>
        /// <param name="Flags">Frame Flags</param>
        protected TermOfUseFrame(string FrameID, FrameFlags Flags, Stream FS)
            : base(FrameID, Flags, FS) { }

        /// <summary>
        /// Gets or sets language of current frame
        /// </summary>
        public Language Language
        {
            get
            { return _Language; }
            set
            { _Language = value; }
        }

        #region -> Override method's and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            // 3: Language Length
            return (base.OnGetLength() + 3);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)TextEncoding); // Write Text Encoding

            _Language.Write(TStream.FS);

            TStream.WriteText(Text, TextEncoding, false);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (base.IsValid == false)
                return false;

            return _Language.IsValidLanguage;
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(Text))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (((TermOfUseFrame)obj)._Language == this._Language &&
                ((TermOfUseFrame)obj).FrameID == this.FrameID)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves Hash function for particular types
        /// </summary>
        /// <returns>HashCode for current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Convert current object to string
        /// </summary>
        /// <returns>String contain converted of current object</returns>
        public override string ToString()
        {
            return "Term of use [" + _Language + "]";
        }
    }

    /// <summary>
    /// A Class for frames that include PricePayed, DateOfPurch, TextEncoding, Text(Seller)
    /// </summary>
    public class OwnershipFrame : TextFrame
    {
        // Inherits:
        //      Text
        //      Encoding
        private Price _Price;
        private SDate _DateOfPurch;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new OwnershipFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">TagStream to read data from</param>
        /// <param name="Length">Maximum available length for current frame is TagStream</param>
        public OwnershipFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            Length--;
            if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            _Price = new Price(TStream, Length);
            Length -= _Price.Length;
            if (!_Price.IsValid)
            {
                ExceptionOccured(new ID3Exception("Price is not valid value. ownership frame will not read", FrameID, ExceptionLevels.Error));
                return;
            }

            if (Length >= 8)
            {
                _DateOfPurch = new SDate(TStream);
                Length -= 8;
            }
            else
            {
                ExceptionOccured(new ID3Exception("Date is not valid for this frame", FrameID, ExceptionLevels.Error));
                return;
            }

            Seller = TStream.ReadText(Length, TextEncoding);
        }

        /// <summary>
        /// Create new Ownership from specific information
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="PricePayed">Price payed for file</param>
        /// <param name="PurchDate">Purch date of file</param>
        /// <param name="Seller">Name of seller</param>
        /// <param name="TEncoding">Tex Encoding to use for text</param>
        public OwnershipFrame(FrameFlags Flags, Price PricePayed, SDate PurchDate,
            string Seller, TextEncodings TEncoding, Stream FS)
            : base("OWNE", Flags, FS)
        {
            _Price = PricePayed;
            _DateOfPurch = PurchDate;
            this.Seller = Seller;
        }

        /// <summary>
        /// Get/Set DateOfPurch for current frame
        /// </summary>
        public SDate DateOfPurch
        {
            get
            { return _DateOfPurch; }
            set
            { _DateOfPurch = value; }
        }

        /// <summary>
        /// Get price of current frame
        /// </summary>
        public Price Price
        {
            get
            {
                return _Price;
            }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            // base.OnGetLength(): 10(Header) + 1(Encoding) + Text.Length(According to encoding)
            // Price.Length + 8(Date) + 1(Seprator of Price)
            return (base.OnGetLength() + _Price.Length) + 9;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData( int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)TextEncoding); // Write Text Encoding

            TStream.WriteText(_Price.ToString(), TextEncodings.Ascii, true);

            TStream.WriteText(_DateOfPurch.String, TextEncodings.Ascii, false);

            TStream.WriteText(Seller, TextEncoding, false);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (_DateOfPurch == null && _Price == null)
                return false;

            return base.OnValidating();
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(Seller))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// This property is not available for Ownership
        /// </summary>
        public new string Text
        {
            get
            {
                throw (new InvalidOperationException("This property not available for Ownership"));
            }
        }

        /// <summary>
        /// Get/Set Current frame seller
        /// </summary>
        public string Seller
        {
            get
            { return base.Text; }
            set
            {
                base.Text = value;
                // Base.Text control the value for null
            }
        }
    }

    /// <summary>
    /// A Class for frames that include FrameIdentifier, URL, AdditionalData
    /// </summary>
    public class LinkFrame : TextFrame
    {
        private string _FrameIdentifier;
        private string _AdditionalData;

        /// <summary>
        /// Create new LinkFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length"></param>
        public LinkFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _FrameIdentifier = TStream.ReadText(4, TextEncodings.Ascii);
            if (!ValidatingFrameID(_FrameIdentifier, ExceptionLevels.Warning))
                return;
            Length -= 4;
            // There is 3 byte in article that i think it's not true
            // because frame identifier is 4 character

            // use Text variable as URL
            URL = TStream.ReadText(Length, TextEncodings.Ascii, ref Length, true);

            _AdditionalData = TStream.ReadText(Length, TextEncodings.Ascii);
        }

        /// <summary>
        /// New LinkedFrame from specific information
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="FrameIdentifier">FrameIdentifier of frame that linked</param>
        /// <param name="URL">URL address of Linked Frame</param>
        /// <param name="AdditionalData">Additional data of Linked Frame</param>
        public LinkFrame(FrameFlags Flags, string FrameIdentifier,
            string URL, string AdditionalData, Stream FS)
            : base("LINK", Flags, FS)
        {
            this.URL = URL;
            _AdditionalData = AdditionalData;

            // Check if FrameIdentifier is valid
            ValidatingFrameID(FrameIdentifier, ExceptionLevels.Warning);

            _FrameIdentifier = FrameIdentifier;
        }

        /// <summary>
        /// URL of current Link Frame
        /// </summary>
        public string URL
        {
            get
            { return base.Text; }
            set
            {
                // Check for null value (base.Text check it)
                base.Text = value;
            }
        }

        /// <summary>
        /// Get/Set Additional Data of Current Frame
        /// </summary>
        public string AdditionalData
        {
            get
            { return _AdditionalData; }
            set
            { _AdditionalData = value; }
        }

        /// <summary>
        /// Frame Identifier of Linked Frame
        /// </summary>
        public string FrameIdentifier
        {
            get
            { return _FrameIdentifier; }
            set
            {
                if (ValidatingFrameID(value, ExceptionLevels.Warning))
                    _FrameIdentifier = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            // 4: FrameIdentifier
            // 1: Seprator
            return 5 + URL.Length + _AdditionalData.Length;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.WriteText(_FrameIdentifier, TextEncodings.Ascii, false);

            TStream.WriteText(URL, TextEncodings.Ascii, true); // Write URL

            TStream.WriteText(_AdditionalData, TextEncodings.Ascii, false);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (_FrameIdentifier == "")
                return false;

            return true;
        }

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (((LinkFrame)obj)._FrameIdentifier == this._FrameIdentifier)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves Hash function for particular types
        /// </summary>
        /// <returns>HashCode for current object</returns>
        public override int GetHashCode() { return base.GetHashCode(); }

        #endregion

        /// <summary>
        /// This property is not Available for current class
        /// </summary>
        public new TextEncodings TextEncoding
        {
            get
            { return TextEncodings.Ascii; }
        }

        /// <summary>
        /// This property is not Available for current class
        /// </summary>
        public new string Text
        {
            get
            { throw (new Exception("This property is not available for current class")); }
        }
    }

    /// <summary>
    /// A Class for frames that include Text, Description, Encoding and Language
    /// </summary>
    public class TextWithLanguageFrame : UserTextFrame
    {
        private Language _Language;

        /// <summary>
        /// Create new TextWithLanguageFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">TagStream contains frame data</param>
        /// <param name="Length">Maximum available length for current frame is TagStream</param>
        public TextWithLanguageFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            TextEncoding = (TextEncodings)TStream.FS.ReadByte();
            Length--;
            if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            _Language = new Language(TStream.FS);
            Length -= 3;

            Description = TStream.ReadText(Length, TextEncoding, ref Length, true);

            Text = TStream.ReadText(Length, TextEncoding);
        }

        /// <summary>
        /// Create new TextWithLanguageFrame from specific information
        /// </summary>
        /// <param name="FrameID">4 character frame identifer for current frame</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Text">Text of current frame</param>
        /// <param name="Description">Description of current frame</param>
        /// <param name="TextEncoding">Text Encoding used for current frame</param>
        /// <param name="Lang">Language used for text of current frame</param>
        public TextWithLanguageFrame(string FrameID, FrameFlags Flags, string Text,
            string Description, TextEncodings TextEncoding, string Lang, Stream FS)
            : base(FrameID, Flags, FS)
        {
            if (FrameID != "USLT" && FrameID != "COMM")
                throw (new ArgumentException(FrameID + " is not valid Frame for TextWithLanguageFrame"));

            Language = new Language(Lang);
            this.Text = Text;
            this.Description = Description;
            this.TextEncoding = TextEncoding;
        }

        /// <summary>
        /// Get/Set current frame Language 
        /// </summary>
        public Language Language
        {
            get
            { return _Language; }
            set
            { _Language = value; }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            // 3: Language Length
            return (base.OnGetLength() + 3);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData( int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)TextEncoding); // Write Text Encoding

            _Language.Write(TStream.FS);

            TStream.WriteText(Description, TextEncoding, true);

            TStream.WriteText(Text, TextEncoding, false);
        }

        /// <summary>
        /// Occur when want to validate frame information
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (IsValidEnumValue(TextEncoding) &&
                    (Text != "" || Description != ""))
                return true;
            else
                return false;
        }

        private void SetEncoding()
        {
            if (StaticMethods.IsAscii(Text) && StaticMethods.IsAscii(Description))
                TextEncoding = TextEncodings.Ascii;
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        #endregion

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (((TextWithLanguageFrame)obj).FrameID == this.FrameID &&
                ((TextWithLanguageFrame)obj)._Language == this._Language &&
                ((TextWithLanguageFrame)obj).Description == this.Description)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves Hash function for particular types
        /// </summary>
        /// <returns>HashCode for current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// convert current Frame to System.String
        /// </summary>
        /// <returns>Description [Language]</returns>
        public override string ToString()
        {
            return Description + " [" + _Language + "]";
        }
    }
}