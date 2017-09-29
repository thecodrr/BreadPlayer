using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BreadPlayer.Extensions
{
    public class SortedObservableCollection<T, TKey>
        : ThreadSafeObservableCollection<T>
    {
        private CoreDispatcher _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

        /// <summary>
		/// Creates a new SortedObservableCollection instance.
		/// </summary>
		/// <param name="keySelector">The function to select the sorting key.</param>
		public SortedObservableCollection(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector;
            _comparer = Comparer<TKey>.Default;
        }

        private Func<T, TKey> _keySelector;
        private IComparer<TKey> _comparer;

        /// <summary>
        /// Adds an item to a sorted collection.
        /// </summary>
        public async void AddSorted(T item)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                int i = 0;
                int j = Count - 1;

                while (i <= j)
                {
                    int n = (i + j) / 2;
                    int c = _comparer.Compare(_keySelector(item), _keySelector(this[n]));

                    if (c == 0) { i = n; break; }
                    if (c > 0)
                    {
                        i = n + 1;
                    }
                    else
                    {
                        j = n - 1;
                    }
                }

                Insert(i, item);
            });
        }

        public void AddSortedRange(IEnumerable<T> range)
        {
            try
            {
                // get out if no new items
                if (range == null || !range.Any())
                {
                    return;
                }

                // add the items, making sure no events are fired

                _isObserving = false;
                var objectArray = range.ToArray();
                for (int i = 0; i < objectArray.Count(); i++)
                {
                    AddSorted(objectArray[i]);
                }
                _isObserving = true;

                // fire the events
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
                // LOLLO NOTE I took out the following so the list viewers don't lose the position.
                //if(reset)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
                // OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while adding range to TSCollection.", ex);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (_isObserving)
                {
                    base.OnCollectionChanged(e);
                }
            }
            catch (Exception ex)
            {
                BLogger.Logger.Error("Error occured while updating TSCollection on collectionchanged.", ex);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_isObserving)
            {
                base.OnPropertyChanged(e);
            }
        }
    }
}