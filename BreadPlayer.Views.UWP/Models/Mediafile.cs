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
using BreadPlayer.Core;
using System;
using Windows.UI;

namespace BreadPlayer.Models
{
    public class Mediafile : ObservableObject, IComparable<Mediafile>
    {
        #region Fields
        private Color albumArtColor;
        private PlayerState state;
        private string path;
        private string encrypted_meta_file;
        private LiteDB.ObjectId id;
        private string attached_picture;
        private string comment;
        private string folderPath;
        private string synchronized_lyric;
        private string album;
        private string beatsperminutes;
        private string composer;
        private string genre;
        private string copyright_message;
        private string date;
        private string encoded_by;
        private string lyric;
        private string content_group_description;
        private string title;
        private string subtitle;
        private string length;
        private string orginal_filename;
        private string lead_artist;
        private string publisher;
        private string track_number;
        private string size;
        private string year;
        private string NaN = "NaN";
        #endregion

        ThreadSafeObservableCollection<Playlist> playlists = new ThreadSafeObservableCollection<Playlist>();
       
        #region Properties
        public ThreadSafeObservableCollection<Playlist> Playlists { get { return playlists; } set { Set(ref playlists, value); }}
        public Color AlbumArtColor { get { return albumArtColor; } set { Set(ref albumArtColor, value); } }
        public string Path { get { return path; } set { Set(ref path, value); } }
        public PlayerState State { get { return state; } set { Set(ref state, value); } }
        public string EncryptedMetaFile { get { return encrypted_meta_file; } set { encrypted_meta_file = string.IsNullOrEmpty(value) ? encrypted_meta_file = NaN : value; } }
        public LiteDB.ObjectId _id { get { return id; } set { Set(ref id, value); } }
        public string AttachedPicture { get { return attached_picture; } set { attached_picture = value; } }
       public string Comment { get { return comment; } set { comment = string.IsNullOrEmpty(value) ? comment = NaN : value; } }
      public string FolderPath { get { return folderPath; } set { folderPath = string.IsNullOrEmpty(value) ? folderPath = "" : value; } }
       public string SynchronizedLyric
        {
            get { return synchronized_lyric; }
            set { synchronized_lyric = string.IsNullOrEmpty(value) ? synchronized_lyric = NaN : value; }
        }
        public string Album { get { return album; } set { album = string.IsNullOrEmpty(value) ? album = "Unknown Album" : value; } }
        public string BeatsPerMinutes { get { return beatsperminutes; } set { beatsperminutes = string.IsNullOrEmpty(value) ? beatsperminutes = NaN : value; } }
        public string Composer { get { return composer; } set { composer = string.IsNullOrEmpty(value) ? composer = NaN : value; } }
        public string Genre { get { return genre; } set { genre = string.IsNullOrEmpty(value) ? genre = "Other" : value; } }
        public string CopyrightMessage { get { return copyright_message; } set { copyright_message = string.IsNullOrEmpty(value) ? copyright_message = NaN : value; } }
        public string Date { get { return date; } set { date = string.IsNullOrEmpty(value) ? date = NaN : value; } }
    public string EncodedBy { get { return encoded_by; } set { encoded_by = string.IsNullOrEmpty(value) ? encoded_by = NaN : value; } }
        public string Lyric { get { return lyric; } set { lyric = string.IsNullOrEmpty(value) ? lyric = NaN : value; } }
        public string ContentGroupDescription { get { return content_group_description; } set { content_group_description = string.IsNullOrEmpty(value) ? content_group_description = NaN : value; } }
        public string Title { get { return title; } set { title = string.IsNullOrEmpty(value) ? title = System.IO.Path.GetFileNameWithoutExtension(path) : value; } }
        public string Subtitle { get { return subtitle; } set { subtitle = string.IsNullOrEmpty(value) ? subtitle = NaN : value; } }
    public string Publisher { get { return publisher; } set { publisher = string.IsNullOrEmpty(value) ? publisher = NaN : value; } }
        public string TrackNumber { get { return track_number; } set { track_number = string.IsNullOrEmpty(value) ? track_number = NaN : value; } }
    public string Size { get { return size; } set { size = string.IsNullOrEmpty(value) ? size = NaN : value; } }
        public string Year { get { return year; } set { year = value == "0" || string.IsNullOrEmpty(value) ? "" : value; } }
        public string LeadArtist { get { return lead_artist; } set { lead_artist = string.IsNullOrEmpty(value) ? lead_artist = NaN : value; } }
        public string OrginalFilename { get { return orginal_filename; } set { orginal_filename = string.IsNullOrEmpty(value) ? orginal_filename = NaN : value; } }
        public string Length { get { return length; } set { length = string.IsNullOrEmpty(value) ? length = NaN : value; } }

        #endregion

        public int CompareTo(Mediafile compareTo)
        {
           return this.Title.CompareTo(compareTo.Title);            
        }
        public Mediafile()
        {
           // GetText(Data);
        }
    }

}
