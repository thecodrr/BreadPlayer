using BreadPlayer.Core.Models;
using BreadPlayer.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task GetMostPlayedSongs()
        {
            await ChangeFilteredCollection(t => (t as Mediafile).PlayCount > 1,
                                    t => "Under " + GetNearest10(t.PlayCount) + " Plays").ConfigureAwait(false);
        }

        public async Task GetRecentlyPlayedSongs()
        {
            await ChangeFilteredCollection(t => t.LastPlayed != null && (DateTime.Now.Subtract(t.LastPlayed)).Days <= 14,
                                     t => t.LastPlayed.ToString("D")).ConfigureAwait(false);
        }

        public async Task GetRecentlyAddedSongs()
        {
            await ChangeFilteredCollection(t => (t as Mediafile).AddedDate != null && (DateTime.Now.Subtract(t.AddedDate)).Days < 7,
                                    t => t.AddedDate.ToString("D")).ConfigureAwait(false);
        }

        private async Task<ThreadSafeObservableCollection<IGrouping<string, Mediafile>>> ChangeFilteredCollection(Func<Mediafile, bool> filterFunc, Func<Mediafile, string> groupingFunc)
        {
            IEnumerable<Mediafile> filtered = null;
            await BreadDispatcher.InvokeAsync(() =>
            {
                filtered = SettingsViewModel.TracksCollection.Elements.Where(filterFunc);
                if (filtered?.Any() == true)
                {
                    CurrentCollection = new ThreadSafeObservableCollection<IGrouping<string, Mediafile>>(filtered.GroupBy(groupingFunc));
                }
            });
            return CurrentCollection;
        }

        private int GetNearest10(int n)
        {
            return (n + 9) - ((n + 9) % 10);
        }
    }
}