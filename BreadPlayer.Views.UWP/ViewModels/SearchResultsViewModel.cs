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
      
        public Task<IEnumerable<Mediafile>> StartSearch(string query)
        {
            LibraryService service = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks"));
            return service.Query(query);
        }
        public async Task GetAlbumsAndTracks(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                QueryAlbums = new ThreadSafeObservableCollection<Album>();
                QuerySongs = new ThreadSafeObservableCollection<Mediafile>();
                var queryresults = (await StartSearch(query.ToLower()));
                if(queryresults != null && queryresults.Any())
                {
                    var groupedQueryresults = queryresults.GroupBy(t => t.Album).ToArray();
                    for (int i = 0; i < groupedQueryresults.Count(); i++)
                    {
                        QuerySongs.AddRange(groupedQueryresults[i].Where(t => t.Title.ToLower().Contains(query.ToLower())));
                        if (groupedQueryresults[i].Key.ToLower().Contains(query.ToLower()))
                        {
                            var albumSongs = groupedQueryresults[i].Select(t => t);
                            var firstSong = albumSongs.First() ?? new Mediafile();
                            Album album = new Album
                            {
                                Artist = firstSong?.LeadArtist,
                                AlbumName = groupedQueryresults[i].Key,
                                AlbumArt = string.IsNullOrEmpty(firstSong?.AttachedPicture) ? null : firstSong?.AttachedPicture
                            };
                            QueryAlbums.Add(album);
                        }
                    }
                }
            }
        }
    }
}
