using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Macalifa.Tags.ID3.ID3v2Frames;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
using Macalifa.Tags.ID3.ID3v2Frames.ArrayFrames;
using System.Diagnostics;
using System.Reflection;
using Macalifa.Tags.ID3.ID3v2Frames.BinaryFrames;
namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Provide a class to read and write ID3v2 information of files
    /// </summary>
    public class ID3v2
    {
        #region -> Variables <-

        private static bool _AutoTextEncoding = true; // when want to save frame use automatic text encoding
        private static TextEncodings _DefaultUnicodeEncoding = TextEncodings.UTF_16; // when use AutoTextEncoding which unicode type must use
        private bool _DropUnknown; // if true. unknown frames will not save
        private FilterCollection _Filter; // Contain Filter Frames
        private int _OriginID3Length; // ID3 Length of file on disk
        private ID3v2HeaderFlags _HeaderFlags;
        private string _FilePath; // ID3 file path        
        private FilterTypes _FilterType; //Indicate wich filter type use
        private bool _LoadLinkedFrames; // Indicate load Link frames when loading ID3 or not
        private Version _Version; // Contain ID3 version information
        private bool _HaveTag; // Indicate if current file have ID3v2 Info
        private ExceptionCollection _Errors; // Contain Errors that occured

        private Hashtable _CollectionFrames; // Contain FrameCollections for frames that can occur more than one time
        private Hashtable _SingleFrames; // contain frames that maximum can occur one time

        #endregion

        private enum CollectionIndex
        {
            Text,
            UserText,
            Private,
            TextWithLanguage,
            SynchronisedText,
            AttachedPicture,
            EncapsulatedObject,
            Popularimeter,
            AudioEncryption,
            Link,
            TermOfUse,
            DataWithSymbol,
            Unknown
        }

        /// <summary>
        /// Create new ID3v2 class for specific file
        /// </summary>
        /// <param name="FilePath">FileAddress to read ID3 information from</param>
        /// <param name="LoadData">Indicate load ID3 in constructor or not</param>
        public ID3v2(bool LoadData, Stream FS)
        {
            // ------ Set default values -----------
            _LoadLinkedFrames = true;
            _DropUnknown = false;            

            Initializer();
            TStream = new Macalifa.Tags.TagStreamUWP(FS);
            if (LoadData == true)
                Load();
        }
        Macalifa.Tags.TagStreamUWP TStream;
        private void Initializer()
        {
            _Filter = new FilterCollection();
            
            _CollectionFrames = new Hashtable();
            _SingleFrames = new Hashtable();

            _FilterType = FilterTypes.NoFilter;
            _Errors = new ExceptionCollection();


            FrameCollection<TextFrame> TextFrames = new FrameCollection<TextFrame>(CollectionIndex.Text.ToString());
            FrameCollection<UserTextFrame> UserTextFrames = new FrameCollection<UserTextFrame>(CollectionIndex.UserText.ToString());
            FrameCollection<PrivateFrame> PrivateFrames = new FrameCollection<PrivateFrame>(CollectionIndex.Private.ToString());
            FrameCollection<TextWithLanguageFrame> TextWithLangFrames = new FrameCollection<TextWithLanguageFrame>(CollectionIndex.TextWithLanguage.ToString());
            FrameCollection<SynchronisedText> SynchronisedTextFrames = new FrameCollection<SynchronisedText>(CollectionIndex.SynchronisedText.ToString());
            FrameCollection<AttachedPictureFrame> AttachedPictureFrames = new FrameCollection<AttachedPictureFrame>(CollectionIndex.AttachedPicture.ToString());
            FrameCollection<GeneralFileFrame> EncapsulatedObjectFrames = new FrameCollection<GeneralFileFrame>(CollectionIndex.EncapsulatedObject.ToString());
            FrameCollection<PopularimeterFrame> PopularimeterFrames = new FrameCollection<PopularimeterFrame>(CollectionIndex.Popularimeter.ToString());
            FrameCollection<AudioEncryptionFrame> AudioEncryptionFrames = new FrameCollection<AudioEncryptionFrame>(CollectionIndex.AudioEncryption.ToString());
            FrameCollection<LinkFrame> LinkFrames = new FrameCollection<LinkFrame>(CollectionIndex.Link.ToString());
            FrameCollection<TermOfUseFrame> TermOfUseFrames = new FrameCollection<TermOfUseFrame>(CollectionIndex.TermOfUse.ToString());
            FrameCollection<DataWithSymbolFrame> DataWithSymbolFrames = new FrameCollection<DataWithSymbolFrame>(CollectionIndex.DataWithSymbol.ToString());
            FrameCollection<BinaryFrame> UnknownFrames = new FrameCollection<BinaryFrame>(CollectionIndex.Unknown.ToString());

            _CollectionFrames.Add(CollectionIndex.Text, TextFrames);
            _CollectionFrames.Add(CollectionIndex.UserText, UserTextFrames);
            _CollectionFrames.Add(CollectionIndex.Private, PrivateFrames);
            _CollectionFrames.Add(CollectionIndex.TextWithLanguage, TextWithLangFrames);
            _CollectionFrames.Add(CollectionIndex.SynchronisedText, SynchronisedTextFrames);
            _CollectionFrames.Add(CollectionIndex.AttachedPicture, AttachedPictureFrames);
            _CollectionFrames.Add(CollectionIndex.EncapsulatedObject, EncapsulatedObjectFrames);
            _CollectionFrames.Add(CollectionIndex.Popularimeter, PopularimeterFrames);
            _CollectionFrames.Add(CollectionIndex.AudioEncryption, AudioEncryptionFrames);
            _CollectionFrames.Add(CollectionIndex.Link, LinkFrames);
            _CollectionFrames.Add(CollectionIndex.TermOfUse, TermOfUseFrames);
            _CollectionFrames.Add(CollectionIndex.DataWithSymbol, DataWithSymbolFrames);
            _CollectionFrames.Add(CollectionIndex.Unknown, UnknownFrames);

            Version = new Version(2, 3, 0, 0);
        }

        #region -> File Name Methods <-

        private string GetFilePath(string Formula)
        {
            if (Formula == string.Empty)
                return FilePath;

            return Path.Combine(Path.GetDirectoryName(_FilePath), MakeFileName(Formula));
        }

        /// <summary>
        /// Get FileName according to specific formula
        /// </summary>
        /// <param name="Formula">Formula to make FileName</param>
        /// <returns>System.String contain FileName according to formula or String.Empty</returns>
        public string MakeFileName(string Formula)
        {
            string FileName = "";

            Formula = Formula.Replace("<", "<;");
            string ID;
            foreach (string St in Formula.Split('>', '<'))
            {
                if (St.StartsWith(";"))
                {
                    ID = St.Remove(0, 1).ToUpper();
                    if (ID.StartsWith("TRCK"))
                    {
                        string TRCK = TrackNumber;
                        if (ID.Length == 5)
                        {
                            int Digits = int.Parse(ID[4].ToString());

                            while (Digits-- > TrackNumber.Length)
                            {
                                FileName += "0";
                            }
                        }
                        FileName += TRCK;
                    }
                    else
                        FileName += GetTextFrame(ID);
                }
                else
                    FileName += St;
            }

            return FileName + ".mp3";
        }

        /// <summary>
        /// Make a temp file name for specific filename
        /// </summary>
        /// <param name="FileName">Filename to make temp name</param>
        /// <returns>string contain Temp FileName</returns>
        private string MakeTempFilePath(string FileName)
        {
            FileName += "~Temp";

            // Make sure that file name doesn't exists
            int counter = 0;
            string TempName = FileName;
            while (File.Exists(TempName))
                TempName = FileName + (counter++).ToString();

            return TempName;
        }

        #endregion

        /// <summary>
        /// Get TrackNumber for renaming
        /// </summary>
        private string TrackNumber
        {
            get
            {
                string Track = GetTextFrame("TRCK");
                int i = Track.IndexOf('/');
                if (i != -1)
                    Track = Track.Substring(0, i);
                return Track;
            }
        }

        /// <summary>
        /// Add specific ID3Error to ErrorCollection
        /// </summary>
        /// <param name="Error">Error to add</param>
        private void AddError(Exception Error)
        {
            _Errors.Add(Error);
        }

        private int OriginAudioPosition
        {
            get
            { return (_OriginID3Length > 0) ? _OriginID3Length + 11 : 0; }
        }

        private bool _IsTemplate;
        /// <summary>
        /// Indicate if current ID3v2 is template not a real file
        /// </summary>
        public bool IsTemplate
        {
            get { return _IsTemplate; }
            set { _IsTemplate = value; }
        }

        #region -> Public Properties <-

        /// <summary>
        /// Gets Collection of Errors that occured
        /// </summary>
        public ExceptionCollection Errors
        {
            get
            { return _Errors; }
        }

        /// <summary>
        /// Gets FileAddress of current ID3v2
        /// </summary>
        public string FilePath
        {
            get
            { return _FilePath; }
            private set
            { _FilePath = value; }
        }

        /// <summary>
        /// Get FileName of current ID3v2
        /// </summary>
        public string FileName
        {
            get
            { return Path.GetFileName(_FilePath); }
        }

        /// <summary>
        /// Get Filter of current frame
        /// </summary>
        public FilterCollection Filter
        {
            get
            { return _Filter; }
        }

        /// <summary>
        /// Gets or Sets current Tag filter type
        /// </summary>
        public FilterTypes FilterType
        {
            get
            { return _FilterType; }
            set
            { _FilterType = value; }
        }

        /// <summary>
        /// Indicate load Linked frames info while loading Tag
        /// </summary>
        public bool LoadLinkedFrames
        {
            get
            { return _LoadLinkedFrames; }
            set
            { _LoadLinkedFrames = value; }
        }

        /// <summary>
        /// Indicate drop unknown frame while saving ID3 or not
        /// </summary>
        public bool DropUnknowFrames
        {
            get
            { return _DropUnknown; }
            set
            { _DropUnknown = value; }
        }

        /// <summary>
        /// Get or Set version of current ID3 Tag
        /// </summary>
        public System.Version Version
        {
            get
            { return _Version; }
            set
            {
                if (value.Major != 2)
                    throw new ArgumentOutOfRangeException("Major", "Major version for this application is always 2");

                if (value.Minor < 3 || value.Minor > 4)
                    throw new ArgumentOutOfRangeException("Minor", "Minor Version for this application can be 3 or 4");

                if (value.Build > 255)
                    throw new ArgumentOutOfRangeException("Build", "Build can have maximum value of 255");

                _Version = value;
            }
        }

        /// <summary>
        /// Indicate if current file have ID3v2 Information
        /// </summary>
        public bool HaveTag
        {
            get
            { return _HaveTag; }
            set
            {
                if (_HaveTag == true && value == false)
                    ClearAll();

                _HaveTag = value;
            }
        }

        /// <summary>
        /// Get length of current ID3 Tag
        /// </summary>
        public int Length
        {
            get
            {
                int RLen = 0;
                foreach (FrameCollectionBase Coll in _CollectionFrames.Values)
                {
                    if (Coll.Name != CollectionIndex.Unknown.ToString() ||
                        (Coll.Name == CollectionIndex.Unknown.ToString() && !_DropUnknown))
                    {
                        foreach (Frame F in Coll)
                            if (F.IsValid)
                                RLen += F.Length + 10;
                    }
                }

                foreach (Frame Fr in _SingleFrames.Values)
                    if (Fr.IsValid)
                        RLen += Fr.Length + 10;

                return RLen;
            }
        }

        /// <summary>
        /// Indicate if current ID3Info had error while openning
        /// </summary>
        public bool HaveError
        {
            get
            {
                if (_Errors.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets or sets unicode encoding of AutoTextEncoding
        /// </summary>
        public static TextEncodings DefaultUnicodeEncoding
        {
            get
            { return _DefaultUnicodeEncoding; }
            set
            {
                if ((int)value > 3 || (int)value < 2)
                    throw (new ArgumentOutOfRangeException("Default unicode must be one of (UTF_16, UTF_16BE, UTF_8)"));

                _DefaultUnicodeEncoding = value;
            }
        }

        /// <summary>
        /// Indicate while saving automatically detect encoding of texts of not
        /// </summary>
        public static bool AutoTextEncoding
        {
            get
            { return _AutoTextEncoding; }
            set
            { _AutoTextEncoding = value; }
        }

        /// <summary>
        /// Indicate if current ID3v2 is experimental
        /// </summary>
        public bool Experimental
        {
            get
            { return (_HeaderFlags & ID3v2HeaderFlags.Experimental) == ID3v2HeaderFlags.Experimental; }
            set
            {
                if (value)
                    _HeaderFlags |= ID3v2HeaderFlags.Experimental;
                else
                    _HeaderFlags &= ~ID3v2HeaderFlags.Experimental;
            }
        }

        /// <summary>
        /// Indicate if current ID3v2 is Unsychronized
        /// </summary>
        public bool Unsynchronisation
        {
            get
            { return (_HeaderFlags & ID3v2HeaderFlags.Unsynchronisation) == ID3v2HeaderFlags.Unsynchronisation; }
            set
            {
                if (value)
                    _HeaderFlags |= ID3v2HeaderFlags.Unsynchronisation;
                else
                    _HeaderFlags &= ~ID3v2HeaderFlags.Unsynchronisation;
            }
        }

        /// <summary>
        /// Indicate if current ID3v2 contains Extended header
        /// </summary>
        public bool ExtendedHeader
        {
            get
            { return (_HeaderFlags & ID3v2HeaderFlags.ExtendedHeader) == ID3v2HeaderFlags.ExtendedHeader; }
        }

        #endregion

        #region -> Load Methods <-

        /// <summary>
        /// Load ID3 information from file
        /// </summary>
        /// <exception cref="FileNotFoundException">File Not Found</exception>
        public void Load()
        {
            Errors.Clear();
            ClearAll();
            
            ReadHeader();

            if (!HaveTag) // If file don't contain ID3v2 exit function
            {
                return;
            }

            ReadFrames(_OriginID3Length);
        }

        /// <summary>
        /// Load all linked information frames
        /// </summary>
        public void LoadAllLinkedFrames()
        {
            foreach (LinkFrame LF in LinkFrames)
                LoadFrameFromFile(LF.FrameIdentifier, LF.URL);
        }

        /// <summary>
        /// Load spefic frame information
        /// </summary>
        /// <param name="FrameID">FrameID to load</param>
        /// <param name="FileAddress">FileAddress to read tag from</param>
        private void LoadFrameFromFile(string FrameID, string FileAddress)
        {
            ID3v2 LinkedInfo = new ID3v2(false, TStream.FS);
            LinkedInfo.Filter.Add(FrameID);
            LinkedInfo.FilterType = FilterTypes.LoadFiltersOnly;
            LinkedInfo.Load();

            if (LinkedInfo.HaveError)
                foreach (ID3Exception IE in LinkedInfo.Errors)
                    _Errors.Add(new ID3Exception("In Linked Info(" +
                        FileAddress + "): " + IE.Message, IE.FrameID, IE.Level));

            foreach (FrameCollectionBase Coll in LinkedInfo._CollectionFrames)
            {
                if (Coll.Name == CollectionIndex.Link.ToString())
                {

                    continue;
                }

                foreach (Frame Fr in Coll)
                {
                    FrameCollection<Frame> Temp =
                        (FrameCollection<Frame>)_CollectionFrames[
                        Enum.Parse(typeof(CollectionIndex), Coll.Name)];

                    Temp.Add(Fr);
                }
            }

            foreach (Frame In in (Frame[])LinkedInfo._SingleFrames.Values)
            {
                if (_SingleFrames.ContainsKey(In.FrameID))
                    _SingleFrames.Remove(In);

                _SingleFrames.Add(In.FrameID, LinkedInfo._SingleFrames[In]);
            }
        }

        #endregion

        #region -> Save Methods <-

        /// <summary>
        /// Save ID3v2 data without renaming file with minor version of 3
        /// </summary>
        public void Save()
        {
            Save("");
        }

        /// <summary>
        /// Save ID3 info to file
        /// </summary>
        /// <param name="Formula">Formula to renaming file</param>
        public void Save(string Formula)
        {
            SaveAs(GetFilePath(Formula));
        }

        /// <summary>
        /// Save Current ID3v2 to specific location
        /// </summary>
        /// <param name="NewFilePath">Path to save file</param>
        public void SaveAs(string NewFilePath)
        {
            //try
            //{
            //    string TempPath = MakeTempFilePath(NewFilePath);

            //    using (TagStream writer = new TagStream(TempPath, FileMode.Create))
            //    {
            //        int OriginLength;
            //        if (!_HaveTag)
            //        {
            //            AppendFromOriginalFile(writer);
            //            OriginLength = 0;
            //        }
            //        else
            //        {
            //            OriginLength = WriteHeader();
            //            WriteFrames(Version.Minor);
            //            if (!IsTemplate)
            //                AppendFromOriginalFile(writer);
            //        }
            //        // if Orginal file and current file both don't contain ID3
            //        // we don't need to do anything

            //        writer.Dispose();

            //        _OriginID3Length = OriginLength;
            //    }

            //    TagStream.DeleteRename(FilePath, TempPath, NewFilePath);
            //    FilePath = NewFilePath;
            //}
            //catch (Exception Ex)
            //{
            //    AddError(Ex);
            //    throw Ex;
            //}
        }

        /// <summary>
        /// Write all frames to specific TagStream
        /// </summary>
        /// <param name="writer">TagStream to write data to</param>
        /// <param name="Ver">Minor Version of ID3</param>
        private void WriteFrames(int Ver)
        {
            foreach (FrameCollectionBase Coll in _CollectionFrames.Values)
                if (Coll.Name != CollectionIndex.Unknown.ToString() ||
                       (Coll.Name == CollectionIndex.Unknown.ToString() && !_DropUnknown))
                    foreach (Frame Fr in Coll)
                    {
                        // If Frame is not valid and is not UserTextFrame we ignore it
                        if (!FramesInfo.IsCompatible(Fr.FrameID, Ver) && FramesInfo.IsTextFrame(Fr.FrameID, Ver) != 2)
                        {
                            AddError(new ID3Exception("nonCompatible Frame found on Frames and will not save with file", Fr.FrameID, ExceptionLevels.Warning));
                            continue;
                        }

                        if (Fr.IsValid)
                            Fr.WriteData(Ver);
                    }

            foreach (Frame Fr in _SingleFrames.Values)
                if (FramesInfo.IsCompatible(Fr.FrameID, Ver) && Fr.IsValid)
                    Fr.WriteData(Ver);
        }

        /// <summary>
        /// Append a file to another from start position
        /// </summary>
        /// <param name="writer">File that data must append to it</param>
        private void AppendFromOriginalFile(FileStream writer)
        {
            if (IsTemplate)
                return;

            FileStream reader = new FileStream(FilePath, FileMode.Open);
            reader.Seek(OriginAudioPosition, SeekOrigin.Begin);

            byte[] Buf = new byte[reader.Length - OriginAudioPosition];
            reader.Read(Buf, 0, Buf.Length);
            writer.Write(Buf, 0, Buf.Length);

            reader.Dispose();
        }

        #endregion

        #region -> Public Methods <-

        /// <summary>
        /// Search TextFrames for specific FrameID
        /// </summary>
        /// <param name="FrameID">FrameID to search in TextFrames</param>
        /// <returns>TextFrame according to FrameID</returns>
        public string GetTextFrame(string FrameID)
        {
            foreach (TextFrame TF in TextFrames)
                if (TF.FrameID == FrameID)
                    return TF.Text;

            return "";
        }

        /// <summary>
        /// Set text of specific TextFrame
        /// </summary>
        /// <param name="FrameID">FrameID</param>
        /// <param name="Text">Text to set</param>
        public void SetTextFrame(string FrameID, string Text)
        {
            if (!FramesInfo.IsValidFrameID(FrameID))
                return;

            TextFrame[] TF = TextFrames.ToArray();
            for (int i = 0; i < TextFrames.Count - 1; i++)
            {
                if (TF[i].FrameID == FrameID)
                {
                    TextFrames.RemoveAt(i);
                    break;
                }
            }

            if (Text != "")
            {
                HaveTag = true;
                TextFrames.Add(new TextFrame(FrameID, new FrameFlags(),
                    Text, (StaticMethods.IsAscii(Text) ? TextEncodings.Ascii : _DefaultUnicodeEncoding),
                    _Version.Minor, TStream.FS));
            }
        }

        /// <summary>
        /// Clear all ID3 Tag information
        /// </summary>
        public void ClearAll()
        {
            foreach (CollectionBase Coll in _CollectionFrames.Values)
                Coll.Clear();

            _SingleFrames.Clear();

            _HaveTag = false;
        }

        #endregion

        #region -> Private 'Read Methods' <-

        /// <summary>
        /// Read all frames from specific FileStream
        /// </summary>
        /// <param name="Data">FileStream to read data from</param>
        /// <param name="Length">Length of data to read from FileStream</param>
        private void ReadFrames(int Length)
        {
            string FrameID;
            int FrameLength;
            FrameFlags Flags = new FrameFlags();
            byte Buf;
            // If ID3v2 is ID3v2.2 FrameID, FrameLength of Frames is 3 byte
            // otherwise it's 4 character
            int FrameIDLen = Version.Minor == 2 ? 3 : 4;

            // Minimum frame size is 10 because frame header is 10 byte
            while (Length > 10)
            {
                // check for padding( 00 bytes )
                Buf = TStream.ReadByte(TStream.FS);
                if (Buf == 0)
                {
                    Length--;
                    continue;
                }

                // if readed byte is not zero. it must read as FrameID
                TStream.FS.Seek(-1, SeekOrigin.Current);

                // ---------- Read Frame Header -----------------------
                FrameID = TStream.ReadText(FrameIDLen, TextEncodings.Ascii);
                if (FrameIDLen == 3)
                    FrameID = FramesInfo.Get4CharID(FrameID);
                try
                {
                    var len = Convert.ToInt32(TStream.ReadUInt(FrameIDLen));
                    FrameLength = len;
                }
                catch{ FrameLength = 0; }
                if (FrameIDLen == 4)
                    Flags = (FrameFlags)TStream.ReadUInt(2);
                else
                    Flags = 0; // must set to default flag

                long Position = TStream.FS.Position;

                if (Length > 0x10000000)
                    throw (new FileLoadException("This file contain frame that have more than 256MB data. This is not valid for ID3."));

                bool Added = false;
                if (IsAddable(FrameID)) // Check if frame is not filter
                    Added = AddFrame(FrameID, FrameLength, Flags, TStream.FS);

                if (!Added)
                    // if don't read this frame
                    // we must go forward to read next frame
                    TStream.FS.Position = Position + FrameLength;

                Length -= FrameLength + 10;
                // 10 for Frame Header header
            }
        }

        /// <summary>
        /// Indicate is specific FrameID filtered or not
        /// </summary>
        /// <param name="FrameID">FrameID to check</param>
        /// <returns>true if can add otherwise false</returns>
        private bool IsAddable(string FrameID)
        {
            if (_FilterType == FilterTypes.NoFilter)
                return true;
            else if (_FilterType == FilterTypes.LoadFiltersOnly)
                return _Filter.IsExists(FrameID);
            else // Not Load Filters
                return !_Filter.IsExists(FrameID);
        }

        /// <summary>
        /// Add Frame information to where it must store
        /// </summary>
        /// <param name="Data">FileStream contain Frame</param>
        /// <param name="FrameID">FrameID of frame</param>
        /// <param name="Length">Maximum available length to read</param>
        /// <param name="Flags">Flags of frame</param>
        private bool AddFrame(string FrameID, int Length, FrameFlags Flags, Stream FS)
        {
            // NOTE: All FrameIDs must be capital letters
            if (!FramesInfo.IsValidFrameID(FrameID))
            {
                AddError(new ID3Exception("nonValid Frame found and dropped", FrameID, ExceptionLevels.Repaired));
                return false;
            }

            int IsText = FramesInfo.IsTextFrame(FrameID, _Version.Minor);
            if (IsText == 1)
            {
                TextFrame TempTextFrame = new TextFrame(FrameID, Flags, Length, FS);
                if (TempTextFrame.IsValid)
                {
                    TextFrames.Add(TempTextFrame);
                    return true;
                }
                return false;
            }
            else if (IsText == 2)
            {
                UserTextFrame TempUserTextFrame = new UserTextFrame(FrameID, Flags, Length, FS);
                if (TempUserTextFrame.IsValid)
                {
                    UserTextFrames.Add(TempUserTextFrame);
                    return true;
                }
                return false;
            }
            else if (FrameID == "LINK")
            {
                LinkFrame LF = new LinkFrame(FrameID, Flags, Length, FS);
                if (LF.IsValid)
                {
                    LinkFrames.Add(LF);
                    if (_LoadLinkedFrames)
                    { LoadFrameFromFile(LF.FrameIdentifier, LF.URL); return true; }
                }
                else
                    AddError(LF.Exception);
            }

            Frame F = null;
            if (FrameID != "RGAD" && FrameID != "NCON")
            {
                FrameInfo Info = FramesInfo.GetFrame(FrameID);

                if (Info == null || Info.ClassType == null)
                {
                    AddError(new ID3Exception("Unknown Frame found and dropped according to setting", FrameID, ExceptionLevels.Warning));
                    return true;
                }

                try
                {
                    F = Info.Constuctor(FrameID, Flags, Length, TStream.FS);
                }
                catch { }
                if (F.IsValid)
                {
                    if (Info.IsSingle)
                    {
                        if (_SingleFrames.Contains(FrameID))
                            _SingleFrames.Remove(FrameID);

                        _SingleFrames.Add(FrameID, F);
                        return true;
                    }
                    else
                        foreach (FrameCollectionBase Coll in _CollectionFrames.Values)
                        {
                            if (Coll.CollectionType == Info.ClassType)
                            {
                                Coll.Remove(F);
                                Coll.Add(F);
                                return true;
                            }
                        }
                    AddError(new ID3Exception("ClassType not found in Collection list", FrameID, ExceptionLevels.Error));
                }
                else if (F.Exception != null)
                    AddError(F.Exception);

            }

            return false;
        }
       
        //private bool ReadUnknownFrame(string FrameID, FrameFlags Flags, TagStream Data, int Length)
        //{
        //    BinaryFrame Unknown = new BinaryFrame(FrameID, Flags, Data, Length);
        //    if (Unknown.IsValid)
        //    {
        //        UnKnownFrames.Add(Unknown);
        //        return true;
        //    }
        //    else
        //        AddError(Unknown.Exception);
        //    return false;
        //}

        #endregion

        #region -> Header Methods <-

        private void ReadHeader()
        {
            TStream.FS.Seek(0, SeekOrigin.Begin);
            if (TStream.ReadText(3, TextEncodings.Ascii) != "ID3")
            {
                _OriginID3Length = 0;
                _HaveTag = false;
                return;
            }

            ReadVersion(); // Read ID3 Version
            ReadHeaderFlags(); // Read header flags
            _OriginID3Length = ReadID3Length();

            if (ExtendedHeader)
            {
                _Errors.Add(new ID3Exception("This file contain Extended Header. This application ignore Extended header", ExceptionLevels.Error));

                TStream.FS.Seek(TStream.ReadUInt(4) + 6, SeekOrigin.Current); // Ignore Extended Frame
            }

            _HaveTag = true;
        }

        private void ReadVersion()
        {
            Version VerInfo = new Version("2." + TStream.ReadByte(TStream.FS).ToString() + "." +
                TStream.ReadByte(TStream.FS).ToString());

            if (VerInfo.Minor > 4)
                _Errors.Add(new ID3Exception("ID3v" + _Version.ToString() +
                    " is higher than this application supporting," +
                    "but try to load it", ExceptionLevels.Warning));

            if (VerInfo.Minor < 2)
                throw new NotSupportedException("ID3v" + _Version.ToString() +
                    " is not supported by this application");

            _Version = VerInfo;
        }

        private void ReadHeaderFlags()
        {
            _HeaderFlags = (ID3v2HeaderFlags)TStream.ReadByte(TStream.FS);

            if ((_HeaderFlags & ID3v2HeaderFlags.Experimental) == ID3v2HeaderFlags.Experimental)
                _Errors.Add(new ID3Exception("This file contain Experimental ID3", ExceptionLevels.Warning));
        }

        private int WriteHeader()
        {
            TStream.WriteText("ID3", TextEncodings.Ascii, false);

            TStream.FS.WriteByte(Convert.ToByte(_Version.Minor));
            TStream.FS.WriteByte(Convert.ToByte(_Version.Build));

            TStream.FS.WriteByte((byte)_HeaderFlags);

            byte[] Buf = new byte[4];
            int Len = Length;
            int TOrginLength = Len;
            for (int i = 3; i >= 0; i--)
            {
                Buf[i] = Convert.ToByte(Len % 0x80);
                Len /= 0x80;
            }
            TStream.FS.Write(Buf, 0, 4);

            return TOrginLength;
        }

        private int ReadID3Length()
        {
            /* ID3 Size is like:
             * 0XXXXXXXb 0XXXXXXXb 0XXXXXXXb 0XXXXXXXb (b means binary)
             * the zero bytes must ignore, so we have 28 bits number = 0x1000 0000 (maximum)
             * it's equal to 256MB
             */
            int RInt = 0;
            int Mul;
            byte Buf;
            for (Mul = 0x200000; Mul >= 1; Mul /= 0x80)
            {
                Buf = TStream.ReadByte(TStream.FS);
                if (Buf > 0x80)
                    throw new DataMisalignedException(Buf.ToString() + " is invalid for ID3 byte size");

                RInt += Buf * Mul;
            }

            return RInt;
        }

        #endregion

        #region -> Public 'Single Time Frames Properties' <-

        private void SetSingleValue(string Key, Frame Value)
        {
            if (Value == null)
                _SingleFrames.Remove(Key);
            else
            {
                if (_SingleFrames.ContainsKey(Key))
                    _SingleFrames[Key] = Value;
                else
                    _SingleFrames.Add(Key, Value);
            }
        }

        private Frame GetSingleValue(string Key)
        {
            return (Frame)_SingleFrames[Key];
        }

        /// <summary>
        /// Get MusicCDIdentifier of current ID3
        /// </summary>
   

        /// <summary>
        /// Get SynchronisedTempoCodes of current ID3
        /// </summary>
        public SynchronisedTempoFrame SynchronisedTempoCodes
        {
            get
            { return (SynchronisedTempoFrame)_SingleFrames["SYTC"]; }
            set
            { SetSingleValue("SYTC", value); }
        }

   
        /// <summary>
        /// Get OwnerShip of current ID3
        /// </summary>
        public OwnershipFrame OwnerShip
        {
            get
            { return (OwnershipFrame)_SingleFrames["OWNE"]; }
            set
            { SetSingleValue("OWNE", value); }
        }
        
        /// <summary>
        /// Get Equalisations of current ID3
        /// </summary>
        public Equalisation Equalisations
        {
            get
            { return (Equalisation)_SingleFrames["EQUA"]; }
            set
            { SetSingleValue("EQUA", value); }
        }
        

        /// <summary>
        /// Get EventTimingCode of current ID3
        /// </summary>
        public EventTimingCodeFrame EventTimingCode
        {
            get
            { return (EventTimingCodeFrame)_SingleFrames["ETCO"]; }
            set
            { SetSingleValue("ETCO", value); }
        }

        #endregion

        #region -> Public 'Collection Properties' <-

        /// <summary>
        /// Get TextFrame Collection of current ID3
        /// </summary>
        public FrameCollection<TextFrame> TextFrames
        {
            get
            { return (FrameCollection<TextFrame>)_CollectionFrames[CollectionIndex.Text]; }
        }

        /// <summary>
        /// Get UserTextFrame Collection of current ID3
        /// </summary>
        public FrameCollection<UserTextFrame> UserTextFrames
        {
            get { return (FrameCollection<UserTextFrame>)_CollectionFrames[CollectionIndex.UserText]; }
        }

        /// <summary>
        /// Get PrivateFrame Collection of current ID3
        /// </summary>
        public FrameCollection<PrivateFrame> PrivateFrames
        {
            get { return (FrameCollection<PrivateFrame>)_CollectionFrames[CollectionIndex.Private]; }
        }

        /// <summary>
        /// Get TextWithLanguageFrame Collection of current ID3
        /// </summary>
        public FrameCollection<TextWithLanguageFrame> TextWithLanguageFrames
        {
            get { return (FrameCollection<TextWithLanguageFrame>)_CollectionFrames[CollectionIndex.TextWithLanguage]; }
        }

        /// <summary>
        /// Get SynchronisedText Collection of current ID3
        /// </summary>
        public FrameCollection<SynchronisedText> SynchronisedTextFrames
        {
            get { return (FrameCollection<SynchronisedText>)_CollectionFrames[CollectionIndex.SynchronisedText]; }
        }

        /// <summary>
        /// Get AttachedPictureFrame Collection of current ID3
        /// </summary>
        public FrameCollection<AttachedPictureFrame> AttachedPictureFrames
        {
            get { return (FrameCollection<AttachedPictureFrame>)_CollectionFrames[CollectionIndex.AttachedPicture]; }
        }

        /// <summary>
        /// Get GeneralFileFrame Collection of current ID3
        /// </summary>
        public FrameCollection<GeneralFileFrame> EncapsulatedObjectFrames
        {
            get { return (FrameCollection<GeneralFileFrame>)_CollectionFrames[CollectionIndex.EncapsulatedObject]; }
        }

        /// <summary>
        /// Get PopularimeterFrame Collection of current ID3
        /// </summary>
        public FrameCollection<PopularimeterFrame> PopularimeterFrames
        {
            get { return (FrameCollection<PopularimeterFrame>)_CollectionFrames[CollectionIndex.Popularimeter]; }
        }

        /// <summary>
        /// Get AudioEncryptionFrame Collection of current ID3
        /// </summary>
        public FrameCollection<AudioEncryptionFrame> AudioEncryptionFrames
        {
            get { return (FrameCollection<AudioEncryptionFrame>)_CollectionFrames[CollectionIndex.AudioEncryption]; }
        }

        /// <summary>
        /// Get LinkFrame Collection of current ID3
        /// </summary>
        public FrameCollection<LinkFrame> LinkFrames
        {
            get { return (FrameCollection<LinkFrame>)_CollectionFrames[CollectionIndex.Link]; }
        }

        /// <summary>
        /// Get TermOfUseFrame Collection of current ID3
        /// </summary>
        public FrameCollection<TermOfUseFrame> TermOfUseFrames
        {
            get { return (FrameCollection<TermOfUseFrame>)_CollectionFrames[CollectionIndex.TermOfUse]; }
        }

        /// <summary>
        /// Get DataWithSymbolFrame Collection of current ID3
        /// </summary>
        public FrameCollection<DataWithSymbolFrame> DataWithSymbolFrames
        {
            get { return (FrameCollection<DataWithSymbolFrame>)_CollectionFrames[CollectionIndex.DataWithSymbol]; }
        }

        /// <summary>
        /// Get BinaryFrame Collection of current ID3
        /// </summary>
        public FrameCollection<BinaryFrame> UnKnownFrames
        {
            get { return (FrameCollection<BinaryFrame>)_CollectionFrames[CollectionIndex.Unknown]; }
        }

        #endregion
    }
}