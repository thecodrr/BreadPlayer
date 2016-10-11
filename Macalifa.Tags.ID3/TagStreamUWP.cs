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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Macalifa.Tags.ID3;
namespace Macalifa.Tags
{
    public class TagStreamUWP
    {
        private BinaryReader _BReader;
        private BinaryWriter _BWriter;
        public Stream FS { get;set; }
        /// <summary>
        /// Create new TagStream
        /// </summary>
        /// <param name="path">Path of file to create tag on it</param>
        /// <param name="mode">FileMode for opening stream</param>
        public TagStreamUWP(System.IO.Stream FileStream)
        {
             FS = FileStream;
            _BReader = new BinaryReader(FileStream);
            _BWriter = new BinaryWriter(FileStream);
        }
        public string ReadText(int MaxLength, TextEncodings TEncoding, ref int ReadedLength, bool DetectEncoding)
        {
            if (MaxLength <= 0)
                return "";
            long Pos = this.FS.Position; // store current position

            MemoryBlockStream MStream = new MemoryBlockStream();
            if (DetectEncoding && MaxLength >= 3)
            {
                byte[] Buffer = new byte[3];
                FS.Read(Buffer, 0, Buffer.Length);
                if (Buffer[0] == 0xFF && Buffer[1] == 0xFE)
                {   // FF FE
                    TEncoding = TextEncodings.UTF_16;// UTF-16 (LE)
                    this.FS.Position--;
                    MaxLength -= 2;
                }
                else if (Buffer[0] == 0xFE && Buffer[1] == 0xFF)
                {   // FE FF
                    TEncoding = TextEncodings.UTF_16BE;
                    this.FS.Position--;
                    MaxLength -= 2;
                }
                else if (Buffer[0] == 0xEF && Buffer[1] == 0xBB && Buffer[2] == 0xBF)
                {
                    // EF BB BF
                    TEncoding = TextEncodings.UTF8;
                    MaxLength -= 3;
                }
                else
                    this.FS.Position -= 3;
            }
            // Indicate text seprator type for current string encoding
            bool Is2ByteSeprator = (TEncoding == TextEncodings.UTF_16 || TEncoding == TextEncodings.UTF_16BE);

            byte Buf, Buf2;
            while (MaxLength > 0)
            {
                if (Is2ByteSeprator)
                {
                    Buf = ReadByte(FS);
                    Buf2 = ReadByte(FS);

                    if (Buf == 0 && Buf2 == 0)
                        break;
                    else
                    {
                        MStream.WriteByte(Buf);
                        MStream.WriteByte(Buf2);
                    }

                    MaxLength--;
                }
                else
                {
                    Buf = ReadByte(FS); // Read First/Next byte from stream

                    if (Buf == 0)
                        break;
                    else
                        MStream.WriteByte(Buf);

                }

                MaxLength--;
            }

            if (MaxLength < 0)
                this.FS.Position += MaxLength;

            ReadedLength -= Convert.ToInt32(this.FS.Position - Pos);

            return StaticMethods.GetEncoding(TEncoding).GetString(MStream.ToArray());
        }
        public byte ReadByte(Stream FS)
        {
            return Convert.ToByte(FS.ReadByte());
        }
        public string ReadText(int MaxLength, TextEncodings TEncoding)
        {
            int i = 0;
            return ReadText(MaxLength, TEncoding, ref i, true);
        }
        public string ReadText(int MaxLength, TextEncodings TEncoding, bool DetectEncoding)
        {
            int i = 0;
            return ReadText(MaxLength, TEncoding, ref i, DetectEncoding);
        }
        public void WriteText(string Text, TextEncodings TEncoding, bool AddNullCharacter)
        {
            byte[] Buf;
            Buf = StaticMethods.GetEncoding(TEncoding).GetBytes(Text);
            FS.Write(Buf, 0, Buf.Length);
            if (AddNullCharacter)
            {
                FS.WriteByte(0);
                if (TEncoding == TextEncodings.UTF_16 || TEncoding == TextEncodings.UTF_16BE)
                    FS.WriteByte(0);
            }
        }
        public void WriteText(string Text, int Length)
        {
            switch (Length)
            {
                case 2:
                    this.AsBinaryWriter.Write(this.StringLength(Text));
                    break;
                case 4:
                    this.AsBinaryWriter.Write((int)this.StringLength(Text));
                    break;
                case 8:
                    this.AsBinaryWriter.Write((long)this.StringLength(Text));
                    break;
                default:
                    throw new ArgumentException("Length must be 2, 4 or 8");
            }

            if (Text.Length > 0)
                WriteText(Text, TextEncodings.UTF_16, true);
        }
        public uint ReadUInt(int Length)
        {
            if (Length > 4 || Length < 1)
                throw (new ArgumentOutOfRangeException("ReadUInt method can read 1-4 byte(s)"));

            byte[] Buf = new byte[Length];
            byte[] RBuf = new byte[4];
            FS.Read(Buf, 0, Length);
            Buf.CopyTo(RBuf, 4 - Buf.Length);
            Array.Reverse(RBuf);
            return BitConverter.ToUInt32(RBuf, 0);
        }
        public MemoryStream ReadData(int Length)
        {
            MemoryStream ms;
            byte[] Buf = new byte[Length];
            FS.Read(Buf, 0, Length);
            ms = new MemoryStream();
            ms.Write(Buf, 0, Length);

            return ms;
        }
        public Int16 StringLength(string st)
        {
            return Convert.ToInt16(st.Length > 0 ? st.Length * 2 + 2 : 0);
        }
        public BinaryReader AsBinaryReader
        {
            get
            { return _BReader; }
        }
        public BinaryWriter AsBinaryWriter
        {
            get
            { return _BWriter; }
        }
        public bool HaveID3v1()
        {
            if (this.FS.Length < 128)
                return false;
            this.FS.Seek(-128, SeekOrigin.End);
            string Tag = ReadText(3, TextEncodings.Ascii);
            return (Tag == "TAG");
        }

    }
}
