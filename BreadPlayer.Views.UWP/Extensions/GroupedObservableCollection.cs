/*
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using BreadPlayer.Core.Models;
using BreadPlayer.Dispatcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BreadPlayer.Extensions
{
    public class GroupedObservableCollection<TKey, TElement> : ObservableCollection<Grouping<TKey, TElement>>
        where TKey : IComparable<TKey>
        where TElement : IComparable<TElement>
    {
        private readonly Func<TElement, TKey> _readKey;

        /// <summary>
        /// This is used as an optimisation for when items are likely to be added in key order and there is a good probability
        /// that when an item is added, then next one will be in the same grouping.
        /// </summary>
        private Grouping<TKey, TElement> _lastEffectedGroup;

        private bool _isObserving;

        public GroupedObservableCollection(Func<TElement, TKey> readKey)
        {
            _readKey = readKey;
        }

        public GroupedObservableCollection(Func<TElement, TKey> readKey, IEnumerable<TElement> items)
            : this(readKey)
        {
            //this.readKey = readKey;
            Elements = new SortedObservableCollection<TElement, string>(t => (t as Mediafile).Title);
            Elements.AddRange(items);
            // this.AddRange(items, false);
            foreach (var item in items)
            {
                AddItem(item);
            }
            //foreach (var item in items)
            //{
            //    this.AddItem(item, false);
            //}
        }

        public bool Contains(TElement item)
        {
            return Contains(item, (a, b) => a.Equals(b));
        }

        public new void Clear()
        {
            RemoveGroups(Keys);
            Elements.Clear();
        }

        public bool Contains(TElement item, Func<TElement, TElement, bool> compare)
        {
            var key = _readKey(item);
            var group = TryFindGroup(key);
            return group != null && group.Any(i => compare(item, i));
        }

        public IEnumerable<TElement> EnumerateItems()
        {
            return this.SelectMany(g => g);
        }

        public void AddItem(TElement item, bool addToElement = false)
        {
            try
            {
                var key = _readKey(item);
                if (addToElement)
                {
                    Elements.AddSorted(item);
                }

                var s = FindOrCreateGroup(key);
                s.Add(item);
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while adding file to grouped collection.", ex);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// </summary>
        public async Task AddRange(IEnumerable<TElement> range, bool addkey = false, bool async = true)
        {
            try
            {
                // get out if no new items
                if (range == null || !range.Any())
                {
                    return;
                }

                if (!addkey)
                {
                    Elements.AddSortedRange(range);
                }
                // add the items, making sure no events are fired
                _isObserving = false;

                var objectArray = range.ToArray();
                if (async)
                {
                    await Task.Run(() =>
                    {
                        AddItems(objectArray);
                    });
                }
                else
                {
                    AddItems(objectArray);
                }

                _isObserving = true;
                // fire the events
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
                // LOLLO NOTE I took out the following so the list viewers don't lose the position.
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));

                void AddItems(TElement[] array)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        AddItem(array[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while adding range to grouped collection.", ex);
            }
        }

        protected async override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (_isObserving)
                {
                    await BreadDispatcher.InvokeAsync(() => base.OnCollectionChanged(e));
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while updating collection on collectionchanged.", ex);
                Debug.Write("Error Code: " + ex.HResult + ";  Error Message: " + ex.Message + "\r\n");
            }
        }

        protected async override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                if (_isObserving)
                {
                    await BreadDispatcher.InvokeAsync(() => base.OnPropertyChanged(e));
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while updating grouped collection on property changed.", ex);
                Debug.Write("Error Code: " + ex.HResult + ";  Error Message: " + ex.Message + "\r\n");
            }
        }

        public void RemoveItem(TElement item)
        {
            Remove(item);
            Elements.Remove(item);
        }

        public IEnumerable<TKey> Keys => this.Select(i => i.Key);
        public SortedObservableCollection<TElement, string> Elements { get; set; } = new SortedObservableCollection<TElement, string>(t => (t as Mediafile).Title);

        public GroupedObservableCollection<TKey, TElement> ReplaceWith(GroupedObservableCollection<TKey, TElement> replacementCollection, IEqualityComparer<TElement> itemComparer)
        {
            // First make sure that the top level group containers match
            var replacementKeys = replacementCollection.Keys.ToList();
            var currentKeys = new HashSet<TKey>(Keys);
            RemoveGroups(currentKeys.Except(replacementKeys));
            EnsureGroupsExist(replacementKeys);

            // Debug.Assert(this.Keys.SequenceEqual(replacementCollection.Keys), "Expected this collection to have exactly the same keys in the same order at this point");

            for (var i = 0; i < replacementCollection.Count; i++)
            {
                MergeGroup(this[i], replacementCollection[i], itemComparer);
            }

            return this;
        }

        private static void MergeGroup(Grouping<TKey, TElement> current, Grouping<TKey, TElement> replacement, IEqualityComparer<TElement> itemComparer)
        {
            // Shortcut the matching and reordering process if the sequences are the same
            if (current.SequenceEqual(replacement, itemComparer))
            {
                return;
            }

            // First remove any items that are not present in the replacement
            var resultSet = new HashSet<TElement>(replacement, itemComparer);
            foreach (var toRemove in current.Except(resultSet, itemComparer).ToList())
            {
                current.Remove(toRemove);
            }

            //Debug.Assert(new HashSet<TElement>(current, itemComparer).IsSubsetOf(replacement), "Expected the current group to be a subset of the replacement group");

            // var currentItemIndexes = current.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);
            // var replacementItemIndexes = replacement.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);

            var currentItemSet = new HashSet<TElement>(current, itemComparer);
            for (var i = 0; i < replacement.Count; i++)
            {
                var findElement = replacement[i];

                if (i >= current.Count || !itemComparer.Equals(current[i], findElement))
                {
                    if (currentItemSet.Contains(findElement))
                    {
                        // The current set already contains the item, but it's in a different position
                        // Find out where it is in the current collection and move it from there
                        // NOTE this isn't very optimal if large sets are being reordered a lot, but the general use case for
                        // this sort of list is that there is some inherent order that won't be changing.

                        for (var j = i + 1; j < current.Count; j++)
                        {
                            if (itemComparer.Equals(current[j], findElement))
                            {
                                current.Move(i, j);
                                break;
                            }
                        }

                        // Debug.Assert(moved, "Expected that the element should have been moved");
                    }
                    else
                    {
                        // This is a new element, insert it here
                        current.Insert(i, replacement[i]);
                    }
                }
            }
        }

        private void EnsureGroupsExist(IList<TKey> requiredKeys)
        {
            //  Debug.Assert(new HashSet<TKey>(this.Keys).IsSubsetOf(requiredKeys), "Expected this collection to contain no additional keys than the new collection at this point");

            if (Count == requiredKeys.Count)
            {
                // Nothing to do.
                return;
            }

            for (var i = 0; i < requiredKeys.Count; i++)
            {
                if (Count <= i || !this[i].Key.Equals(requiredKeys[i]))
                {
                    Insert(i, new Grouping<TKey, TElement>(requiredKeys[i]));
                }
            }
        }

        private void RemoveGroups(IEnumerable<TKey> keys)
        {
            var keySet = new HashSet<TKey>(keys);
            for (var i = Count - 1; i >= 0; i--)
            {
                if (keySet.Contains(this[i].Key))
                {
                    RemoveAt(i);
                }
            }
        }

        public void RemoveGroup(TKey key)
        {
            RemoveAt(IndexOf(this.First(t => t.Key.ToString() == key.ToString())));
        }

        public bool Remove(TElement item)
        {
            var key = _readKey(item);
            var group = TryFindGroup(key);
            var success = group != null && group.Remove(item);

            if (group != null && group.Count == 0)
            {
                Remove(group);
                _lastEffectedGroup = null;
            }

            return success;
        }

        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            if (_lastEffectedGroup != null && _lastEffectedGroup.Key.Equals(key))
            {
                return _lastEffectedGroup;
            }

            var group = this.FirstOrDefault(i => i.Key.Equals(key));

            _lastEffectedGroup = group;

            return group;
        }

        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            Grouping<TKey, TElement> result = null;

            if (_lastEffectedGroup != null && _lastEffectedGroup.Key.Equals(key))
            {
                return _lastEffectedGroup;
            }

            try
            {
                var match = this.Select((group, index) => new { group, index }).FirstOrDefault(i => i.group.Key.CompareTo(key) >= 0);

                if (match == null)
                {
                    //_isObserving = true;
                    // Group doesn't exist and the new group needs to go at the end
                    result = new Grouping<TKey, TElement>(key);
                    Add(result);
                }
                else if (!match.group.Key.Equals(key))
                {
                    //_isObserving = true;
                    // Group doesn't exist, but needs to be inserted before an existing one
                    result = new Grouping<TKey, TElement>(key);
                    Insert(match.index, result);
                }
                else
                {
                    result = match.group;
                }

                _lastEffectedGroup = result;
            }
            catch (Exception ex)
            {
                BLogger.E("Error occured while finding or creating a group.", ex);
            }

            return result;
        }
    }
}

public class Grouping<TKey, TElement> : ThreadSafeObservableCollection<TElement>, IGrouping<TKey, TElement>
{
    public Grouping(TKey key)
    {
        Key = key;
    }

    public Grouping(TKey key, IEnumerable<TElement> items)
        : this(key)
    {
        AddRange(items);

        //foreach (var item in items)
        //{
        //    this.Add(item);
        //}
    }

    public TKey Key { get; }
}