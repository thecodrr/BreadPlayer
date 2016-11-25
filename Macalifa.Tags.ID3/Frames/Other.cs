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

namespace BreadPlayer.Tags.ID3.ID3v2Frames.OtherFrames
{
	/// <summary>
	/// Provide a class for Reverb frame
	/// </summary>
	public class ReverbFrame : Frame
    {
        private int _ReverbLeft;
        private int _ReverbRight;
        private byte _ReverbBouncesLeft;
        private byte _ReverbBouncesRight;
        private byte _ReverbFeedbackLeftToRight;
        private byte _ReverbFeedbackRightToLeft;
        private byte _ReverbFeedbackRightToRight;
        private byte _ReverbFeedbackLeftToLeft;
        private byte _PremixLeftToRight;
        private byte _PremixRightToLeft;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new reveb frame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="Data">Data for frame to read from</param>
        /// <param name="Length">Maximum length of frame</param>
        public ReverbFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            if (Length != 12)
            {
                //RaiseErrorEvent(new ID3Error(208, ID3Versions.ID3v2, _FrameID,
                //"Reveb frame is not in correct length. it will drop", ErrorType.Error));
                return;
            }

            _ReverbLeft = Convert.ToInt32(TStream.ReadUInt(2));
            _ReverbRight = Convert.ToInt32(TStream.ReadUInt(2));
            _ReverbBouncesLeft = TStream.ReadByte(FS);
            _ReverbBouncesRight = TStream.ReadByte(FS);
            _ReverbFeedbackLeftToLeft = TStream.ReadByte(FS);
            _ReverbFeedbackLeftToRight = TStream.ReadByte(FS);
            _ReverbFeedbackRightToRight = TStream.ReadByte(FS);
            _ReverbFeedbackRightToLeft = TStream.ReadByte(FS);
            _PremixLeftToRight = TStream.ReadByte(FS);
            _PremixRightToLeft = TStream.ReadByte(FS);
        }

        /// <summary>
        /// Create new Reverb frame and set all values to zero
        /// </summary>
        /// <param name="Flags">Frame Flags</param>
        public ReverbFrame(FrameFlags Flags, Stream FS)
            : base("RVRB", Flags, FS) { }

        #region -> Public properties <-

        /// <summary>
        /// ReverbLeft of current Reveb frame
        /// </summary>
        public int ReverbLeft
        {
            get
            { return _ReverbLeft; }
            set
            { _ReverbLeft = value; }
        }

        /// <summary>
        /// ReverbRight of current reverb frame
        /// </summary>
        public int ReverbRight
        {
            get
            { return _ReverbRight; }
            set
            { _ReverbRight = value; }
        }

        /// <summary>
        /// ReverbBouncesLeft of current reverb frame
        /// </summary>
        public byte ReverbBouncesLeft
        {
            get
            { return _ReverbBouncesLeft; }
            set
            { _ReverbBouncesLeft = value; }
        }

        /// <summary>
        /// ReverbBouncesRight of current reverb frame
        /// </summary>
        public byte ReverbBouncesRight
        {
            get
            { return _ReverbBouncesRight; }
            set
            { _ReverbBouncesRight = value; }
        }

        /// <summary>
        /// ReverbFeedbackLeftToRight of current reverb frame
        /// </summary>
        public byte ReverbFeedbackLeftToRight
        {
            get
            { return _ReverbFeedbackLeftToRight; }
            set
            { _ReverbFeedbackLeftToRight = value; }
        }

        /// <summary>
        /// ReverbFeedbackRightToLeft of current reverb frame
        /// </summary>
        public byte ReverbFeedbackRightToLeft
        {
            get
            { return _ReverbFeedbackRightToLeft; }
            set
            { _ReverbFeedbackRightToLeft = value; }
        }

        /// <summary>
        /// ReverbFeedbackRightToRight of current reverb frame
        /// </summary>
        public byte ReverbFeedbackRightToRight
        {
            get
            { return _ReverbFeedbackRightToRight; }
            set
            { _ReverbFeedbackRightToRight = value; }
        }

        /// <summary>
        /// ReverbFeedbackLeftToLeft of current reverb frame
        /// </summary>
        public byte ReverbFeedbackLeftToLeft
        {
            get
            { return _ReverbFeedbackLeftToLeft; }
            set
            { _ReverbFeedbackLeftToLeft = value; }
        }

