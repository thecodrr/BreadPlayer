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
using BreadPlayer.Tags.ID3.ID3v2Frames.TextFrames;
//using Tags.ID3.ID3v2Frames.BinaryFrames;

/*
 * This namespace contain frames that have array of information
 */
namespace BreadPlayer.Tags.ID3.ID3v2Frames.ArrayFrames
{
	/// <summary>
	/// A Class for frames that includes TextEncoding, Language, TimeStampFormat, ContentType and ContentDescriptor
	/// </summary>
	public class SynchronisedText : TermOfUseFrame
    {
        // Text is Content Descriptor in this class
        private FrameCollection<Syllable> _Syllables;
        private ContentTypes _ContentType;
        private TimeStamps _TimeStamp;
        new BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Indicates diffrent types of Synchronized text
        /// </summary>
        public enum ContentTypes
        {
            /// <summary>
            /// Other type
            /// </summary>
            Other = 0,
            /// <summary>
            /// Lyric
            /// </summary>
            Lyric,
            /// <summary>
            /// Transcription text
            /// </summary>
            TextTranscription,
            /// <summary>
            /// Movement or part name
            /// </summary>
            MovementOrPartName,
            /// <summary>
            /// Events text
            /// </summary>
            Event,
            /// <summary>
            /// Chord texts
            /// </summary>
            Chord,
            /// <summary>
            /// Popup info
            /// </summary>
            Trivia_PopupInfo
        }

        /// <summary>
        /// New SynchronisedText
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Frame Flag</param>
        /// <param name="Data">FileStream contain current frame data</param>
        /// <param name="Length">Maximum availabel length for this frame</param>
        public SynchronisedText(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _Syllables = new FrameCollection<Syllable>("Syllables");
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            TextEncoding = (TextEncodings)TStream.ReadByte(FS);
            if (!IsValidEnumValue(TextEncoding, ExceptionLevels.Error, FrameID))
                return;

            Length--;

            Language = new Language(TStream.FS);
            Length -= 3;

            _TimeStamp = (TimeStamps)TStream.ReadByte(FS);
            if (!IsValidEnumValue(_TimeStamp, ExceptionLevels.Error, FrameID))
                return;

            Length--;

            _ContentType = (ContentTypes)TStream.ReadByte(FS);
            if (!IsValidEnumValue(_ContentType))
                _ContentType = ContentTypes.Other;
            Length--;

            // use Text variable for descriptor property
            Text = TStream.ReadText(Length, TextEncoding, ref Length, true);

            string tempText;
            uint tempTime;
            while (Length > 5)
            {
                tempText = TStream.ReadText(Length, TextEncoding, ref Length, true);
                tempTime = TStream.ReadUInt(4);

                _Syllables.Add(FrameID, new Syllable(tempTime, tempText));

                Length -= 4;
            }
        }

        /// <summary>
        /// New Synchronised Text
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="TextEncoding">TextEncoding use for texts</param>
        /// <param name="Lang">Language of texts</param>
        /// <param name="TimeStamp">TimeStamps that use for times</param>
        /// <param name="ContentType">ContentType</param>
        /// <param name="ContentDescriptor">Descriptor of Contents</param>
        public SynchronisedText(FrameFlags Flags,
            TextEncodings TextEncoding, string Lang, TimeStamps TimeStamp,
            ContentTypes ContentType, string ContentDescriptor, Stream FS)
            : base("SYLT", Flags, FS)
        {
            _Syllables = new FrameCollection<Syllable>("Syllables");

            this.ContentType = ContentType;
            this.TimeStamp = TimeStamp;
            this.TextEncoding = TextEncoding;
            Language = new Language(Lang);
            this.Text = ContentDescriptor;
        }

        /// <summary>
        /// Gets or Sets TimeStamp of current frame
        /// </summary>
        public TimeStamps TimeStamp
        {
            get { return _TimeStamp; }
            set
            {
                if (IsValidEnumValue(value, ExceptionLevels.Error, FrameID))
                    _TimeStamp = value;
            }
        }

