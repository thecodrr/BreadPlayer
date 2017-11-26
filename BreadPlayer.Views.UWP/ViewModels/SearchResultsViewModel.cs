using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Messengers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class SearchResultsViewModel : ObservableObject
    {
        public SearchResultsViewModel()
        {
            Messenger.Instance.Register(Messengers.MessageTypes.MsgSearch, new System.Action<Message>(HandleSearchMessage));
        }
        private async void HandleSearchMessage(Message message)
        {
            if(message.Payload is string query)
            {
                message.HandledStatus = MessageHandledStatus.HandledCompleted;
                await GetAlbumsAndTracks(query).ConfigureAwait(false);
            }
        }
        private bool _albumsVisible;

        public bool AlbumsVisible
        {
            get => _albumsVisible;
            set => Set(ref _albumsVisible, value);
        }

        private bool _artistsVisible;

        public bool ArtistsVisible
        {
            get => _artistsVisible;
            set => Set(ref _artistsVisible, value);
        }

        private bool _toastsVisible;

        public bool ToastsVisible
        {
            get => _toastsVisible;
            set => Set(ref _toastsVisible, value);
        }

        private List<Mediafile> _querySongs;

        public List<Mediafile> QuerySongs
        {
            get => _querySongs;
            set => Set(ref _querySongs, value);
        }

        private List<Album> _queryAlbums;

        public List<Album> QueryAlbums
        {
            get => _queryAlbums;
            set => Set(ref _queryAlbums, value);
        }

        private List<Artist> _queryArtists;

        public List<Artist> QueryArtists
        {
            get => _queryArtists;
            set => Set(ref _queryArtists, value);
        }

        public async Task<(IEnumerable<Mediafile> Songs, IEnumerable<Album> Albums, IEnumerable<Artist> Artists)> StartSearch(string query)
        {
            var documentStore = new KeyValueStoreDatabaseService(SharedLogic.Instance.DatabasePath, "Tracks");
            LibraryService service = new LibraryService(documentStore);
            return (await service.Query(query).ConfigureAwait(false),
                    await SharedLogic.Instance.AlbumArtistService.QueryAlbumsAsync(query, 5).ConfigureAwait(false),
                    await SharedLogic.Instance.AlbumArtistService.QueryArtistsAsync(query, 5).ConfigureAwait(false));
        }
        public async Task GetAlbumsAndTracks(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var queryresults = (await StartSearch(query.ToLower()).ConfigureAwait(false));
                if (queryresults.Songs?.Any() == true)
                {
                    ToastsVisible = true;
                    QuerySongs = new List<Mediafile>(queryresults.Songs);
                }
                if (queryresults.Albums?.Any() == true)
                {
                    AlbumsVisible = true;
                    QueryAlbums = new List<Album>(queryresults.Albums);
                }
                if (queryresults.Artists?.Any() == true)
                {
                    ArtistsVisible = true;
                    QueryArtists = new List<Artist>(queryresults.Artists);
                }
            }
        }
    }
}