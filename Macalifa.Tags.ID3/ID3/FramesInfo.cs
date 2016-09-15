using System;
using System.Collections.Generic;
using System.Text;
using Macalifa.Tags.ID3.ID3v2Frames;
using System.Collections;
using Macalifa.Tags.ID3.ID3v2Frames.ArrayFrames;
using Macalifa.Tags.ID3.ID3v2Frames.BinaryFrames;
using Macalifa.Tags.ID3.ID3v2Frames.OtherFrames;
using Macalifa.Tags.ID3.ID3v2Frames.StreamFrames;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
using System.Reflection;
/*
 * This file only contain 2 classes that use for storing each frame information
 * ex. if you find TIT2 FrameID in tag with FrameList you can understand is it a
 * TextFrame or not. or even is it a valid FrameID ? and something like this
 */
namespace Macalifa.Tags.ID3.ID3v2Frames
{
    /// <summary>
    /// Static class that represent informaion about ID3v2 frames
    /// </summary>
    public static class FramesInfo
    {
        private static Dictionary<string, FrameInfo> _FramesDictionary; // a dictionary contain all Frames information

        /// <summary>
        /// Gets Dictionary of frames information
        /// </summary>
        public static Dictionary<string, FrameInfo> FramesDictionary
        {
            get
            {
                if (_FramesDictionary == null)
                {
                    _FramesDictionary = new Dictionary<string, FrameInfo>();
                    InitializeFrameDictionary();
                }

                return _FramesDictionary;
            }
        }