        /// <summary>
        /// Gets or sets ContentType of current frame
        /// </summary>
        public ContentTypes ContentType
        {
            get { return _ContentType; }
            set
            {
                if (IsValidEnumValue(value, ExceptionLevels.Error, FrameID))
                    _ContentType = value;
            }
        }

        #region -> Override method and properties <-

        /// <summary>
        /// Get length of current frame
        /// </summary>
        /// <returns>int contains current frame length</returns>
        protected override int OnGetLength()
        {
            // 3: Language
            // 1: Encoding
            // 2: TimeStamp And ContentType
            // Length of text (+ text seprator)
            // Foreach Syllable 4 byte Time
            // For each Syllable 1/2 byte seprator
            return 6 + GetTextLength(Text, TextEncoding, true)
                + GetSyllablesLength();
        }

        private int GetSyllablesLength()
        {
            int res = 0;
            SetEncoding();
            foreach (Syllable S in _Syllables)
                res += GetTextLength(S.Text, this.TextEncoding, true) + 4;
            return res;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            if (ID3v2.AutoTextEncoding)
                SetEncoding();

            TStream.FS.WriteByte((byte)TextEncoding); // Write Text Encoding

            Language.Write(TStream.FS); // Write Language

            TStream.FS.WriteByte((byte)_TimeStamp);
            TStream.FS.WriteByte((byte)_ContentType);

            TStream.WriteText(Text, TextEncoding, true);

            _Syllables.Sort(); // Sort Syllables

            byte[] Buf;
            foreach (Syllable sb in _Syllables)
            {
                TStream.WriteText(sb.Text, TextEncoding, true);

                Buf = BitConverter.GetBytes(sb.Time);
                Array.Reverse(Buf);
                TStream.FS.Write(Buf, 0, 4);
            }
        }

        /// <summary>
        /// Validate frame data
        /// </summary>
        /// <returns>true if valid otherwise false</returns>
        protected override bool OnValidating()
        {
            if (Text == null || _Syllables == null || _Syllables.Count == 0)
                return false;

            return true;
        }

        #endregion

        /// <summary>
        /// Indicate and set Encoding of current frame automatically
        /// </summary>
        private void SetEncoding()
        {
            // Text in this class is content descriptor
            if (StaticMethods.IsAscii(Text))
                foreach (Syllable Sb in Syllables)
                {
                    if (!StaticMethods.IsAscii(Sb.Text))
                    {
                        TextEncoding = ID3v2.DefaultUnicodeEncoding;
                        break;
                    }
                }
            else
                TextEncoding = ID3v2.DefaultUnicodeEncoding;
        }

        /// <summary>
        /// Gets Syllables of current frame
        /// </summary>
        public FrameCollection<Syllable> Syllables
        {
            get
            { return _Syllables; }
        }

        /// <summary>
        /// Get a syllable for specific time of lyric
        /// </summary>
        /// <param name="Time">Time to get syllable</param>
        /// <returns>System.String contain syllable for specific time or Null if syllable not available in that time</returns>
        public string GetText(uint Time)
        {
            foreach (Syllable S in _Syllables)
            {
                if (Time >= S.Time)
                    return S.Text;
            }
            return null;
        }

        /// <summary>
        /// Indicate if current object equals to specific object
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>true if euals otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (((SynchronisedText)obj).Language == this.Language &&
                ((SynchronisedText)obj).Text == this.Text)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get Hashcode for current frame
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return base.GetHashCode(); }

