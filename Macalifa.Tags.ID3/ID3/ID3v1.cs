using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Provide a class to read and write ID3v1
    /// </summary>
    public class ID3v1
    {
        private string _FilePath;
        private string _Title;
        private string _Artist;
        private string _Album;
        private string _Year;
        private string _Comment;
        private byte _TrackNumber;
        private byte _Genre;
        private bool _HaveTag;

        private bool _IsTemplate;
        /// <summary>
        /// Indicate if current ID3v1 is template
        /// </summary>
        [DefaultValue(false)]
        public bool IsTemplate
        {
            get { return _IsTemplate; }
            set { _IsTemplate = value; }
        }
        Stream FileStream { get; set; }
        TagStreamUWP TagStream
        {
            get; set;
        }
        #region -> Public Properties <-

        /// <summary>
        /// Get file path of current ID3v1
        /// </summary>
        public string FilePath
        {
            get
            { return _FilePath; }
            internal set
            { _FilePath = value; }
        }

        /// <summary>
        /// Get file name of current ID3v1
        /// </summary>
        public string FileName
        {
            get
            { return Path.GetFileName(_FilePath); }
        }

        /// <summary>
        /// Get or set Title of current ID3v1
        /// </summary>
        public string Title
        {
            get
            { return _Title; }
            set
            {
                if (value.Length > 30)
                    throw (new ArgumentException("Title Length must be less than 30 characters"));

                _Title = value;
            }
        }

        /// <summary>
        /// Get or set Artist of current ID3v1
        /// </summary>
        public string Artist
        {
            get
            { return _Artist; }
            set
            {
                if (value.Length > 30)
                    throw (new ArgumentException("Artist Length must be less than 30 characters"));
                _Artist = value;
            }
        }

        /// <summary>
        /// Get or set Album of current ID3v1
        /// </summary>
        public string Album
        {
            get
            { return _Album; }
            set
            {
                if (value.Length > 30)
                    throw (new ArgumentException("Album Length must be less than 30 characters"));

                _Album = value;
            }
        }

        /// <summary>
        /// Get or set Year of current ID3v1
        /// </summary>
        public string Year
        {
            get
            { return _Year; }
            set
            {
                if (value.Length > 4)
                    throw (new ArgumentException("Year Length must be less than 4 characters"));
                _Year = value;
            }
        }

        /// <summary>
        /// Get or set Comment of current ID3v1
        /// </summary>
        public string Comment
        {
            get
            { return _Comment; }
            set
            {
                if (value.Length > 28)
                    throw (new ArgumentException("Comment Length must be less than 4 characters"));
                _Comment = value;
            }
        }

        /// <summary>
        /// Get or set TrackNumber of current ID3v1
        /// </summary>
        public byte TrackNumber
        {
            get
            { return _TrackNumber; }
            set
            { _TrackNumber = value; }
        }

        /// <summary>
        /// Get or set Genre of current ID3v1
        /// </summary>
        public byte Genre
        {
            get
            { return _Genre; }
            set
            { _Genre = value; }
        }

        /// <summary>
        /// Indicate if current File contain ID3v1 Information
        /// </summary>
        public bool HaveTag
        {
            get
            { return _HaveTag; }
            set
            { _HaveTag = value; }
        }

        #endregion

        /// <summary>
        /// Create new ID3v1 class
        /// </summary>
        /// <param name="FilePath">Path of file</param>
        /// <param name="LoadData">Indicate load data in constructor or not</param>
        public ID3v1(bool LoadData, Stream TS)
        {
            FileStream = TS;
            TagStream = new TagStreamUWP(FileStream);
            _Title = "";
            _Artist = "";
            _Album = "";
            _Year = "";
            _Comment = "";
            _TrackNumber = 0;
            _Genre = 255;
            _HaveTag = false;

            if (LoadData)
                Load();
        }

        /// <summary>
        /// Load ID3v1 information from file
        /// </summary>
        public void Load()
        {
            if (!TagStream.HaveID3v1()) // HaveID3v1 go to beginning of ID3v1 if exist
            {
                FileStream.Dispose();
                _HaveTag = false;
                return;
            }
            _Title = TagStream.ReadText(30, TextEncodings.Ascii);
            FileStream.Seek(-95, SeekOrigin.End);
            _Artist = TagStream.ReadText(30, TextEncodings.Ascii);
            FileStream.Seek(-65, SeekOrigin.End);
            _Album = TagStream.ReadText(30, TextEncodings.Ascii);
            FileStream.Seek(-35, SeekOrigin.End);
            _Year = TagStream.ReadText(4, TextEncodings.Ascii);
            FileStream.Seek(-31, SeekOrigin.End);
            _Comment = TagStream.ReadText(28, TextEncodings.Ascii);
            FileStream.Seek(-2, SeekOrigin.End);
           // _TrackNumber = TagStream.ReadByte(FileStream);
           // _Genre = TagStream.ReadByte(FileStream);
            //FileStream.Dispose();
            _HaveTag = true;

        }

        /// <summary>
        /// Save ID3v1 information to file
        /// </summary>
        public void Save()
        {
            bool HTag = TagStream.HaveID3v1();
            if (HTag && !_HaveTag) // just delete ID3
            {
                (FileStream).SetLength(FileStream.Length - 128);
                Comment += "HOLLA11";
            }
            else if (!HTag && _HaveTag)
            {
                Comment += "HOLLA";
                FileStream.Seek(0, SeekOrigin.End);
                FileStream.Write(GetTagBytes, 0, 128);
            }
            else if (HTag && _HaveTag)
            {             
                FileStream.Seek(-128, SeekOrigin.End);
                FileStream.Write(GetTagBytes, 0, 128);
            }
            FileStream.Dispose();
        }
        public string Test { get; set; }
        /// <summary>
        /// Clear all values
        /// </summary>
        public void ClearAll()
        {
            this.Album = "";
            this.Artist = "";
            this.Comment = "";
            this.Genre = 255;
            this.Title = "";
            this.TrackNumber = 0;
            this.Year = "";
        }

        /// <summary>
        /// Convert data tot Byte Array to write to file
        /// </summary>
        private byte[] GetTagBytes
        {
            get
            {
                byte[] Buf = new byte[128];
                Array.Clear(Buf, 0, 128);

                Encoding.ASCII.GetBytes("TAG").CopyTo(Buf, 0);
                Encoding.ASCII.GetBytes(_Title).CopyTo(Buf, 3);
                Encoding.ASCII.GetBytes(_Artist).CopyTo(Buf, 33);
                Encoding.ASCII.GetBytes(_Album).CopyTo(Buf, 63);
                Encoding.ASCII.GetBytes(_Year).CopyTo(Buf, 93);
                Encoding.ASCII.GetBytes(_Comment).CopyTo(Buf, 97);
                Buf[126] = _TrackNumber;
                Buf[127] = _Genre;
                return Buf;
            }
        }
    }
}
