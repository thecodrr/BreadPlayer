using BreadPlayer.Web.BaiduLyricsAPI;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class NowPlayingViewModel : ViewModelBase
    {
        #region Loading Properties
        bool isLoading;
        public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }
        bool isFailed;
        public bool IsFailed { get => isFailed; set => Set(ref isFailed, value); }
        double progress;
        public double Progress { get => progress; set => Set(ref progress, value); }
        string progressText;
        public string ProgressText { get => progressText; set => Set(ref progressText, value); }
        #endregion

        string artistBio;
        public string ArtistBio
        {
            get => artistBio;
            set => Set(ref artistBio, value);
        }
        ThreadSafeObservableCollection<LastTrack> albumTracks;
        public ThreadSafeObservableCollection<LastTrack> AlbumTracks
        {
            get => albumTracks;
            set => Set(ref albumTracks, value);
        }
        ThreadSafeObservableCollection<LastArtist> similarArtists;
        public ThreadSafeObservableCollection<LastArtist> SimilarArtists
        {
            get => similarArtists;
            set => Set(ref similarArtists, value);
        }
        private LastfmClient LastfmClient
        {
            get => new Lastfm().LastfmClient;
        }
        public NowPlayingViewModel()
        {
           // GetArtistInfo(Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist);
            //GetLyrics("Eminem Phenomenal");
        }
        private async void GetArtistInfo(string artistName)
        {
            ProgressText = "Sending request for artist info.";
            IsLoading = true;
            var artistInfoResponse = await LastfmClient.Artist.GetInfoAsync(artistName, "en", true);
            ProgressText = "Response for artist info request recieved.";
            Progress = 25;
            if (artistInfoResponse.Success)
            {
                Progress = 30;
                ProgressText = "Response succeeded; Parsing content.";
                LastArtist artist = artistInfoResponse.Content;
                Progress = 40;
                ArtistBio = artist.Bio.Content;
                Progress = 50;
                SimilarArtists = new ThreadSafeObservableCollection<LastArtist>(artist.Similar);
                Progress = 60;
                ProgressText = "Content Parsed.";
                GetAlbumInfo(Core.SharedLogic.Player.CurrentlyPlayingFile.LeadArtist, Core.SharedLogic.Player.CurrentlyPlayingFile.Album);
            }
            else
            {
                Progress = 0;
                ProgressText = "Request Failed due bad network or bad artist name.";
                IsFailed = true;
                IsLoading = false;
            }
        }
        private async void GetAlbumInfo(string artistName, string albumName)
        {
            ProgressText = "Sending request for album info.";
            Progress = 65;
            var albumInfoResponse = await LastfmClient.Album.GetInfoAsync(artistName, albumName, true);
            ProgressText = "Response for album info recieved.";
            Progress = 70;
            if (albumInfoResponse.Success)
            {
                ProgressText = "Parsing content.";
                Progress = 80;
                LastAlbum album = albumInfoResponse.Content;
                AlbumTracks = new ThreadSafeObservableCollection<LastTrack>(album.Tracks);
                Progress = 90;
                ProgressText = "Content parsed.";
            }
            else
            {
                IsFailed = true;
                ProgressText = "Bad request.";
                Progress = 0;
            }
            Progress = 100;
            ProgressText = "Album info recieved.";
            await Task.Delay(2000);
            IsLoading = false;
        }
        private async void GetLyrics(string query)
        {
            ApiMethods methods = new ApiMethods();
            var response = await methods.Search(query);
        }
    }
}
