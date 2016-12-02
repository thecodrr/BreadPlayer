using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BreadPlayer.Extensions
{
    public class SortedObservableCollection<T>
        : ObservableCollection<T> where T : IComparable<T>
    {
        private CoreDispatcher _dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

        protected volatile bool _isObserving = true;

        protected async override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    if (_isObserving)
                        base.OnCollectionChanged(e);
                }
                catch { }
            });
           
        }
        protected async override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
          await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { if (_isObserving) base.OnPropertyChanged(e); });
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> range)
        {
            try
            {
                // get out if no new items
                if (range == null || !range.Any()) return;

                // prepare data for firing the events
                int newStartingIndex = Count;
                var newItems = new List<T>();
                newItems.AddRange(range);

                // add the items, making sure no events are fired

                _isObserving = false;
                foreach (var item in range)
                {
                    Add(item);
                }
                _isObserving = true;

                // fire the events
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
                // LOLLO NOTE I took out the following so the list viewers don't lose the position.
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems: newItems, startingIndex: newStartingIndex));
            }
            catch { }
        }
        protected async override void InsertItem(int index, T item)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (this.Count == 0)
                {
                    base.InsertItem(0, item);
                    return;
                }

                index = Compare(item, 0, this.Count - 1);

                base.InsertItem(index, item);
            });
        }

        private int Compare(T item, int lowIndex, int highIndex)
        {
            int compareIndex = (lowIndex + highIndex) / 2;

            if (compareIndex == 0)
            {
                return SearchIndexByIteration(lowIndex, highIndex, item);
            }

            int result = item.CompareTo(this[compareIndex]);

            if (result < 0)
            {   //item precedes indexed obj in the sort order

                if ((lowIndex + compareIndex) < 100 || compareIndex == (lowIndex + compareIndex) / 2)
                {
                    return SearchIndexByIteration(lowIndex, compareIndex, item);
                }

                return Compare(item, lowIndex, compareIndex);
            }

            if (result > 0)
            {   //item follows indexed obj in the sort order

                if ((compareIndex + highIndex) < 100 || compareIndex == (compareIndex + highIndex) / 2)
                {
                    return SearchIndexByIteration(compareIndex, highIndex, item);
                }

                return Compare(item, compareIndex, highIndex);
            }

            return compareIndex;
        }

        /// <summary>
        /// Iterates through sequence of the collection from low to high index
        /// and returns the index where to insert the new item
        /// </summary>
        private int SearchIndexByIteration(int lowIndex, int highIndex, T item)
        {
            for (int i = lowIndex; i <= highIndex; i++)
            {
                if (item.CompareTo(this[i]) < 0)
                {
                    return i;
                }
            }
            return this.Count;
        }

        /// <summary>
        /// Adds the item to collection by ignoring the index
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            this.InsertItem(index, item);
        }

        private const string _InsertErrorMessage
           = "Inserting and moving an item using an explicit index are not support by sorted observable collection";
    }
}
 