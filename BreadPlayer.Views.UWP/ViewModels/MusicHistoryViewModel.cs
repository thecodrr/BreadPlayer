using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.ViewModels
{
    public class MusicHistoryViewModel : ObservableObject
    {
        LibraryService LibraryService { get; set; }
        public MusicHistoryViewModel()
        {
            LibraryService = new LibraryService(new DocumentStoreDatabaseService(SharedLogic.DatabasePath, "Tracks"));
        }
        
        private ThreadSafeObservableCollection<Mediafile> _mostEatenCollection;
        private ThreadSafeObservableCollection<Mediafile> MostEatenSongsCollection =>
            _mostEatenCollection ?? (_mostEatenCollection = new ThreadSafeObservableCollection<Mediafile>());

        private ThreadSafeObservableCollection<Mediafile> _recentlyAddedSongsCollection;

        private ThreadSafeObservableCollection<Mediafile> RecentlyAddedSongsCollection =>
            _recentlyAddedSongsCollection ?? (_recentlyAddedSongsCollection = new ThreadSafeObservableCollection<Mediafile>());

        private GroupedObservableCollection<string, Album> _recentlyPlayedCollection;
        private GroupedObservableCollection<string, Album> RecentlyPlayedCollection
        {
            get => _recentlyPlayedCollection ?? new GroupedObservableCollection<string, Album>(t => t.Artist);
            set => Set(ref _recentlyPlayedCollection, value);
        }

        public Task<ThreadSafeObservableCollection<Mediafile>> GetMostPlayedSongsAsync()
        {
            return Task.Run(async () =>
            {
                var filtered = await LibraryService.Query(null, t => (t as Mediafile).PlayCount > 1);
                if(filtered?.Any() == true)
                    MostEatenSongsCollection.AddRange(filtered);
                return MostEatenSongsCollection;
            });
        }

        public Task<ThreadSafeObservableCollection<Mediafile>> GetRecentlyPlayedSongsAsync()
        {
            return Task.Run(async () =>
            {
                var filtered = await LibraryService.Query(null, t => (t as Mediafile).LastPlayed != null
                                                                    && (DateTime.Now.Subtract(DateTime.Parse((t as Mediafile).LastPlayed)))
                                                                        .Days <= 14);             
            });
        }
        public Task<ThreadSafeObservableCollection<Mediafile>> GetRecentlyAddedSongsAsync()
        {
            return Task.Run(async () =>
            {
                var filtered = await LibraryService.Query(null, t => (t as Mediafile).AddedDate != null
                                                                    && (DateTime.Now.Subtract(DateTime.Parse((t as Mediafile).AddedDate))).Days < 3);
                if (filtered?.Any() == true)
                    RecentlyAddedSongsCollection.AddRange(filtered);
                return RecentlyAddedSongsCollection;
            });
        }
    }
}
