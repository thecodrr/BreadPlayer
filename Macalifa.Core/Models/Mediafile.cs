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
using Macalifa.Core;
using Windows.UI.Xaml.Media;
using Macalifa.Tags.ID3;
using Macalifa.Tags.ID3.ID3v2Frames.TextFrames;
namespace Macalifa.Models
{
    public class Mediafile : ViewModelBase
    {
        #region Fields
        private PlayerState state;
        public string path;
        private string encrypted_meta_file;
        private LiteDB.ObjectId audio_encryption;
        private string attached_picture;
        private string audio_seek_point_index;
        private string comment;
        private string encryption_method_registration;
        private string equalisation_2;
        private string equalisation;
        private string event_timing_code;
        private string general_encapsulated_object;
        private string group_identification_registration;
        private string involved_people_list;
        private string linked_information;
        private string music_cd_identifier;
        private string mepg_location_lookup_table;
        private string ownership_information;
        private string popularimeter;
        private string private_frame;
        private string relative_volume_adjustment_2;
        private string seek_frame;
        private string signature_frame;
        private string synchronized_lyric;
        private string synced_tempo_codes;
        private string album;
        private string beatsperminutes;
        private string composer;
        private string genre;
        private string copyright_message;
        private string date;
        private string encoding_time;
        private string playlist_delay;
        private string orginal_release_time;
        private string recording_time;
        private string release_time;
        private string tagging_time;
        private string encoded_by;
        private string lyric;
        private string file_type;
        private string time;
        private string content_group_description;
        private string title;
        private string subtitle;
        private string initial_key;
        private string language;
        private string length;
        private string musician_credits_list;
        private string media_type;
        private string mood;
        private string orginal_title;
        private string orginal_filename;
        private string orginal_lyricist;
        private string orginal_artist;
        private string orginal_release_year;
        private string file_owner;
        private string lead_artist;
        private string band_artist;
        private string conductor;
        private string interpreted;
        private string part_of_set;
        private string produced_notice;
        private string publisher;
        private string track_number;
        private string recording_date;
        private string internet_radio_station_name;
        private string internet_radio_station_owner;
        private string size;
        private string album_sort_order;
        private string preformer_sort_order;
        private string title_sort_order;
        private string isrc;
        private string software_or_hardware_and_setting_used_for_encoding;
        private string set_subtitle;
        private string year;
        private string unique_file_identifier;
        private string term_of_use;
        private string unsynchronized_lyric;
        private string commercial_information;
        private string copyright_information;
        private string official_audio_file_web;
        private string official_artist_web;
        private string official_audio_source_web;
        private string official_radio_station_web;
        private string payment_web;
        private string publisher_web;
        private string NaN = "NaN";
        #endregion

        public ThreadSafeObservableCollection<Playlist> playlists = new ThreadSafeObservableCollection<Playlist>();
        #region Properties
        public ThreadSafeObservableCollection<Playlist> Playlists { get { return playlists; } set { Set(ref playlists, value); }}
        
