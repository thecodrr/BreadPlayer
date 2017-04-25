using BreadPlayer.Models;
using BreadPlayer.Database;
using BreadPlayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        ThreadSafeObservableCollection<Mediafile> querySongs;
        public ThreadSafeObservableCollection<Mediafile> QuerySongs
        {
            get => querySongs;
            set => Set(ref querySongs, value);
        }
        ThreadSafeObservableCollection<Album> queryAlbums;
        public ThreadSafeObservableCollection<Album> QueryAlbums
        {
            get => queryAlbums;
            set => Set(ref queryAlbums, value);
        }
      
        public async Task<IEnumerable<Mediafile>> StartSearch(string query)
        {
            LibraryService service = new LibraryService(new KeyValueStoreDatabaseService(Core.SharedLogic.DatabasePath, "Tracks", "TracksText"));
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
                        Album album = new Album()
                        {
                            AlbumSongs = new System.Collections.ObjectModel.ObservableCollection<Mediafile>(albumSongs),
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