        /// <summary>
        /// PremixLeftToRight of current reverb frame
        /// </summary>
        public byte PremixLeftToRight
        {
            get
            { return _PremixLeftToRight; }
            set
            { _PremixLeftToRight = value; }
        }

        /// <summary>
        /// PremixRightToLeft of current reverb frame
        /// </summary>
        public byte PremixRightToLeft
        {
            get
            { return _PremixRightToLeft; }
            set
            { _PremixRightToLeft = value; }
        }

        #endregion

        #region -> Override Method and properties <-
        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;

            Buf = BitConverter.GetBytes(_ReverbLeft);
            Array.Reverse(Buf);
            TStream.FS.Write(Buf, 2, 2);

            Buf = BitConverter.GetBytes(_ReverbRight);
            Array.Reverse(Buf);
            TStream.FS.Write(Buf, 2, 2);

            TStream.FS.WriteByte(_ReverbBouncesLeft);
            TStream.FS.WriteByte(_ReverbBouncesRight);
            TStream.FS.WriteByte(_ReverbFeedbackLeftToLeft);
            TStream.FS.WriteByte(_ReverbFeedbackLeftToRight);
            TStream.FS.WriteByte(_ReverbFeedbackRightToRight);
            TStream.FS.WriteByte(_ReverbFeedbackRightToLeft);
            TStream.FS.WriteByte(_PremixLeftToRight);
            TStream.FS.WriteByte(_PremixRightToLeft);
        }

        /// <summary>
        /// Validate current frame data
        /// </summary>
        /// <returns>True if valid otherwise false</returns>
        protected override bool OnValidating()
        {
            return true;
        }

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        /// <returns>int contains lenght of current frame</returns>
        protected override int OnGetLength()
        {
            return 12;
        }

        #endregion
    }

    /// <summary>
    /// A Class for frames that include Counter
    /// </summary>
    public class PlayCounterFrame : Frame
    {
        /// <summary>
        /// Time that file played
        /// </summary>
        protected long _Counter;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new PlayCounter
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">Maximum length of frame</param>
        public PlayCounterFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            byte[] Long = new byte[8];
            byte[] Buf = new byte[Length];
            // Less than 4 Characters
            TStream.FS.Read(Buf, 0, Length);
            Buf.CopyTo(Long, 8 - Buf.Length);
            Array.Reverse(Long);
            _Counter = BitConverter.ToInt64(Long, 0);
        }

        /// <summary>
        /// Add one to counter
        /// </summary>
        public void AddOne()
        { Counter++; }

        /// <summary>
        /// Gets or Sets Counter of current PlayCounter
        /// </summary>
        public long Counter
        {
            get
            { return _Counter; }
            set
            {
                if (value < 0)
                    throw (new ArgumentException("Counter value can't be less than zero"));

                _Counter = value;
            }
        }

        #region -> Override Methods and properties <-

        /// <summary>
        /// Gets Length of current play counter
        /// </summary>
        /// <returns>int contains length of current PlayCounter</returns>
        protected override int OnGetLength()
        {
            // The Length of counter can't be less than 4 (32bit)
            // In this program we always save it in 8 byte (64bit value)
            // 8: Long value (Counter)
            return 8;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;
            Buf = BitConverter.GetBytes(_Counter);
            Array.Reverse(Buf);
            TStream.FS.Write(Buf, 0, 8);
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        /// <returns>True if valid otherwise false</returns>
        protected override bool OnValidating()
        {
            if (_Counter <= 0)
                return false;

            return true;
        }

        #endregion
    }

    /// <summary>
    /// A class for RelativeVolumeAdjustment
    /// </summary>
    public class RelativeVolumeFrame : Frame
    {
        private byte _IncDec; // Increment Decrement Byte

        private byte _BitForVolumeDescription;
        // All volume descriptors are 12 items we store them in a array
        private uint[] _Descriptors;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new RaltiveVolumeFrame
        /// </summary>
        /// <param name="FrameID">4 Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">Length to read from FileStream</param>
        public RelativeVolumeFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            _Descriptors = new uint[12];

            _IncDec = TStream.ReadByte(FS); // Read Increment Decrement Byte

            _BitForVolumeDescription = TStream.ReadByte(FS); // Read Volume Description Length
            Length -= 2;

            if (_BitForVolumeDescription == 0)
            {
                ExceptionOccured(new ID3Exception("BitForVolumeDescription of Relative volume information frame can't be zero", ExceptionLevels.Error));
                return;
            }

            if (_BitForVolumeDescription / 8 > 4 ||
                _BitForVolumeDescription % 8 != 0)
            {
                ExceptionOccured(new ID3Exception("This program don't support " + _BitForVolumeDescription.ToString() +
                    " Bits of description for Relative Volume information", ExceptionLevels.Error));
                return;
            }

            int DesLen = _BitForVolumeDescription / 8; // Length of each volume descriptor
            int Counter = 0;
            while (CanContinue(Length, DesLen, 2))
            {
                _Descriptors[Counter++] = TStream.ReadUInt(DesLen);
                _Descriptors[Counter++] = TStream.ReadUInt(DesLen);
                Length -= 2;
            }
        }

     

        /// <summary>
        /// Indicate if reading data can continue
        /// </summary>
        /// <param name="MaxLength">Maximum available length</param>
        /// <param name="DesLen">Length of each Volume Descriptor</param>
        /// <param name="BlockToRead">How many descriptors want to read</param>
        /// <returns>true if data is availabe otherwise false</returns>
        private bool CanContinue(int MaxLength, int DesLen, int BlockToRead)
        {
            if (MaxLength >= DesLen * BlockToRead)
                return true;

            return false;
        }

        #region -> IncrementDecrement Properties <-

        /// <summary>
        /// Indicate is it increment or decrement for right
        /// </summary>
        public IncrementDecrement Right
        {
            get
            { return (IncrementDecrement)(_IncDec & 1); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 1; // 00000001
                else
                    _IncDec &= 254; // 11111110
            }
        }

        /// <summary>
        /// Indicate is it increment or decrement for left
        /// </summary>
        public IncrementDecrement Left
        {
            get
            { return (IncrementDecrement)(_IncDec & 2); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 2; // 00000010
                else
                    _IncDec &= 253; // 11111101
            }
        }

        /// <summary>
        /// Indicate is it increment or decrement for rear right
        /// </summary>
        public IncrementDecrement RightBack
        {
            get
            { return (IncrementDecrement)(_IncDec & 4); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 4; // 00000100
                else
                    _IncDec &= 251; // 11111011
            }
        }

        /// <summary>
        /// Indicate is it increment or decrement for rear left
        /// </summary>
        public IncrementDecrement LeftBack
        {
            get
            { return (IncrementDecrement)(_IncDec & 8); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 8; // 00001000
                else
                    _IncDec &= 247; // 11110111
            }
        }

        /// <summary>
        /// Indicate is it increment or decrement for center
        /// </summary>
        public IncrementDecrement Center
        {
            get
            { return (IncrementDecrement)(_IncDec & 16); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 16; // 00010000
                else
                    _IncDec &= 239; // 11101111
            }
        }

        /// <summary>
        /// Indicate is it increment or decrement for bass
        /// </summary>
        public IncrementDecrement Bass
        {
            get
            { return (IncrementDecrement)(_IncDec & 32); }
            set
            {
                if (value == IncrementDecrement.Increment)
                    _IncDec |= 32; // 00100000
                else
                    _IncDec &= 223; // 11011111
            }
        }

        #endregion

        #region -> Volumes Properties <-

        /// <summary>
        /// Change volume for right (relative value)
        /// </summary>
        public uint RelativeVolumeChangeRight
        {
            get
            { return (uint)_Descriptors[0]; }
            set
            { _Descriptors[0] = value; }
        }

        /// <summary>
        /// Change volume for left (relative value)
        /// </summary>
        public uint RelativeVolumeChangeLeft
        {
            get
            { return (uint)_Descriptors[1]; }
            set
            { _Descriptors[1] = value; }
        }

        /// <summary>
        /// Peak volume of right
        /// </summary>
        public uint PeakVolumeRight
        {
            get
            { return (uint)_Descriptors[2]; }
            set
            { _Descriptors[2] = value; }
        }

        /// <summary>
        /// Peak volume of left
        /// </summary>
        public uint PeakVolumeLeft
        {
            get
            { return (uint)_Descriptors[3]; }
            set
            { _Descriptors[3] = value; }
        }

        /// <summary>
        /// Change volume for rear right (relative value)
        /// </summary>
        public uint RelativeVolumeChangeRightBack
        {
            get
            { return (uint)_Descriptors[4]; }
            set
            { _Descriptors[4] = value; }
        }

        /// <summary>
        /// Change volume for rear left (relative value)
        /// </summary>
        public uint RelativeVolumeChangeLeftBack
        {
            get
            { return (uint)_Descriptors[5]; }
            set
            { _Descriptors[5] = value; }
        }

        /// <summary>
        /// Peak volume of rear right
        /// </summary>
        public uint PeakVolumeRightBack
        {
            get
            { return (uint)_Descriptors[6]; }
            set
            { _Descriptors[6] = value; }
        }

        /// <summary>
        /// Peak volume of rear left
        /// </summary>
        public uint PeakVolumeLeftBack
        {
            get
            { return (uint)_Descriptors[7]; }
            set
            { _Descriptors[7] = value; }
        }

        /// <summary>
        /// Change volume for center (relative value)
        /// </summary>
        public uint RelativeVolumeChangeCenter
        {
            get
            { return (uint)_Descriptors[8]; }
            set
            { _Descriptors[8] = value; }
        }

        /// <summary>
        /// Peak volume of center
        /// </summary>
        public uint PeakVolumeCenter
        {
            get
            { return (uint)_Descriptors[9]; }
            set
            { _Descriptors[9] = value; }
        }

        /// <summary>
        /// Change volume for bass (relative value)
        /// </summary>
        public uint RelativeVolumeChangeBass
        {
            get
            { return (uint)_Descriptors[10]; }
            set
            { _Descriptors[10] = value; }
        }

        /// <summary>
        /// Peak volume of bass
        /// </summary>
        public uint PeakVolumeBass
        {
            get
            { return (uint)_Descriptors[11]; }
            set
            { _Descriptors[11] = value; }
        }

        #endregion

        /// <summary>
        /// Indicate how many bits used for volume descripting (usually 16)
        /// </summary>
        public byte BitsForVolumeDescription
        {
            get
            {
                return _BitForVolumeDescription;
            }
            set
            {
                if (value % 8 != 0 || value > 32 || value < 8)
                    throw (new ArgumentException("Need multiple of 8 number between 8 to 32."));

                _BitForVolumeDescription = value;
            }
        }

        /// <summary>
        /// Indicate how many bytes need to descripting volume
        /// </summary>
        private int ByteFotVoulmeDescription
        {
            get
            { return (_BitForVolumeDescription / 8); }
        }

        /// <summary>
        /// Convert a uint to byte array
        /// </summary>
        /// <param name="Num">number to convert</param>
        /// <param name="ArrayLength">length of return array</param>
        /// <returns>byte array contain information of num</returns>
        private byte[] ToByteArray(uint Num, int ArrayLength)
        {
            byte[] Buf, R;
            Buf = BitConverter.GetBytes(Num);
            R = new byte[ArrayLength];
            Array.Copy(Buf, 0, R, 0, ArrayLength);
            Array.Reverse(R);
            return R;
        }

        #region -> Override Methods and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        /// <returns>int contains length of current frame</returns>
        protected override int OnGetLength()
        {
            // 1: Increment/Decrement
            // 1: BitUsedForVolumeDescription
            // 12: Volume Descriptors
            return 2 + (12 * ByteFotVoulmeDescription);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;
            int DesLength = ByteFotVoulmeDescription;

            TStream.FS.WriteByte(_IncDec); // Write Increment/Decrement
            TStream.FS.WriteByte(_BitForVolumeDescription); // Write Bits For volume descripting

            for (int i = 0; i < 12; i++)
            {
                Buf = ToByteArray((uint)_Descriptors[i], DesLength);
                TStream.FS.Write(Buf, 0, DesLength);
            }
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_Descriptors == null || _Descriptors.Length == 0)
                return false;
            return true;
        }

        #endregion
    }
}