        public string Path { get { return path; } set { Set(ref path, value); } }
        public PlayerState State { get { return state; } set { Set(ref state, value); } }
        public string EncryptedMetaFile { get { return encrypted_meta_file; } set { encrypted_meta_file = string.IsNullOrEmpty(value) ? encrypted_meta_file = NaN : value; } }
        public LiteDB.ObjectId _id { get { return audio_encryption; } set { Set(ref audio_encryption, value); } }
        public string AttachedPicture { get { return attached_picture; } set { attached_picture = value; } }
        public string AudioSeekPointIndex { get { return audio_seek_point_index; } set { audio_seek_point_index = string.IsNullOrEmpty(value) ? audio_seek_point_index = NaN : value; } }
        public string Comment { get { return comment; } set { comment = string.IsNullOrEmpty(value) ? comment = NaN : value; } }
        public string EncryptionMethodRegistration { get { return encryption_method_registration; } set { encryption_method_registration = string.IsNullOrEmpty(value) ? encryption_method_registration = NaN : value; } }
        public string Equalisation2 { get { return equalisation_2; } set { equalisation_2 = string.IsNullOrEmpty(value) ? equalisation_2 = NaN : value; } }
        public string Equalisation { get { return equalisation; } set { equalisation = string.IsNullOrEmpty(value) ? equalisation = NaN : value; } }
        public string EventTimingCode { get { return event_timing_code; } set { event_timing_code = string.IsNullOrEmpty(value) ? event_timing_code = NaN : value; } }
        public string GeneralEncapsulatedObject { get { return general_encapsulated_object; } set { general_encapsulated_object = string.IsNullOrEmpty(value) ? general_encapsulated_object = NaN : value; } }
        public string GroupIdentificationRegistration { get { return group_identification_registration; } set { group_identification_registration = string.IsNullOrEmpty(value) ? group_identification_registration = NaN : value; } }
        public string LinkedInformation { get { return linked_information; } set { linked_information = string.IsNullOrEmpty(value) ? linked_information = NaN : value; } }
        public string MusicCDIdentifier { get { return music_cd_identifier; } set { music_cd_identifier = string.IsNullOrEmpty(value) ? music_cd_identifier = NaN : value; } }
        public string MepgLocationLookupTable { get { return mepg_location_lookup_table; } set { mepg_location_lookup_table = string.IsNullOrEmpty(value) ? mepg_location_lookup_table = NaN : value; } }
        public string OwnershipInformation { get { return ownership_information; } set { ownership_information = string.IsNullOrEmpty(value) ? ownership_information = NaN : value; } }
        public string Popularimeter { get { return popularimeter; } set { popularimeter = string.IsNullOrEmpty(value) ? popularimeter = NaN : value; } }
        public string PrivateFrame { get { return private_frame; } set { private_frame = string.IsNullOrEmpty(value) ? private_frame = NaN : value; } }
        public string RelativeVolumeAdjustment2 { get { return relative_volume_adjustment_2; } set { relative_volume_adjustment_2 = string.IsNullOrEmpty(value) ? relative_volume_adjustment_2 = NaN : value; } }
        public string SeekFrame { get { return seek_frame; } set { seek_frame = string.IsNullOrEmpty(value) ? seek_frame = NaN : value; } }
        public string SignatureFrame { get { return signature_frame; } set { signature_frame = string.IsNullOrEmpty(value) ? signature_frame = NaN : value; } }
        public string SynchronizedLyric
        {
            get { return synchronized_lyric; }
            set { synchronized_lyric = string.IsNullOrEmpty(value) ? synchronized_lyric = NaN : value; }
        }
        public string SyncedTempoCodes { get { return synced_tempo_codes; } set { synced_tempo_codes = string.IsNullOrEmpty(value) ? synced_tempo_codes = NaN : value; } }
        public string Album { get { return album; } set { album = string.IsNullOrEmpty(value) ? album = "Unknown Album" : value; } }
        public string BeatsPerMinutes { get { return beatsperminutes; } set { beatsperminutes = string.IsNullOrEmpty(value) ? beatsperminutes = NaN : value; } }
        public string Composer { get { return composer; } set { composer = string.IsNullOrEmpty(value) ? composer = NaN : value; } }
        public string Genre { get { return genre; } set { genre = string.IsNullOrEmpty(value) ? genre = NaN : value; } }
        public string CopyrightMessage { get { return copyright_message; } set { copyright_message = string.IsNullOrEmpty(value) ? copyright_message = NaN : value; } }
        public string Date { get { return date; } set { date = string.IsNullOrEmpty(value) ? date = NaN : value; } }
        public string EncodingTime { get { return encoding_time; } set { encoding_time = string.IsNullOrEmpty(value) ? encoding_time = NaN : value; } }
        public string PlaylistDelay { get { return playlist_delay; } set { playlist_delay = string.IsNullOrEmpty(value) ? playlist_delay = NaN : value; } }
        public string OrginalReleaseTime { get { return orginal_release_time; } set { orginal_release_time = string.IsNullOrEmpty(value) ? orginal_release_time = NaN : value; } }
        public string RecordingTime { get { return recording_time; } set { recording_time = string.IsNullOrEmpty(value) ? recording_time = NaN : value; } }
        public string ReleaseTime { get { return release_time; } set { release_time = string.IsNullOrEmpty(value) ? release_time = NaN : value; } }
        public string TaggingTime { get { return tagging_time; } set { tagging_time = string.IsNullOrEmpty(value) ? tagging_time = NaN : value; } }
        public string EncodedBy { get { return encoded_by; } set { encoded_by = string.IsNullOrEmpty(value) ? encoded_by = NaN : value; } }
        public string Lyric { get { return lyric; } set { lyric = string.IsNullOrEmpty(value) ? lyric = NaN : value; } }
        public string FileType { get { return file_type; } set { file_type = string.IsNullOrEmpty(value) ? file_type = NaN : value; } }
        public string Time { get { return time; } set { time = string.IsNullOrEmpty(value) ? time = NaN : value; } }
        public string InvolvedPeopleList { get { return involved_people_list; } set { involved_people_list = string.IsNullOrEmpty(value) ? involved_people_list = NaN : value; } }
        public string ContentGroupDescription { get { return content_group_description; } set { content_group_description = string.IsNullOrEmpty(value) ? content_group_description = NaN : value; } }
        public string Title { get { return title; } set { title = string.IsNullOrEmpty(value) ? title = System.IO.Path.GetFileNameWithoutExtension(path) : value; } }
        public string Subtitle { get { return subtitle; } set { subtitle = string.IsNullOrEmpty(value) ? subtitle = NaN : value; } }
        public string InitialKey { get { return initial_key; } set { initial_key = string.IsNullOrEmpty(value) ? initial_key = NaN : value; } }
        public string Language { get { return language; } set { language = string.IsNullOrEmpty(value) ? language = NaN : value; } }
        public string Length { get { return length; } set { length = string.IsNullOrEmpty(value) ? length = NaN : value; } }
        public string MusicianCreditsList { get { return musician_credits_list; } set { musician_credits_list = string.IsNullOrEmpty(value) ? musician_credits_list = NaN : value; } }
        public string MediaType { get { return media_type; } set { media_type = string.IsNullOrEmpty(value) ? media_type = NaN : value; } }
        public string Mood { get { return mood; } set { mood = string.IsNullOrEmpty(value) ? mood = NaN : value; } }
        public string OrginalTitle { get { return orginal_title; } set { orginal_title = string.IsNullOrEmpty(value) ? orginal_title = NaN : value; } }
        public string OrginalFilename { get { return orginal_filename; } set { orginal_filename = string.IsNullOrEmpty(value) ? orginal_filename = NaN : value; } }
        public string OrginalLyricist { get { return orginal_lyricist; } set { orginal_lyricist = string.IsNullOrEmpty(value) ? orginal_lyricist = NaN : value; } }
        public string OrginalArtist { get { return orginal_artist; } set { orginal_artist = string.IsNullOrEmpty(value) ? orginal_artist = NaN : value; } }
        public string OrginalReleaseYear { get { return orginal_release_year; } set { orginal_release_year = string.IsNullOrEmpty(value) ? orginal_release_year = NaN : value; } }
        public string FileOwner { get { return file_owner; } set { file_owner = string.IsNullOrEmpty(value) ? file_owner = NaN : value; } }
        public string LeadArtist { get { return lead_artist; } set { lead_artist = string.IsNullOrEmpty(value) ? lead_artist = "Unknown Artist" : value; } }
        public string BandArtist { get { return band_artist; } set { band_artist = string.IsNullOrEmpty(value) ? band_artist = NaN : value; } }
        public string Conductor { get { return conductor; } set { conductor = string.IsNullOrEmpty(value) ? conductor = NaN : value; } }
        public string Interpreted { get { return interpreted; } set { interpreted = string.IsNullOrEmpty(value) ? interpreted = NaN : value; } }
        public string Partofset { get { return part_of_set; } set { part_of_set = string.IsNullOrEmpty(value) ? part_of_set = NaN : value; } }
        public string ProducedNotice { get { return produced_notice; } set { produced_notice = string.IsNullOrEmpty(value) ? produced_notice = NaN : value; } }
        public string Publisher { get { return publisher; } set { publisher = string.IsNullOrEmpty(value) ? publisher = NaN : value; } }
        public string TrackNumber { get { return track_number; } set { track_number = string.IsNullOrEmpty(value) ? track_number = NaN : value; } }
        public string RecordingDate { get { return recording_date; } set { recording_date = string.IsNullOrEmpty(value) ? recording_date = NaN : value; } }
        public string InternetRadioStationName { get { return internet_radio_station_name; } set { internet_radio_station_name = string.IsNullOrEmpty(value) ? internet_radio_station_name = NaN : value; } }
        public string InternetRadioStationOwner { get { return internet_radio_station_owner; } set { internet_radio_station_owner = string.IsNullOrEmpty(value) ? internet_radio_station_owner = NaN : value; } }
        public string Size { get { return size; } set { size = string.IsNullOrEmpty(value) ? size = NaN : value; } }
        public string AlbumSortOrder { get { return album_sort_order; } set { album_sort_order = string.IsNullOrEmpty(value) ? album_sort_order = NaN : value; } }
        public string PreformerSortOrder { get { return preformer_sort_order; } set { preformer_sort_order = string.IsNullOrEmpty(value) ? preformer_sort_order = NaN : value; } }
        public string TitleSortOrder { get { return title_sort_order; } set { title_sort_order = string.IsNullOrEmpty(value) ? title_sort_order = NaN : value; } }
        public string ISRC { get { return isrc; } set { isrc = string.IsNullOrEmpty(value) ? isrc = NaN : value; } }
        public string SoftwareHardwareAndSettingUsedForEncoding { get { return software_or_hardware_and_setting_used_for_encoding; } set { software_or_hardware_and_setting_used_for_encoding = string.IsNullOrEmpty(value) ? software_or_hardware_and_setting_used_for_encoding = NaN : value; } }
        public string SetSubtitle { get { return set_subtitle; } set { set_subtitle = string.IsNullOrEmpty(value) ? set_subtitle = NaN : value; } }
        public string Year { get { return year; } set { year = string.IsNullOrEmpty(value) ? year = NaN : value; } }
        public string UniqueFileIdentifier { get { return unique_file_identifier; } set { unique_file_identifier = string.IsNullOrEmpty(value) ? unique_file_identifier = NaN : value; } }
        public string TermOfUse { get { return term_of_use; } set { term_of_use = string.IsNullOrEmpty(value) ? term_of_use = NaN : value; } }
        public string UnsynchronizedLyric { get { return unsynchronized_lyric; } set { unsynchronized_lyric = string.IsNullOrEmpty(value) ? unsynchronized_lyric = NaN : value; } }
        public string CommercialInformation { get { return commercial_information; } set { commercial_information = string.IsNullOrEmpty(value) ? commercial_information = NaN : value; } }
        public string CopyrightInformation { get { return copyright_information; } set { copyright_information = string.IsNullOrEmpty(value) ? copyright_information = NaN : value; } }
        public string OfficialAudioFileweb { get { return official_audio_file_web; } set { official_audio_file_web = string.IsNullOrEmpty(value) ? official_audio_file_web = NaN : value; } }
        public string OfficialArtistweb { get { return official_artist_web; } set { official_artist_web = string.IsNullOrEmpty(value) ? official_artist_web = NaN : value; } }
        public string OfficialAudioSourceweb { get { return official_audio_source_web; } set { official_audio_source_web = string.IsNullOrEmpty(value) ? official_audio_source_web = NaN : value; } }
        public string OfficialRadioStationWeb { get { return official_radio_station_web; } set { official_radio_station_web = string.IsNullOrEmpty(value) ? official_radio_station_web = NaN : value; } }
        public string Paymentweb { get { return payment_web; } set { payment_web = string.IsNullOrEmpty(value) ? payment_web = NaN : value; } }
        public string Publisherweb { get { return publisher_web; } set { publisher_web = string.IsNullOrEmpty(value) ? publisher_web = NaN : value; } }
        #endregion

