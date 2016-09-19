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
using Macalifa.Tags.ID3.ID3v2Frames;
using System.IO;

/*
 * This namespace contain Frames that is usefull for sending and recieving
 * mpeg files over streams. ex listening to audio from internet
 */
namespace Macalifa.Tags.ID3.ID3v2Frames.StreamFrames
{
    /// <summary>
    /// A class for PositionSynchronised frame
    /// </summary>
    public class PositionSynchronisedFrame : Frame
    {
        private TimeStamps _TimeStamp;
        private long _Position;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new PositionSynchronisedFrame
        /// </summary>
        /// <param name="FrameID">FrameID for this frame</param>
        /// <param name="Flags">Frame Flags</param>
        /// <param name="Data">TagStream to read data from</param>
        /// <param name="Length">Maximum available length for this frame</param>
        public PositionSynchronisedFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            _TimeStamp = (TimeStamps)TStream.ReadByte(FS);
            if (!IsValidEnumValue(_TimeStamp, ExceptionLevels.Error, FrameID))
                return;

            Length--;

            byte[] Long = new byte[8];
            byte[] Buf = new byte[Length];

            TStream.FS.Read(Buf, 0, Length);
            Buf.CopyTo(Long, 8 - Buf.Length);
            Array.Reverse(Long);
            _Position = BitConverter.ToInt64(Long, 0);
        }

        /// <summary>
        /// Gets or sets current frame TimeStamp
        /// </summary>
        public TimeStamps TimeStamp
        {
            get
            { return _TimeStamp; }
            set
            {
                if (!Enum.IsDefined(typeof(TimeStamps), value))
                    throw (new ArgumentException("This is not valid value for TimeStamp"));

                _TimeStamp = value;
            }
        }

        /// <summary>
        /// Gets or sets current frame Position
        /// </summary>
        public long Position
        {
            get
            { return _Position; }
            set
            { _Position = value; }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            return true;
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            TStream.FS.WriteByte((byte)_TimeStamp);

            byte[] Buf;
            Buf = BitConverter.GetBytes(_Position);
            Array.Reverse(Buf);
            TStream.FS.Write(Buf, 0, 8);
        }

        /// <summary>
        /// Gets lenght of current frame
        /// </summary>
        /// <returns>int contains current frame length</returns>
        protected override int OnGetLength()
        {
            return 9;
        }

        #endregion
    }

    /// <summary>
    /// A class for RecomendedBufferSize Frame
    /// </summary>
    public class RecomendedBufferSizeFrame : Frame
    {
        private uint _BufferSize;
        private bool _EmbededInfoFlag;
        private uint _OffsetToNextTag;
        TagStreamUWP TStream;
        /// <summary>
        /// Create new RecomendedBufferSize
        /// </summary>
        /// <param name="FrameID">Characters tag identifier</param>
        /// <param name="Flags">2 Bytes flags identifier</param>
        /// <param name="Data">Contain Data for this frame</param>
        /// <param name="Length">Length to read from FileStream</param>
        public RecomendedBufferSizeFrame(string FrameID, FrameFlags Flags, int Length, Stream FS)
            : base(FrameID, Flags, FS)
        {
            TStream = new TagStreamUWP(FS);
            _BufferSize = TStream.ReadUInt(3);
            _EmbededInfoFlag = Convert.ToBoolean(TStream.ReadByte(FS));

            if (Length > 4)
                _OffsetToNextTag = TStream.ReadUInt(4);
        }

        /// <summary>
        /// Gets or Sets Buffer size for current frame
        /// </summary>
        public uint BufferSize
        {
            get
            {
                return _BufferSize;
            }
            set
            {
                if (value > 0xFFFFFF)
                    throw (new ArgumentException("Buffer size can't be greater 16,777,215(0xFFFFFF)"));

                _BufferSize = value;
            }
        }

        /// <summary>
        /// Gets or Sets current frame EmbeddedInfoFlag
        /// </summary>
        public bool EmbededInfoFlag
        {
            get { return _EmbededInfoFlag; }
            set { _EmbededInfoFlag = value; }
        }

        /// <summary>
        /// Gets or Sets Offset to next tag
        /// </summary>
        public uint OffsetToNextTag
        {
            get
            {
                return _OffsetToNextTag;
            }
            set
            {
                _OffsetToNextTag = value;
            }
        }

        #region -> Override Method and properties <-

        /// <summary>
        /// Gets length of current frame
        /// </summary>
        /// <returns>int contains length of current frame</returns>
        protected override int OnGetLength()
        {
            // 3: Buffer Size
            // 1: Info Flag
            // 4: Offset to next tag (if available)
            return 4 + (_OffsetToNextTag > 0 ? 4 : 0);
        }

        /// <summary>
        /// Writing Data to specific TagStream
        /// </summary>
        protected override void OnWritingData(int MinorVersion)
        {
            byte[] Buf;
            int Len = Length;
            var tg = TStream.FS;
            Buf = BitConverter.GetBytes(_BufferSize);
            Array.Reverse(Buf);
            tg.Write(Buf, 0, Buf.Length);

            tg.WriteByte(Convert.ToByte(_EmbededInfoFlag));

            if (_OffsetToNextTag > 0)
            {
                Buf = BitConverter.GetBytes(_OffsetToNextTag);
                Array.Reverse(Buf);
                tg.Write(Buf, 0, Buf.Length);
            }
        }

        /// <summary>
        /// Indicate if current frame data is valid
        /// </summary>
        protected override bool OnValidating()
        {
            if (_BufferSize != 0)
                return true;
            else
                return false;
        }

        #endregion
    }
}
