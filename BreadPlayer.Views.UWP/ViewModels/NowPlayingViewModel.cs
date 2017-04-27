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
            GetArtistInfo("Adele");
            GetAlbumInfo("Adele", "25");
            GetLyrics("Eminem Phenomenal");
        }
        private async void GetArtistInfo(string artistName)
        {
            var artistInfoResponse = await LastfmClient.Artist.GetInfoAsync(artistName, "en", true);
            if(artistInfoResponse.Success)
            {
                LastArtist artist = artistInfoResponse.Content;
                ArtistBio = artist.Bio.Content;
                SimilarArtists = new ThreadSafeObservableCollection<LastArtist>(artist.Similar);
            }
        }
        private async void GetAlbumInfo(string artistName, string albumName)
        {
            var albumInfoResponse = await LastfmClient.Album.GetInfoAsync(artistName, albumName, true);
            if (albumInfoResponse.Success)
            {
                LastAlbum album = albumInfoResponse.Content;
                AlbumTracks = new ThreadSafeObservableCollection<LastTrack>(album.Tracks);
            }
        }
        private async void GetLyrics(string query)
        {
            ApiMethods methods = new ApiMethods();
            var response = await methods.Search(query);
        }
    }
}