        public Mediafile()
        {
           // GetText(Data);
        }
        public async void GetText(ID3v2 Data)
        {
            if (Data != null)
            {
                Album = string.IsNullOrEmpty((await GetTextFrame(Data, "TALB"))) ? "Unknown Album" : (await GetTextFrame(Data, "TALB"));
                Title = string.IsNullOrEmpty((await GetTextFrame(Data, "TIT2"))) ? System.IO.Path.GetFileNameWithoutExtension(path) : (await GetTextFrame(Data, "TIT2"));
                LeadArtist = string.IsNullOrEmpty((await GetTextFrame(Data, "TPE1"))) ? "Unknown LeadArtist" : (await GetTextFrame(Data, "TPE1"));
                BeatsPerMinutes = string.IsNullOrEmpty((await GetTextFrame(Data, "TBPM"))) ? "Unknown BPM(BeatsPerMinutes)" : (await GetTextFrame(Data, "TBPM"));
                Composer = string.IsNullOrEmpty((await GetTextFrame(Data, "TCOM"))) ? "Unknown Composer" : (await GetTextFrame(Data, "TCOM"));
                Genre = string.IsNullOrEmpty((await GetTextFrame(Data, "TCON"))) ? "Unknown ContentType" : (await GetTextFrame(Data, "TCON"));
                CopyrightMessage = string.IsNullOrEmpty((await GetTextFrame(Data, "TCOP"))) ? "Unknown CopyrightMessage" : (await GetTextFrame(Data, "TCOP"));
                Date = string.IsNullOrEmpty((await GetTextFrame(Data, "TDAT"))) ? "Unknown Date" : (await GetTextFrame(Data, "TDAT"));
                //EncodingTime = string.IsNullOrEmpty((await GetTextFrame(Data, "TDEN"))) ? "Unknown EncodingTime" : (await GetTextFrame(Data, "TDEN"));
                //PlaylistDelay = string.IsNullOrEmpty((await GetTextFrame(Data, "TDLY"))) ? "Unknown PlaylistDelay" : (await GetTextFrame(Data, "TDLY"));
                //OrginalReleaseTime = string.IsNullOrEmpty((await GetTextFrame(Data, "TDOR"))) ? "Unknown OrginalReleaseTime" : (await GetTextFrame(Data, "TDOR"));
                //RecordingTime = string.IsNullOrEmpty((await GetTextFrame(Data, "TDRC"))) ? "Unknown RecordingTime" : (await GetTextFrame(Data, "TDRC"));
                //ReleaseTime = string.IsNullOrEmpty((await GetTextFrame(Data, "TDRL"))) ? "Unknown ReleaseTime" : (await GetTextFrame(Data, "TDRL"));
                //TaggingTime = string.IsNullOrEmpty((await GetTextFrame(Data, "TDTG"))) ? "Unknown TaggingTime" : (await GetTextFrame(Data, "TDTG"));
                EncodedBy = string.IsNullOrEmpty((await GetTextFrame(Data, "TENC"))) ? "Unknown EncodedBy" : (await GetTextFrame(Data, "TENC"));
                Lyric = string.IsNullOrEmpty((await GetTextFrame(Data, "TEXT"))) ? "Unknown Lyric/TextWriter" : (await GetTextFrame(Data, "TEXT"));
                //FileType = string.IsNullOrEmpty((await GetTextFrame(Data, "TFLT"))) ? "Unknown FileType" : (await GetTextFrame(Data, "TFLT"));
                //Time = string.IsNullOrEmpty((await GetTextFrame(Data, "TIME"))) ? "Unknown Time" : (await GetTextFrame(Data, "TIME"));
                InvolvedPeopleList = string.IsNullOrEmpty((await GetTextFrame(Data, "TIPL"))) ? "Unknown InvolvedPeopleList" : (await GetTextFrame(Data, "TIPL"));
                //ContentGroupDescription = string.IsNullOrEmpty((await GetTextFrame(Data, "TIT1"))) ? "Unknown ContentGroupDescription" : (await GetTextFrame(Data, "TIT1"));
                               Subtitle = string.IsNullOrEmpty((await GetTextFrame(Data, "TIT3"))) ? "Unknown Subtitle/Desripction" : (await GetTextFrame(Data, "TIT3"));
                //InitialKey = string.IsNullOrEmpty((await GetTextFrame(Data, "TKEY"))) ? "Unknown InitialKey" : (await GetTextFrame(Data, "TKEY"));
                Language = string.IsNullOrEmpty((await GetTextFrame(Data, "TLAN"))) ? "Unknown Language" : (await GetTextFrame(Data, "TLAN"));
                Length = string.IsNullOrEmpty((await GetTextFrame(Data, "TLEN"))) ? "Unknown Length" : (await GetTextFrame(Data, "TLEN"));
                MusicianCreditsList = string.IsNullOrEmpty((await GetTextFrame(Data, "TMCL"))) ? "Unknown MusicianCreditsList" : (await GetTextFrame(Data, "TMCL"));
                //MediaType = string.IsNullOrEmpty((await GetTextFrame(Data, "TMED"))) ? "Unknown MediaType" : (await GetTextFrame(Data, "TMED"));
                Mood = string.IsNullOrEmpty((await GetTextFrame(Data, "TMOO"))) ? "Unknown Mood" : (await GetTextFrame(Data, "TMOO"));
                //OrginalTitle = string.IsNullOrEmpty((await GetTextFrame(Data, "TOAL"))) ? "Unknown OrginalTitle" : (await GetTextFrame(Data, "TOAL"));
                //OrginalFilename = string.IsNullOrEmpty((await GetTextFrame(Data, "TOFN"))) ? "Unknown OrginalFilename" : (await GetTextFrame(Data, "TOFN"));
                //OrginalLyricist = string.IsNullOrEmpty((await GetTextFrame(Data, "TOLY"))) ? "Unknown OrginalLyricist" : (await GetTextFrame(Data, "TOLY"));
                //OrginalArtist = string.IsNullOrEmpty((await GetTextFrame(Data, "TOPE"))) ? "Unknown OrginalArtist" : (await GetTextFrame(Data, "TOPE"));
                //OrginalReleaseYear = string.IsNullOrEmpty((await GetTextFrame(Data, "TORY"))) ? "Unknown OrginalReleaseYear" : (await GetTextFrame(Data, "TORY"));
                FileOwner = string.IsNullOrEmpty((await GetTextFrame(Data, "TOWN"))) ? "Unknown FileOwner" : (await GetTextFrame(Data, "TOWN"));
                //BandArtist = string.IsNullOrEmpty((await GetTextFrame(Data, "TPE2"))) ? "Unknown BandArtist" : (await GetTextFrame(Data, "TPE2"));
                Conductor = string.IsNullOrEmpty((await GetTextFrame(Data, "TPE3"))) ? "Unknown Conductor" : (await GetTextFrame(Data, "TPE3"));
                //Interpreted = string.IsNullOrEmpty((await GetTextFrame(Data, "TPE4"))) ? "Unknown Interpreted" : (await GetTextFrame(Data, "TPE4"));
                //Partofset = string.IsNullOrEmpty((await GetTextFrame(Data, "TPOS"))) ? "Unknown Partofset" : (await GetTextFrame(Data, "TPOS"));
                //ProducedNotice = string.IsNullOrEmpty((await GetTextFrame(Data, "TPRO"))) ? "Unknown ProducedNotice" : (await GetTextFrame(Data, "TPRO"));
                Publisher = string.IsNullOrEmpty((await GetTextFrame(Data, "TPUB"))) ? "Unknown Publisher" : (await GetTextFrame(Data, "TPUB"));
                TrackNumber = string.IsNullOrEmpty((await GetTextFrame(Data, "TRCK"))) ? "Unknown TrackNumber" : (await GetTextFrame(Data, "TRCK"));
                RecordingDate = string.IsNullOrEmpty((await GetTextFrame(Data, "TRDA"))) ? "Unknown RecordingDate" : (await GetTextFrame(Data, "TRDA"));
                //InternetRadioStationName = string.IsNullOrEmpty((await GetTextFrame(Data, "TRSN"))) ? "Unknown InternetRadioStationName" : (await GetTextFrame(Data, "TRSN"));
                //InternetRadioStationOwner = string.IsNullOrEmpty((await GetTextFrame(Data, "TRSO"))) ? "Unknown InternetRadioStationOwner" : (await GetTextFrame(Data, "TRSO"));
                Size = string.IsNullOrEmpty((await GetTextFrame(Data, "TSIZ"))) ? "Unknown Size" : (await GetTextFrame(Data, "TSIZ"));
                //AlbumSortOrder = string.IsNullOrEmpty((await GetTextFrame(Data, "TSOA"))) ? "Unknown AlbumSortOrder" : (await GetTextFrame(Data, "TSOA"));
                //PreformerSortOrder = string.IsNullOrEmpty((await GetTextFrame(Data, "TSOP"))) ? "Unknown PreformerSortOrder" : (await GetTextFrame(Data, "TSOP"));
                //TitleSortOrder = string.IsNullOrEmpty((await GetTextFrame(Data, "TSOT"))) ? "Unknown TitleSortOrder" : (await GetTextFrame(Data, "TSOT"));
                ISRC = string.IsNullOrEmpty((await GetTextFrame(Data, "TSRC"))) ? "Unknown ISRC" : (await GetTextFrame(Data, "TSRC"));
                //SoftwareHardwareAndSettingUsedForEncoding = string.IsNullOrEmpty((await GetTextFrame(Data, "TSSE"))) ? "Unknown Software/HardwareAndSettingUsedForEncoding" : (await GetTextFrame(Data, "TSSE"));
                //SetSubtitle = string.IsNullOrEmpty((await GetTextFrame(Data, "TSST"))) ? "Unknown SetSubtitle" : (await GetTextFrame(Data, "TSST"));
                Year = string.IsNullOrEmpty((await GetTextFrame(Data, "TYER"))) ? "Unknown Year" : (await GetTextFrame(Data, "TYER"));
            }            
        }
        public async Task<string> GetTextFrame(ID3v2 info, string FrameID)
        {
            // string Text = "";
            return await Task.Run(() =>
            {
                foreach (TextFrame TF in info.TextFrames)
                    if (TF.FrameID == FrameID)
                    {
                        return TF.Text;
                    }
                return "";
            });

        }
    }

}
