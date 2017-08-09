using BreadPlayer.Core;
using BreadPlayer.Core.Models;
using BreadPlayer.Database;
using BreadPlayer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
        
        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> _mostPlayedCollection;
        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> MostPlayedSongsCollection
        {
            get => _mostPlayedCollection ?? new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
            set => Set(ref _mostPlayedCollection, value);
        }

    private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> _recentlyAddedSongsCollection;

        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> RecentlyAddedSongsCollection
        {
            get => _recentlyAddedSongsCollection ?? new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
            set => Set(ref _recentlyAddedSongsCollection, value);
        }

        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> _recentlyPlayedCollection;
        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> RecentlyPlayedCollection
        {
            get => _recentlyPlayedCollection ?? new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
            set => Set(ref _recentlyPlayedCollection, value);
        }

        public ThreadSafeObservableCollection<IGrouping<string, Mediafile>> GetMostPlayedSongsAsync()
        {
            var filtered = SettingsViewModel.TracksCollection.Elements.Where(t => (t as Mediafile).PlayCount > 1);
            if (filtered != null)
            {
                MostPlayedSongsCollection = new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
                MostPlayedSongsCollection.AddRange(filtered.GroupBy(t => "Under " + GetNearest10(t.PlayCount) + " Plays"));
            }
            return MostPlayedSongsCollection;            
        }
        private int GetNearest10(int n)
        {
            return (n + 9) - ((n + 9) % 10);
        }
        public ThreadSafeObservableCollection<IGrouping<string, Mediafile>> GetRecentlyPlayedSongsAsync()
        {
            var filtered = SettingsViewModel.TracksCollection.Elements.Where(t => t.LastPlayed != null
                                                                   && (DateTime.Now.Subtract(DateTime.Parse(t.LastPlayed)))
                                                                       .Days <= 14);
            if (filtered != null)
            {
                RecentlyPlayedCollection = new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
                RecentlyPlayedCollection.AddRange(filtered.GroupBy(t => DateTime.Parse(t.LastPlayed).ToString("D")));
            }
            return RecentlyPlayedCollection;
        }
        public ThreadSafeObservableCollection<IGrouping<string, Mediafile>> GetRecentlyAddedSongsAsync()
        {
            var filtered = SettingsViewModel.TracksCollection.Elements.Where(t => (t as Mediafile).AddedDate != null
                                                                     && (DateTime.Now.Subtract(DateTime.Parse((t as Mediafile).AddedDate))).Days < 7);
            if (filtered?.Any() == true)
            {
                RecentlyAddedSongsCollection = new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
                RecentlyAddedSongsCollection.AddRange(filtered.GroupBy(t => DateTime.Parse(t.AddedDate).ToString("D")));
            }
            return RecentlyAddedSongsCollection;
        }
    }
}
