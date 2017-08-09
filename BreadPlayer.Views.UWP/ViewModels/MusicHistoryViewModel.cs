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
        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> currentCollection;
        public ThreadSafeObservableCollection<IGrouping<string, Mediafile>> CurrentCollection
        {
            get => currentCollection ?? new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>();
            set => Set(ref currentCollection, value);
        }

        public void GetMostPlayedSongs()
        {
            ChangeFilteredCollection(t => (t as Mediafile).PlayCount > 1,
                                    t => "Under " + GetNearest10(t.PlayCount) + " Plays");
        }
        public void GetRecentlyPlayedSongs()
        {
            ChangeFilteredCollection(t => t.LastPlayed != null && (DateTime.Now.Subtract(DateTime.Parse(t.LastPlayed))).Days <= 14,
                                     t => DateTime.Parse(t.LastPlayed).ToString("D"));
        }
        public void GetRecentlyAddedSongs()
        {
            ChangeFilteredCollection(t => (t as Mediafile).AddedDate != null && (DateTime.Now.Subtract(DateTime.Parse((t as Mediafile).AddedDate))).Days < 7,
                                    t => DateTime.Parse(t.AddedDate).ToString("D"));
        }
        private ThreadSafeObservableCollection<IGrouping<string, Mediafile>> ChangeFilteredCollection(Func<Mediafile, bool> filterFunc, Func<Mediafile, string> groupingFunc)
        {
            var filtered = SettingsViewModel.TracksCollection.Elements.Where(filterFunc);
            if (filtered?.Any() == true)
            {
                CurrentCollection = new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>(filtered.GroupBy(groupingFunc));
            }
            return CurrentCollection;
        }
        private int GetNearest10(int n)
        {
            return (n + 9) - ((n + 9) % 10);
        }
    }
}
