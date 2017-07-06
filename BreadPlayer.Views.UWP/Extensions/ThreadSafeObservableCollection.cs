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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

/// <summary>
/// a thread safe observable collection using reader writer lock - created with the help of http://web.archive.org/web/20101105144104/http://www.deanchalk.me.uk/post/Thread-Safe-Dispatcher-Safe-Observable-Collection-for-WPF.aspx
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadSafeObservableCollection<T> : ObservableCollection<T>, INotifyCollectionChanged
{
    protected volatile bool _isObserving = true;
    public bool IsObserving { get => _isObserving;
        set => _isObserving = value;
    }

    //public static readonly int MAX_CAPACITY = int.MaxValue - 1; // MS limit
    //private readonly int _capacity = MAX_CAPACITY;
    //public int Capacity { get { return _capacity; } }
    private CoreDispatcher _dispatcher;
    internal ReaderWriterLockSlim Sync = new ReaderWriterLockSlim();
    public ThreadSafeObservableCollection()
    {
        _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
    }
    public ThreadSafeObservableCollection(IEnumerable<T> collection = null)
    {
        //copy the collection to ourself
        if (collection != null)
        {
            AddRange(collection);
        }
        _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
    }

    public async new void Add(T item)
    {
        if (_dispatcher == null)
        {
            _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        if (_dispatcher.HasThreadAccess)
        {
            DoAdd(item);
        }
        else
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoAdd(item));
        }
    }

    private void DoAdd(T item)
    {
        if (!Sync.IsWriteLockHeld)
        {
            Sync.EnterWriteLock();
        }

        base.Add(item);
        Sync.ExitWriteLock();
    }

    public async new void Clear()
    {
        if (_dispatcher.HasThreadAccess)
        {
            DoClear();
        }
        else
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, DoClear);
        }
    }

    protected async override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
        });
    }
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_isObserving)
        {
            base.OnPropertyChanged(e);
        }
    }

    /// <summary> 
    /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
    /// </summary> 
    public void AddRange(IEnumerable<T> range)
    {
        try
        {
            // get out if no new items
            if (range == null || !range.Any())
            {
                return;
            }

            _isObserving = false;
            var objectArray = range.ToArray();
            for (int i = 0; i < objectArray.Count(); i++)
            {
                Add(objectArray[i]);
            }
            _isObserving = true;

            // fire the events
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
            // LOLLO NOTE I took out the following so the list viewers don't lose the position.
            //if(reset)
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }
        catch (Exception ex)
        {
            BLogger.Logger.Error("Error occured while adding range to TSCollection.", ex);
        }
    }

    /// <summary> 
    /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
    /// </summary> 
    public void RemoveRange(IEnumerable<T> collection)
    {
        // get out if no new items
        if (collection == null || !collection.Any())
        {
            return;
        }

        // add the items, making sure no events are fired
        _isObserving = false;
        foreach (var item in collection)
        {
            Remove(item);
        }
        _isObserving = true;

        // fire the events
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
        // LOLLO NOTE I took out the following so the list viewers don't lose the position.
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
        //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
    }

    /// <summary> 
    /// Clears the current collection and replaces it with the specified item. 
    /// </summary> 
    public void Replace(T item)
    {
        ReplaceRange(new[] { item });
    }

    /// <summary> 
    /// Clears the current collection and replaces it with the specified collection. 
    /// </summary> 
    public void ReplaceRange(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException("collection");
        }

        Sync.EnterWriteLock();
        Clear();
        foreach (var i in collection)
        {
            Insert(base.Count - 1, i);
        }

        Sync.ExitWriteLock();
    }

    private void DoClear()
    {
        if (!Sync.IsWriteLockHeld)
        {
            Sync.EnterWriteLock();
        }

        base.Clear();
        Sync.ExitWriteLock();
    }

    public new bool Contains(T item)
    {
        if (!Sync.IsReadLockHeld)
        {
            Sync.EnterReadLock();
        }

        var result = base.Contains(item);
        Sync.ExitReadLock();
        return result;
    }

    public new void CopyTo(T[] array, int arrayIndex)
    {
        Sync.EnterWriteLock();
        base.CopyTo(array, arrayIndex);
        Sync.ExitWriteLock();
    }

    public new int Count
    {
        get
        {
            if (!Sync.IsWriteLockHeld)
            {
                Sync.EnterReadLock();
            }

            var result = base.Count;
            if (Sync.IsReadLockHeld)
            {
                Sync.ExitReadLock();
            }

            return result;
        }
    }




    public new bool Remove(T item)
    {
        if (_dispatcher.HasThreadAccess)
        {
            return DoRemove(item);
        }

        bool? op = null;
        var removeTask = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            op = DoRemove(item);
        });
        removeTask.AsTask().Wait();
        if (op == null)
        {
            return false;
        }

        return op.Value;
    }

    private bool DoRemove(T item)
    {
        Sync.EnterWriteLock();
        var result = base.Remove(item);
        Sync.ExitWriteLock();
        return result;
    }


    public new int IndexOf(T item)
    {
        Sync.EnterReadLock();
        var result = base.IndexOf(item);
        Sync.ExitReadLock();
        return result;
    }

    public new async void Insert(int index, T item)
    {
        if (_dispatcher.HasThreadAccess)
        {
            DoInsert(index, item);
        }
        else
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoInsert(index, item));
        }
    }

    private void DoInsert(int index, T item)
    {
        if (!Sync.IsWriteLockHeld)
        {
            Sync.EnterWriteLock();
        }

        base.Insert(index, item);
        Sync.ExitWriteLock();
    }

    public new async void RemoveAt(int index)
    {
        if (_dispatcher.HasThreadAccess)
        {
            DoRemoveAt(index);
        }
        else
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoRemoveAt(index));
        }
    }

    private void DoRemoveAt(int index)
    {
        Sync.EnterWriteLock();
        if (base.Count == 0 || base.Count <= index)
        {
            Sync.ExitWriteLock();
            return;
        }

        base.RemoveAt(index);
        Sync.ExitWriteLock();
    }

    public new T this[int index]
    {
        get
        {
            Sync.EnterReadLock();
            var result = base[index];
            Sync.ExitReadLock();
            return result;
        }
        set
        {
            Sync.EnterWriteLock();
            if (base.Count == 0 || base.Count <= index)
            {
                Sync.ExitWriteLock();
                return;
            }
            base[index] = value;
            Sync.ExitWriteLock();
        }
    }

    public IEnumerable<T> AsLocked()
    {
        return new ThreadSafeObservableCollectionEnumerableWrapper<T>(this);
    }
}

