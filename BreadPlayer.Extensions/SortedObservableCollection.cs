using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BreadPlayer.Extensions
{
    public class SortedObservableCollection<T, TKey>
        : ThreadSafeObservableCollection<T>
    {
        private const string CountName = nameof(Count);
        private const string IndexerName = "Item[]";
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
        public void AddSorted(T item, bool notify)
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
            if (notify)
                Insert(i, item);
            else
                Items.Insert(i, item);
        }
        
        public async void OnCollectionReset() => await InitializeSwitch.Dispatcher.RunAsync(() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));

        public void NotifyProperties(bool count = true)
        {
            if (count)
                OnPropertyChanged(new PropertyChangedEventArgs(CountName));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
        }       
    }
}