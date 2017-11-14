using BreadPlayer.Core.Models;
using BreadPlayer.Extensions;
using BreadPlayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Helpers
{
    public static class TrackCollectionHelper
    {
        public static int GetCurrentlyPlayingIndex(this ThreadSafeObservableCollection<Mediafile> collection)
        {
            return collection?.IndexOf(GetCurrentlyPlaying(collection)) ?? 0;
        }
        public static Mediafile GetCurrentlyPlaying(this ThreadSafeObservableCollection<Mediafile> collection)
        {
            return collection?.FirstOrDefault(t => t.State == Core.Common.PlayerState.Playing) ?? null;
        }
        public static Mediafile GetSongInCollection(this ThreadSafeObservableCollection<Mediafile> collection, Mediafile song)
        {
            return collection?.FirstOrDefault(t => t.Id == song.Id) ?? null;
        }
        public static Mediafile GetSongByPath(this ThreadSafeObservableCollection<Mediafile> collection, string path)
        {
            return collection?.FirstOrDefault(t => t.Path == path) ?? null;
        }
        public static bool IsPlayingCollection(this ThreadSafeObservableCollection<Mediafile> collection)
        {
            return collection?.Any(t => t.State == Core.Common.PlayerState.Playing) == true;
        }
        public static Grouping<IGroupKey, Mediafile> GetCurrentlyPlayingGroup(this GroupedObservableCollection<IGroupKey, Mediafile> collection)
        {
            return collection?.FirstOrDefault(t => t.Any(c => c.State == Core.Common.PlayerState.Playing));
        }
        public static int GetCurrentlyPlayingGroupIndex(this GroupedObservableCollection<IGroupKey, Mediafile> collection)
        {
            return collection?.IndexOf(GetCurrentlyPlayingGroup(collection)) ?? 0;
        }
        public static int GetPlayingSongIndexInGroup(this Grouping<IGroupKey, Mediafile> group)
        {
            return group?.IndexOf(group.FirstOrDefault(t => t.State == Core.Common.PlayerState.Playing)) ?? 0;
        }
        public static Grouping<IGroupKey, Mediafile> GetPreviousGroup(this GroupedObservableCollection<IGroupKey, Mediafile> collection)
        {
            return collection?.ElementAt(GetCurrentlyPlayingGroupIndex(collection) - 1);
        }
        public static Grouping<IGroupKey, Mediafile> GetNextGroup(this GroupedObservableCollection<IGroupKey, Mediafile> collection)
        {
            return collection?.ElementAt(GetCurrentlyPlayingGroupIndex(collection) + 1);
        }
    }
}