        /// <summary>
        /// Initialize All FrameID infos
        /// </summary>
        private static void InitializeFrameDictionary()
        {
            _FramesDictionary.Add("", new FrameInfo("", "CRM", "Encrypted Meta File",
                new bool[] { true, false, false }, false, null));
            _FramesDictionary.Add("AENC", new FrameInfo("AENC", "CRA", "Audio Encryption",
                new bool[] { true, true, true }, false, typeof(AudioEncryptionFrame)));
            _FramesDictionary.Add("APIC", new FrameInfo("APIC", "PIC", "Attached Picture",
                new bool[] { true, true, true }, false, typeof(AttachedPictureFrame)));
            _FramesDictionary.Add("ASPI", new FrameInfo("ASPI", null, "Audio Seek Point Index",
                new bool[] { false, false, true }, true, null));
            _FramesDictionary.Add("COMM", new FrameInfo("COMM", "COM", "Comment",
                new bool[] { true, true, true }, false, typeof(TextWithLanguageFrame)));
            _FramesDictionary.Add("COMR", new FrameInfo("COMR", null, "Commercial Frame",
                new bool[] { false, true, true }, true, typeof(CommercialFrame)));
            _FramesDictionary.Add("ENCR", new FrameInfo("ENCR", null, "Encryption Method Registration",
                new bool[] { false, true, true }, false, typeof(DataWithSymbolFrame)));
            _FramesDictionary.Add("EQU2", new FrameInfo("EQU2", null, "Equalisation (2)",
                new bool[] { false, false, true }, true, null));
            _FramesDictionary.Add("EQUA", new FrameInfo("EQUA", "EQU", "Equalisation",
                new bool[] { true, true, false }, true, typeof(Equalisation)));
            _FramesDictionary.Add("ETCO", new FrameInfo("ETCO", "ETC", "Event Timing Code",
                new bool[] { true, true, true }, false, typeof(EventTimingCodeFrame)));
            _FramesDictionary.Add("GEOB", new FrameInfo("GEOB", "GEO", "General Encapsulated Object",
                new bool[] { true, true, true }, false, typeof(GeneralFileFrame)));
            _FramesDictionary.Add("GRID", new FrameInfo("GRID", null, "Group Identification Registration",
                new bool[] { false, true, true }, false, typeof(DataWithSymbolFrame)));
            _FramesDictionary.Add("IPLS", new FrameInfo("IPLS", "IPL", "Involved People List",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("LINK", new FrameInfo("LINK", "LNK", "Linked Information",
                new bool[] { true, true, true }, false, typeof(LinkFrame)));
            _FramesDictionary.Add("MCDI", new FrameInfo("MCDI", "MCI", "Music CD Identifier",
                new bool[] { true, true, true }, true, typeof(BinaryFrame)));
            _FramesDictionary.Add("MLLT", new FrameInfo("MLLT", "MLL", "Mepg Location Lookup Table",
                new bool[] { true, true, true }, true, null));
            _FramesDictionary.Add("OWNE", new FrameInfo("OWNE", null, "Ownership Information",
                new bool[] { false, true, true }, true, typeof(OwnershipFrame)));
            _FramesDictionary.Add("PCNT", new FrameInfo("PCNT", "CNT", "Play Counter",
                new bool[] { true, true, true }, true, typeof(PlayCounterFrame)));
            _FramesDictionary.Add("POPM", new FrameInfo("POPM", "POP", "Popularimeter",
                new bool[] { true, true, true }, false, typeof(PopularimeterFrame)));
            _FramesDictionary.Add("POSS", new FrameInfo("POSS", null, "Position Synchronisation Frame",
                new bool[] { false, true, true }, true, typeof(PositionSynchronisedFrame)));
            _FramesDictionary.Add("PRIV", new FrameInfo("PRIV", null, "Private Frame",
                new bool[] { false, true, true }, false, typeof(PrivateFrame)));
            _FramesDictionary.Add("RBUF", new FrameInfo("RBUF", "BUF", "Recommended Buffer Size",
                new bool[] { true, true, true }, true, typeof(RecomendedBufferSizeFrame)));
            _FramesDictionary.Add("RVA2", new FrameInfo("RVA2", null, "Relative Volume Adjustment (2)",
                new bool[] { false, false, true }, true, null));
            _FramesDictionary.Add("RVAD", new FrameInfo("RVAD", "RVA", "Relative Volume Adjustment",
                new bool[] { true, true, false }, true, typeof(RelativeVolumeFrame)));
            _FramesDictionary.Add("RVRB", new FrameInfo("RVRB", "REV", "Reverb",
                new bool[] { true, true, true }, true, typeof(ReverbFrame)));
            _FramesDictionary.Add("SEEK", new FrameInfo("SEEK", null, "Seek Frame",
                new bool[] { false, false, true }, true, null));
            _FramesDictionary.Add("SIGN", new FrameInfo("SIGN", null, "Signature Frame",
                new bool[] { false, false, true }, false, typeof(DataWithSymbolFrame)));
            _FramesDictionary.Add("SYLT", new FrameInfo("SYLT", "SLT", "Synchronized Lyric/Text",
                new bool[] { true, true, true }, false, typeof(SynchronisedText)));
            _FramesDictionary.Add("SYTC", new FrameInfo("SYTC", "STC", "Synced Tempo Codes",
                new bool[] { true, true, true }, true, typeof(SynchronisedTempoFrame)));
            _FramesDictionary.Add("TALB", new FrameInfo("TALB", "TAL", "Album",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TBPM", new FrameInfo("TBPM", "TBP", "BPM ( Beats Per Minutes)",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TCOM", new FrameInfo("TCOM", "TCM", "Composer",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TCON", new FrameInfo("TCON", "TCO", "Content Type",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TCOP", new FrameInfo("TCOP", "TCR", "Copyright Message",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDAT", new FrameInfo("TDAT", "TDA", "Date",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDEN", new FrameInfo("TDEN", null, "Encoding Time",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDLY", new FrameInfo("TDLY", "TDY", "Playlist Delay",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDOR", new FrameInfo("TDOR", null, "Orginal Release Time",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDRC", new FrameInfo("TDRC", null, "Recording Time",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDRL", new FrameInfo("TDRL", null, "Release Time",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TDTG", new FrameInfo("TDTG", null, "Tagging Time",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TENC", new FrameInfo("TENC", "TEN", "Encoded By",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TEXT", new FrameInfo("TEXT", "TXT", "Lyric/Text Writer",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TFLT", new FrameInfo("TFLT", "TFT", "File Type",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TIME", new FrameInfo("TIME", "TIM", "Time",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TIPL", new FrameInfo("TIPL", null, "Involved People List",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TIT1", new FrameInfo("TIT1", "TT1", "Content Group Description",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TIT2", new FrameInfo("TIT2", "TT2", "Title",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TIT3", new FrameInfo("TIT3", "TT3", "Subtitle/Desripction",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TKEY", new FrameInfo("TKEY", "TKE", "Initial Key",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TLAN", new FrameInfo("TLAN", "TLA", "Language",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TLEN", new FrameInfo("TLEN", "TLE", "Length",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TMCL", new FrameInfo("TMCL", null, "Musician Credits List",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TMED", new FrameInfo("TMED", "TMT", "Media Type",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TMOO", new FrameInfo("TMOO", null, "Mood",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TOAL", new FrameInfo("TOAL", "TOT", "Orginal Title",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TOFN", new FrameInfo("TOFN", "TOF", "Orginal Filename",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TOLY", new FrameInfo("TOLY", "TOL", "Orginal Lyricist",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TOPE", new FrameInfo("TOPE", "TOA", "Orginal Artist",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TORY", new FrameInfo("TORY", "TOR", "Orginal Release Year",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TOWN", new FrameInfo("TOWN", null, "File Owner",
                new bool[] { false, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPE1", new FrameInfo("TPE1", "TP1", "Lead Artist",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPE2", new FrameInfo("TPE2", "TP2", "Band Artist",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPE3", new FrameInfo("TPE3", "TP3", "Conductor",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPE4", new FrameInfo("TPE4", "TP4", "Interpreted",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPOS", new FrameInfo("TPOS", "TPA", "Part of set",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPRO", new FrameInfo("TPRO", null, "Produced Notice",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TPUB", new FrameInfo("TPUB", "TPB", "Publisher",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TRCK", new FrameInfo("TRCK", "TRK", "Track Number",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TRDA", new FrameInfo("TRDA", "TRD", "Recording Date",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TRSN", new FrameInfo("TRSN", null, "Internet Radio Station Name",
                new bool[] { false, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TRSO", new FrameInfo("TRSO", null, "Internet Radio Station Owner",
                new bool[] { false, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSIZ", new FrameInfo("TSIZ", "TSI", "Size",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSOA", new FrameInfo("TSOA", null, "Album Sort Order",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSOP", new FrameInfo("TSOP", null, "Preformer Sort Order",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSOT", new FrameInfo("TSOT", null, "Title Sort Order",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSRC", new FrameInfo("TSRC", "TRC", "ISRC",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSSE", new FrameInfo("TSSE", "TSS", "Software/Hardware And Setting Used For Encoding",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TSST", new FrameInfo("TSST", null, "Set Subtitle",
                new bool[] { false, false, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("TYER", new FrameInfo("TYER", "TYE", "Year",
                new bool[] { true, true, false }, true, typeof(TextFrame)));
            _FramesDictionary.Add("UFID", new FrameInfo("UFID", "UFI", "Unique File Identifier",
                new bool[] { true, true, true }, false, typeof(PrivateFrame)));
            _FramesDictionary.Add("USER", new FrameInfo("USER", null, "Term Of Use",
                new bool[] { false, true, true }, false, typeof(TermOfUseFrame)));
            _FramesDictionary.Add("USLT", new FrameInfo("USLT", "ULT", "Unsynchronized Lyric",
                new bool[] { true, true, true }, false, typeof(TextWithLanguageFrame)));
            _FramesDictionary.Add("WCOM", new FrameInfo("WCOM", "WCM", "Commercial Information",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WCOP", new FrameInfo("WCOP", "WCP", "Copyright Information",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WOAF", new FrameInfo("WOAF", "WAF", "Official Audio File web",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WOAR", new FrameInfo("WOAR", "WAR", "Official Artist web",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WOAS", new FrameInfo("WOAS", "WAS", "Official Audio Source web",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WORS", new FrameInfo("WORS", null, "Official Radio Station Web",
                new bool[] { false, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WPAY", new FrameInfo("WPAY", null, "Payment web",
                new bool[] { false, true, true }, true, typeof(TextFrame)));
            _FramesDictionary.Add("WPUB", new FrameInfo("WPUB", "WPB", "Publisher web",
                new bool[] { true, true, true }, true, typeof(TextFrame)));
        }

        /// <summary>
        /// Get FrameInfo from 4 chacarter FrameID
        /// </summary>
        /// <param name="FrameID">4 character FrameID to get FrameInfo</param>
        /// <returns>FrameInfo contain Specific frame information</returns>
        public static FrameInfo GetFrame(string FrameID)
        {
            return (FrameInfo)FramesDictionary[FrameID];
        }
      
        /// <summary>
        /// Get 4 Character FrameID for specific 3 Character FrameID
        /// </summary>
        /// <param name="FrameID3">3 character FrameID</param>
        /// <returns>System.String contain 4 Character FrameID or null if not found</returns>
        public static string Get4CharID(string FrameID3)
        {
            foreach (FrameInfo FI in FramesDictionary.Values)
                if (FrameID3 == FI.FrameID3Char)
                    return FI.FrameID4Char;
            return null;
        }

        /// <summary>
        /// Get 3 character FrameID from specific 4 Character FrameID
        /// </summary>
        /// <param name="FrameID">4 character FrameID</param>
        /// <returns>3 Chacater FrameID</returns>
        public static string Get3CharID(string FrameID)
        {
            if (FramesDictionary.ContainsKey(FrameID))
                return ((FrameInfo)FramesDictionary[FrameID]).FrameID3Char;

            return null;
        }

        /// <summary>
        /// Indicate if specific FrameID is TextFrame(1), UserTextFrame(2) or non of them(0)
        /// </summary>
        /// <param name="FrameID">FrameID to control</param>
        /// <param name="Ver">minor version of ID3v2</param>
        /// <returns>int that indicate FrameID type</returns>
        public static int IsTextFrame(string FrameID, int Ver)
        {
            // 0: mean's it's not TextFrame and UserTextFrame either
            // 1: it's TextFrame
            // 2: it's UserTextFrame
            if (FrameID == "IPLS")
            {
                if (Ver == 4) // in version 4 IPLS frame removed
                    return 0;
                else
                    return 1;
            }

            if (FrameID[0] == 'T' || FrameID[0] == 'W')
            {
                if (FramesDictionary.ContainsKey(FrameID))
                    if (((FrameInfo)FramesDictionary[FrameID]).IsValid(Ver))
                        return 1;
                return 2;
            }

            return 0;
        }

        /// <summary>
        /// Indicate if specific FrameID is compatible with specific minor version of ID3v2
        /// </summary>
        /// <remarks>This method return false for UserTextFrames</remarks>
        /// <param name="FrameID">FrameID to check</param>
        /// <param name="Ver">minor version of ID3v2</param>
        /// <returns>true if it's compatible otherwise false</returns>
        public static bool IsCompatible(string FrameID, int Ver)
        {
            if (!FramesDictionary.ContainsKey(FrameID))
                return false;

            return ((FrameInfo)FramesDictionary[FrameID]).IsValid(Ver);
        }

        /// <summary>
        /// Indicate if specific string is a valid FrameID
        /// </summary>
        /// <param name="FrameID">FrameID to check</param>
        /// <returns>true if valid otherwise false</returns>
        public static bool IsValidFrameID(string FrameID)
        {
            if (FrameID == null)
                return false;

            if (FrameID.Length != 4)
                return false;
            else
                foreach (char ch in FrameID)
                    if (!Char.IsUpper(ch) && !char.IsDigit(ch))
                        return false;
            return true;
        }

        /// <summary>
        /// Gets all textframes
        /// </summary>
        public static FrameInfo[] TextFrames
        {
            get
            {
                ArrayList List = new ArrayList();
                foreach (FrameInfo var in FramesDictionary.Values)
                    if (var.FrameID4Char.StartsWith("T"))
                        List.Add(var);

                return (FrameInfo[])List.ToArray(typeof(FrameInfo));
            }
        }
    }

    /// <summary>
    /// Provide information for one Frame
    /// </summary>
    public class FrameInfo
    {
        private string _Name;
        private string _FrameID4Ch;   // FrameID with 4 Characters
        private string _FrameID3Ch;   // FrameID with 3 Characters
        private bool[] _Validation;
        private bool _IsSingle; // Indicate if this frame can maximum be one time
        private Type _ClassType; // class type integrated with this Frame

        /// <summary>
        /// Create new FrameInfo class
        /// </summary>
        public FrameInfo(string FrameID, string FrameID3Char, string Name,
            bool[] Validation, bool IsSingle, Type Class)
        {
            _Name = Name;
            _FrameID4Ch = FrameID;
            _FrameID3Ch = FrameID3Char;
            _Validation = Validation;
            _IsSingle = IsSingle;
            _ClassType = Class;
        }

        /// <summary>
        /// Get Name of current FrameIDInfo
        /// </summary>
        public string Name
        {
            get
            { return _Name; }
        }

        /// <summary>
        /// Get FrameID of current FrameIDInfo for specific version of ID3v2
        /// </summary>
        /// <param name="Version">minor version of ID3v2 to compatible with FrameID</param>
        /// <returns>System.String retrieve FrameID of current FrameIDInfo</returns>
        public string FrameID(int Version)
        {
            if (Version < 2 || Version > 4)
                throw (new ArgumentOutOfRangeException("Version must be between 2-4"));
            else if (Version == 2)
                return _FrameID3Ch;
            else
                return _FrameID4Ch;
        }

        /// <summary>
        /// Indicate if current FrameID is valid for specific Version of ID3v2
        /// </summary>
        /// <param name="Version">Version of ID3v2</param>
        /// <returns>true if it's valid otherwise false</returns>
        public bool IsValid(int Version)
        {
            if (Version < 2 && Version > 4)
                throw (new ArgumentOutOfRangeException("Version value must be between 2-4"));

            return _Validation[Version - 2];
        }

        /// <summary>
        /// Get 3 character FrameID
        /// </summary>
        public string FrameID3Char
        {
            get
            { return _FrameID3Ch; }
        }

        /// <summary>
        /// Get 4 character FrameID
        /// </summary>
        public string FrameID4Char
        {
            get
            { return _FrameID4Ch; }
        }

        /// <summary>
        /// Indicate if current Frame can occur maximum one time
        /// </summary>
        public bool IsSingle
        {
            get
            { return _IsSingle; }
        }

        /// <summary>
        /// Get Type that integrated with current Frame
        /// </summary>
        public Type ClassType
        {
            get
            { return _ClassType; }
        }

        /// <summary>
        /// Get construtor for a frame
        /// </summary>
        /// <param name="FrameID">FrameId to get consctructor</param>
        /// <param name="Flags">Flags of frame</param>
        /// <param name="reader">TagStream to read frame from</param>
        /// <param name="Length">Maximum available length for frame</param>
        /// <returns>Constructor</returns>
        public Frame Constuctor(string FrameID, FrameFlags Flags,
             int Length, System.IO.Stream TStream)
        {
             Type[] Con = new Type[4];
                Con[0] = typeof(string);
                Con[1] = typeof(FrameFlags);
                Con[2] = typeof(int);
                Con[3] = typeof(System.IO.Stream);

                object[] Params = new object[4];
                Params[0] = FrameID;
                Params[1] = Flags;
                Params[2] = Length;

                Params[3] = TStream;


                return (Frame)ClassType.GetConstructor(Con).Invoke(Params);
           
            }

        /// <summary>
        /// Convert current FrameInfo to string
        /// </summary>
        /// <returns>System.String contain current FrameInfo Name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
