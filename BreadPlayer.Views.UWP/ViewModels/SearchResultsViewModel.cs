using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;

namespace BreadPlayer.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private ThreadSafeObservableCollection<Mediafile> _querySongs;
        public ThreadSafeObservableCollection<Mediafile> QuerySongs
        {
            get => _querySongs;
            set => Set(ref _querySongs, value);
        }

        private ThreadSafeObservableCollection<Album> _queryAlbums;
        public ThreadSafeObservableCollection<Album> QueryAlbums
        {
            get => _queryAlbums;
            set => Set(ref _queryAlbums, value);
        }
      
        public async Task<IEnumerable<Mediafile>> StartSearch(string query)
        {
            LibraryService service = new LibraryService(new KeyValueStoreDatabaseService(SharedLogic.DatabasePath, "Tracks", "TracksText"));
            return await service.Query(query);
        }
        public async Task GetAlbumsAndTracks(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                QueryAlbums = new ThreadSafeObservableCollection<Album>();
                QuerySongs = new ThreadSafeObservableCollection<Mediafile>();
                var queryresults = (await StartSearch(query)).GroupBy(t => t.Album).ToArray();
                for(int i = 0; i < queryresults.Count(); i++)
                {
                    QuerySongs.AddRange(queryresults[i].Where(t => t.Title.ToLower().Contains(query.ToLower())));
                    if (queryresults[i].Key.ToLower().Contains(query.ToLower()))
                    {
                        var albumSongs = queryresults[i].Select(t => t);
                        var firstSong = albumSongs.First() ?? new Mediafile();
                        Album album = new Album
                        {
                            Artist = firstSong?.LeadArtist,
                            AlbumName = queryresults[i].Key,
                            AlbumArt = string.IsNullOrEmpty(firstSong?.AttachedPicture) ? null : firstSong?.AttachedPicture
                        };
                        QueryAlbums.Add(album);
                    }
                }
            }
        }
    }
}