public class ThreadSafeObservableCollectionEnumerableWrapper<T> : IEnumerable<T>
{
    private readonly ThreadSafeObservableCollection<T> _mInner;


    public ThreadSafeObservableCollectionEnumerableWrapper(ThreadSafeObservableCollection<T> observable)
    {

        _mInner = observable;
    }

    #region Implementation of IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new SafeReaderWriterEnumerator<T>(_mInner, _mInner.Sync);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}

/// <summary>
/// Created with the help of http://www.codeproject.com/Articles/56575/Thread-safe-enumeration-in-C 
/// provides a ThreadSafe enumerator for ThreadSafeObservableCollection
/// </summary>
/// <typeparam name="T"></typeparam>
public class SafeReaderWriterEnumerator<T> : IEnumerator<T>
{
    // this is the (thread-unsafe)
    // enumerator of the underlying collection
    private readonly IEnumerator<T> _mInner;
    // this is the object we shall lock on. 
    private ReaderWriterLockSlim _mLock;

    public SafeReaderWriterEnumerator(ThreadSafeObservableCollection<T> inner, ReaderWriterLockSlim @lock)
    {
        _mLock = @lock;
        // entering lock in constructor
        _mLock.EnterReadLock();
        _mInner = inner.GetEnumerator();

    }

    #region Implementation of IDisposable

    public void Dispose()
    {
        // .. and exiting lock on Dispose()
        // This will be called when foreach loop finishes
        _mLock.ExitReadLock();
        _mInner.Dispose();
    }

    #endregion

    #region Implementation of IEnumerator

    // we just delegate actual implementation
    // to the inner enumerator, that actually iterates
    // over some collection

    public bool MoveNext()
    {
        return _mInner.MoveNext();
    }

    public void Reset()
    {
        _mInner.Reset();
    }

    public T Current => _mInner.Current;


    object IEnumerator.Current => _mInner.Current;

    #endregion
}