        /// <summary>
        /// Convert current frame to string
        /// </summary>
        /// <returns>System.String contains current frame</returns>
        public override string ToString()
        {
            return Text + " [" + Language + "]";
        }
    }

    /// <summary>
    /// Provide a class with Text and Time this class used SynchronisedText class and don't have any other usage
    /// </summary>
    public class Syllable : IComparable
    {
        /// <summary>
        /// Text of syllable
        /// </summary>
        protected string _Text;
        /// <summary>
        /// Time of syllable
        /// </summary>
        protected uint _Time;

        /// <summary>
        /// Create ne Syllable class
        /// </summary>
        /// <param name="Time">Absoulute Time for Syllable</param>
        /// <param name="Text">Text of Syllable</param>
        public Syllable(uint Time, string Text)
        {
            _Text = Text;
            _Time = Time;
        }

        /// <summary>
        /// Create new Syllable with 0 as time and string.empty as text
        /// </summary>
        public Syllable()
        {
            Text = "";
            Time = 0;
        }

        /// <summary>
        /// Gets or sets Text of current syllable
        /// </summary>
        public string Text
        {
            get
            { return _Text; }
            set
            { _Text = value; }
        }

        /// <summary>
        /// Gets or sets absolute time of current Syllable
        /// </summary>
        
        public uint Time
        {
            get
            { return _Time; }
            set
            { _Time = value; }
        }

        /// <summary>
        /// Compare current syllable to specific one
        /// </summary>
        /// <returns>Positive number if current is greater otherwise false</returns>
        public int CompareTo(object obj)
        {
            if (this._Time > ((Syllable)obj)._Time)
                return 1;
            else if (this._Time < ((Syllable)obj)._Time)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// Convert current syllable to string
        /// </summary>
        /// <returns>System.String contains current syllable</returns>
        public override string ToString()
        {
            return _Time.ToString() + ":" + _Text;
        }
    }

    /// <summary>
    /// A class for frame that include TempCodes, TimeStampFormat
    /// </summary>
    public class SynchronisedTempoFrame : Frame
    {
        private FrameCollection<TempoCode> _TempoCodes;
        private TimeStamps _TimeStamp;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new STempoCodes
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length"></param>
        public SynchronisedTempoFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _TempoCodes = new FrameCollection<TempoCode>("Temnpo Codes");
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _TimeStamp = (TimeStamps)TStream.ReadByte(FS);
            if (IsValidEnumValue(_TimeStamp, ExceptionLevels.Error, FrameID))
                return;

            int Tempo;
            uint Time;

            while (Length > 4)
            {
                Tempo = TStream.ReadByte(FS);
                Length--;

                if (Tempo == 0xFF)
                {
                    Tempo += TStream.ReadByte(FS);
                    Length--;
                }

                Time = TStream.ReadUInt(4);
                Length -= 4;

                _TempoCodes.Add(FrameID, new TempoCode(Tempo, Time));
            }
        }

        /// <summary>
        /// Create new SynchronisedTempoFrame from TimeStamps
        /// </summary>
        /// <param name="Flags">FrameFlags</param>
        /// <param name="TimeStamp">TimeStamps for current SynchronisedTempoFrame</param>
        public SynchronisedTempoFrame(FrameFlags Flags, TimeStamps TimeStamp, Stream FS)
            : base("SYTC", Flags, FS)
        {
            _TempoCodes = new FrameCollection<TempoCode>("Tempo Codes");

            this.TimeStampFormat = TimeStamp;
        }

        /// <summary>
        /// Get/Set TimeStamp for current frame
        /// </summary>
        public TimeStamps TimeStampFormat
        {
            get
            { return _TimeStamp; }
            set
            {
                if (IsValidEnumValue(value, ExceptionLevels.Error, FrameID))
                    _TimeStamp = value;
            }
        }

        /// <summary>
        /// Gets Collection of TempoCode for current frame
        /// </summary>
        public FrameCollection<TempoCode> TempoCodes
        {
            get { return _TempoCodes; }
        }

        #region -> Override Method and properties <-
 
        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            return _TempoCodes.Length + 1;
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override bool OnValidating()
        {
            if (_TempoCodes.Count == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="wr">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;

            TStream.FS.WriteByte((byte)_TimeStamp);

            _TempoCodes.Sort();

            foreach (TempoCode TC in _TempoCodes.ToArray())
            {
                Buf = TC.Data();
                TStream.FS.Write(Buf, 0, Buf.Length);
            }
        }

        #endregion
    }

    /// <summary>
    /// Provide Tempo for STempoCodes
    /// </summary>
    public class TempoCode : IComparable, ILengthable
    {
        private int _Tempo;
        private uint _Time;

        /// <summary>
        /// Create new TempoCode
        /// </summary>
        /// <param name="Tempo">Tempo for current frame</param>
        /// <param name="Time">Time for current frame</param>
        public TempoCode(int Tempo, uint Time)
        {
            _Tempo = Tempo;
            _Time = Time;
        }

        /// <summary>
        /// Get/Set current Tempo
        /// </summary>
        public int Tempo
        {
            get
            { return _Tempo; }
            set
            {
                if (value > 510 || value < 2)
                    throw (new ArgumentException("Tempo must be between 2-510"));

                _Tempo = value;
            }
        }

        /// <summary>
        /// Get/Set Current frame time
        /// </summary>
        public uint Time
        {
            get
            { return _Time; }
            set
            { _Time = value; }
        }

        /// <summary>
        /// Get byte of information for current TempoCode
        /// </summary>
        /// <returns>byte array contain TempoCode data</returns>
        internal byte[] Data()
        {
            byte[] Buf = new byte[Length];
            int c = 0;
            if (_Tempo > 0xFF)
            {
                Buf[c++] = 0xFF;
                Buf[c++] = Convert.ToByte(_Tempo - 0xFF);
            }
            else
                Buf[c++] = Convert.ToByte(_Tempo);

            byte[] B = BitConverter.GetBytes(_Time);
            Array.Reverse(B);
            Array.Copy(B, 0, Buf, c, 4);

            return Buf;
        }

        /// <summary>
        /// Determines wheter the specified object is equal to current object
        /// </summary>
        /// <param name="obj">object to compare with current object</param>
        /// <returns>true if they were equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(TempoCode))
                return false;

            if (((TempoCode)obj)._Time == this._Time)
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
        /// Compare current TempoCode to specific object
        /// </summary>
        /// <param name="obj">Object to comapre current TempoCode</param>
        /// <returns>0 if equal, 1 if current frame be greater than object otherwise -1</returns>
        public int CompareTo(object obj)
        {
            if (Equals(obj))
                return 0;
            else if (this._Time > ((TempoCode)obj)._Time)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        public int Length
        {
            get
            {
                if (Tempo > 0xFF)
                    return 6;
                else
                    return 5;
            }
        }

        /// <summary>
        /// Gets value of current TempoCode converted to string
        /// </summary>
        /// <returns>System.String contain value of current TempoCode</returns>
        public override string ToString()
        {
            return _Tempo.ToString() + ":" + _Time.ToString();
        }
    }

    /// <summary>
    /// Provide a class for Equalisation frame
    /// </summary>
    public class Equalisation : Frame
    {
        private byte _AdjustmentBits;
        private FrameCollection<FrequencyAdjustmentFrame> _Frequensies;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new Equalisation frame
        /// </summary>
        /// <param name="FrameID">4 characters frame identifer of current Frame class</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">TagStream to read frame from</param>
        /// <param name="Length">Maximum length to read frame</param>
        public Equalisation(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _Frequensies = new FrameCollection<FrequencyAdjustmentFrame>("Frequency Adjustment");
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _AdjustmentBits = TStream.ReadByte(FS);
            Length--;

            if (_AdjustmentBits == 0)
            {
                ExceptionOccured(new ID3Exception("Adjustment bit of Equalisation is zero. this frame is invalid", FrameID, ExceptionLevels.Error));
                return;
            }

            if (_AdjustmentBits % 8 != 0 || _AdjustmentBits > 32)
            {
                ExceptionOccured(new ID3Exception("AdjustmentBit of Equalisation Frame is out of supported range of this program", FrameID, ExceptionLevels.Error));
                return;
            }

            int AdLen = _AdjustmentBits / 8;

            int FreqBuf;
            uint AdjBuf;
            while (Length > 3)
            {
                FreqBuf = Convert.ToInt32(TStream.ReadUInt(2));

                AdjBuf = TStream.ReadUInt(AdLen);
                _Frequensies.Add(FrameID, new FrequencyAdjustmentFrame(FreqBuf, AdjBuf));

                Length -= 2 + AdLen;
            }
        }

        /// <summary>
        /// Create new Equalisation Frame
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="AdjustmentBits">AdjustmentBit of current </param>
        public Equalisation(FrameFlags Flags, byte AdjustmentBits, Stream FS)
            : base("EQUA", Flags, FS)
        {
            this.AdjustmentLength = AdjustmentBits;

            _Frequensies = new FrameCollection<FrequencyAdjustmentFrame>("FrequencyAdjustment");
        }

        /// <summary>
        /// Gets or Sets Adjustment length in bit
        /// </summary>
        public byte AdjustmentLength
        {
            get
            { return _AdjustmentBits; }
            set
            {
                if (value == 0 || value % 8 != 0 || value > 32)
                    throw (new ArgumentOutOfRangeException("Adjustment bits must be in range of 8 - 32 and be multiple of 8"));

                _AdjustmentBits = value;
            }
        }

        /// <summary>
        /// Get All frequencis
        /// </summary>
        /// <returns>frequencis array</returns>
        public FrameCollection<FrequencyAdjustmentFrame> Frequencies
        {
            get
            { return _Frequensies; }
        }

        #region -> Overide Methods <-

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            int RLen = 0;
            RLen = _Frequensies.Count * (1 + (_AdjustmentBits / 8));
            return RLen + 1;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data</param>
        /// <param name="MinorVersion">ID3 minor version</param>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;

            TStream.FS.WriteByte(_AdjustmentBits);

            foreach (FrequencyAdjustmentFrame FA in _Frequensies.ToArray())
            {
                Buf = FA.GetBytes(_AdjustmentBits);
                TStream.FS.Write(Buf, 0, Buf.Length);
            }
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>true if was valid frame otherwise false</returns>
        protected override bool OnValidating()
        {
            if (_Frequensies.Count == 0)
                return false;

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Provide a class for frequency frames. containing Inc/Dec, Frequency, Adjustment
    /// </summary>
    public class FrequencyAdjustmentFrame : ILengthable, IComparable
    {
        private IncrementDecrement _IncDec;
        private int _Frequency;
        private uint _Adjustment;

        /// <summary>
        /// Create new FrequencyAdjustment Frame
        /// </summary>
        /// <param name="Frequency">Frequency with inc/dec bit</param>
        /// <param name="Adjustment">Adjustment</param>
        public FrequencyAdjustmentFrame(int Frequency, uint Adjustment)
        {
            _IncDec = (IncrementDecrement)Convert.ToByte(Frequency & 0x8000);
            Frequency &= 0x7FFF;

            _Adjustment = Adjustment;
            _Frequency = Frequency;
        }

        /// <summary>
        /// Create new FrequencyAdjustment frame
        /// </summary>
        /// <param name="IncDec">Increment/Decrement</param>
        /// <param name="Frequency">Frequency</param>
        /// <param name="Adjustment">Adjustment</param>
        public FrequencyAdjustmentFrame(IncrementDecrement IncDec
            , int Frequency, uint Adjustment)
        {
            _IncDec = IncDec;
            this.Frequency = Frequency;
            _Adjustment = Adjustment;
        }

        /// <summary>
        /// Get/Set Frequency of current FrequencyAdjustmentFrame
        /// </summary>
        public int Frequency
        {
            get
            { return _Frequency; }
            set
            {
                if (value > 0x7FFF || value < 0)
                    throw (new ArgumentException("Frequency value must be between 0 - 32767 Hz"));

                _Frequency = value;
            }
        }

        /// <summary>
        /// Get/Set Adjustment for current Frequency frame
        /// </summary>
        public uint Adjustment
        {
            get { return _Adjustment; }
            set { _Adjustment = value; }
        }

        /// <summary>
        /// Convert current Frequency adjustment to byte array
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBytes(int AdjustmentBits)
        {
            int AdByte = AdjustmentBits / 8;
            byte[] Buf = new byte[AdByte + 2];
            byte[] Temp;
            int AddFreq = _Frequency;

            if (_IncDec == IncrementDecrement.Increment)
                AddFreq |= 0xFFFF;

            Temp = BitConverter.GetBytes(AddFreq);
            Array.Reverse(Temp);

            Array.Copy(Temp, 0, Buf, 0, 2);

            Temp = BitConverter.GetBytes(_Adjustment);
            Array.Reverse(Temp);
            Array.Copy(Temp, 0, Buf, 0, AdByte);

            return Buf;
        }

        // Length of Frequencies must calculate in FrequencyFrame
        /// <summary>
        /// Gets length of current frame
        /// </summary>
        public int Length
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Compare current Frame with specific object
        /// </summary>
        /// <param name="obj">Object to compare with current frame</param>
        /// <returns>A number greater than zero if Current frame be greater that object or zero if they're equal otherwise returns negative number</returns>
        public int CompareTo(object obj)
        {
            return this._Frequency - ((FrequencyAdjustmentFrame)obj)._Frequency;
        }
    }

    /// <summary>
    /// A class for EventTimingCode frame
    /// </summary>
    public class EventTimingCodeFrame : Frame
    {
        private TimeStamps _TimeStamp;
        private FrameCollection<EventCode> _Events;
        BreadPlayer.Tags.TagStreamUWP TStream;
        /// <summary>
        /// Create new EventTimingCodeFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Data">TagStream to read data from</param>
        /// <param name="Length">Maximum available length</param>
        public EventTimingCodeFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            _Events = new FrameCollection<EventCode>("EventCode");
            TStream = new BreadPlayer.Tags.TagStreamUWP(FS);
            _TimeStamp = (TimeStamps)TStream.ReadByte(FS);
            if (!IsValidEnumValue(_TimeStamp, ExceptionLevels.Error, FrameID))
                return;

            Length--;

            while (Length >= 5)
            {
                _Events.Add(FrameID, new EventCode(TStream.ReadByte(FS), TStream.ReadUInt(4)));

                Length -= 5;
            }
        }

        /// <summary>
        /// Gets or sets TimeStamp of current EventTimeCodeFrame
        /// </summary>
        public TimeStamps TimeStamp
        {
            get
            { return _TimeStamp; }
            set
            {
                if (IsValidEnumValue(value, ExceptionLevels.Error, FrameID))
                    _TimeStamp = value;
            }
        }

        /// <summary>
        /// Create new EventTimingCode frame
        /// </summary>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="TimeStamp">TimeStamp use for times</param>
        public EventTimingCodeFrame(FrameFlags Flags, TimeStamps TimeStamp, Stream FS)
            : base("ETCO", Flags, FS)
        {
            this.TimeStamp = TimeStamp;
        }

        /// <summary>
        /// Gets all Events for current frame
        /// </summary>
        public FrameCollection<EventCode> Events
        {
            get { return _Events; }
        }

        #region -> Override Methods <-

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_Events.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            _Events.Sort();
            byte[] Buf;

            TStream.FS.WriteByte((byte)_TimeStamp);

            foreach (EventCode EC in _Events.ToArray())
            {
                TStream.FS.WriteByte((byte)EC.EventType);
                Buf = BitConverter.GetBytes(EC.Time);
                Array.Reverse(Buf);
                TStream.FS.Write(Buf, 0, Buf.Length);
            }
        }

        /// <summary>
        /// Gets length of current frame in byte
        /// </summary>
        /// <returns>int contain length of current frame</returns>
        protected override int OnGetLength()
        {
            return _Events.Count * 5 + 1;
            // 1: TimeStamp
        }

        #endregion
    }

    /// <summary>
    /// A class for Event Codes
    /// </summary>
    public class EventCode : ILengthable
    {
        private byte _EventType;
        private uint _Time;

        /// <summary>
        /// Create new EventCode
        /// </summary>
        /// <param name="EventType">Event Type</param>
        /// <param name="Time">Time of Event</param>
        public EventCode(byte EventType, uint Time)
        {
            _EventType = EventType;
            _Time = Time;
        }

        /// <summary>
        /// Gets or sets Event type
        /// </summary>
        public byte EventType
        {
            get
            { return _EventType; }
            set
            { _EventType = value; }
        }

        /// <summary>
        /// Gets or set Time of event
        /// </summary>
        public uint Time
        {
            get
            { return _Time; }
            set
            { _Time = value; }
        }

        /// <summary>
        /// Gets length of EventCode
        /// </summary>
        public int Length
        {
            get { return 5; }
        }

        /// <summary>
        /// Convert current EventCode to System.String
        /// </summary>
        /// <returns>System.String contain converted of current EventCode</returns>
        public override string ToString()
        {
            return _Time.ToString() + ":" + _EventType.ToString();
        }
    }
}
