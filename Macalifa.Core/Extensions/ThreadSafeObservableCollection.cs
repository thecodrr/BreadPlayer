/* 
	Macalifa. A music player made for Windows 10 store.
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;


/// <summary>
/// a thread safe observable collection using reader writer lock - created with the help of http://web.archive.org/web/20101105144104/http://www.deanchalk.me.uk/post/Thread-Safe-Dispatcher-Safe-Observable-Collection-for-WPF.aspx
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
{
    private CoreDispatcher _dispatcher;
    internal ReaderWriterLockSlim sync = new System.Threading.ReaderWriterLockSlim();
    public ThreadSafeObservableCollection()
    {
        _dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
    }
    public ThreadSafeObservableCollection(IEnumerable<T> collection = null)
    {
        //copy the collection to ourself
        if (collection != null)
        {
            foreach (var item in collection)
            {
                base.Add(item);
            }
        }
        _dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
    }

    public async new void Add(T item)
    {
        if (_dispatcher.HasThreadAccess)
            DoAdd(item);
        else
           await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoAdd(item));
    }

    private void DoAdd(T item)
    {
        sync.EnterWriteLock();
        base.Add(item);
        sync.ExitWriteLock();
    }

    public async new void Clear()
    {
        if (_dispatcher.HasThreadAccess)
            DoClear();
        else
           await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, DoClear);
    }
    /// <summary> 
    /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
    /// </summary> 
    public void AddRange(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException("collection");
        
        foreach (var i in collection)
            this.DoAdd(i);
    }

    /// <summary> 
    /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
    /// </summary> 
    public void RemoveRange(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException("collection");
        sync.EnterWriteLock();
        foreach (var i in collection)
            this.Remove(i);
        sync.ExitWriteLock();
    }

    /// <summary> 
    /// Clears the current collection and replaces it with the specified item. 
    /// </summary> 
    public void Replace(T item)
    {
        ReplaceRange(new T[] { item });
    }

    /// <summary> 
    /// Clears the current collection and replaces it with the specified collection. 
    /// </summary> 
    public void ReplaceRange(IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException("collection");
        sync.EnterWriteLock();
        this.Clear();
        foreach (var i in collection)
            this.Insert(base.Count - 1, i);
        sync.ExitWriteLock();
    }
    
    private void DoClear()
    {
        sync.EnterWriteLock();
        base.Clear();
        sync.ExitWriteLock();
    }

    public new bool Contains(T item)
    {
        sync.EnterReadLock();
        var result = base.Contains(item);
        sync.ExitReadLock();
        return result;
    }

    public new void CopyTo(T[] array, int arrayIndex)
    {
        sync.EnterWriteLock();
        base.CopyTo(array, arrayIndex);
        sync.ExitWriteLock();
    }

    public new int Count
    {
        get
        {
            sync.EnterReadLock();
            var result = base.Count;
            sync.ExitReadLock();
            return result;
        }
    }




    public new bool Remove(T item)
    {
        if (_dispatcher.HasThreadAccess)
            return DoRemove(item);
        else
        {
            bool? op = null;
            var removeTask = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                op = DoRemove(item);
            });
            removeTask.AsTask().Wait();
            if (op == null)
                return false;
            return op.Value;
        }
    }

    private bool DoRemove(T item)
    {
        sync.EnterWriteLock();
        var result = base.Remove(item);
        sync.ExitWriteLock();
        return result;

    }


    public new int IndexOf(T item)
    {
        sync.EnterReadLock();
        var result = base.IndexOf(item);
        sync.ExitReadLock();
        return result;


    }

    public new async void Insert(int index, T item)
    {
        if (_dispatcher.HasThreadAccess)
            DoInsert(index, item);
        else
        {

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoInsert(index, item));
        }
    }

    private void DoInsert(int index, T item)
    {
        sync.EnterWriteLock();

        base.Insert(index, item);
        sync.ExitWriteLock();
    }

    public new async void RemoveAt(int index)
    {
        if (_dispatcher.HasThreadAccess)
            DoRemoveAt(index);
        else
           await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoRemoveAt(index));
    }

    private void DoRemoveAt(int index)
    {
        sync.EnterWriteLock();
        if (base.Count == 0 || base.Count <= index)
        {
            sync.ExitWriteLock();
            return;
        }

        base.RemoveAt(index);
        sync.ExitWriteLock();

    }

    public new T this[int index]
    {
        get
        {
            sync.EnterReadLock();
            var result = base[index];
            sync.ExitReadLock();
            return result;
        }
        set
        {
            sync.EnterWriteLock();
            if (base.Count == 0 || base.Count <= index)
            {
                sync.ExitWriteLock();
                return;
            }
            base[index] = value;
            sync.ExitWriteLock();
        }

    }

    public IEnumerable<T> AsLocked()
    {
        return new ThreadSafeObservableCollectionEnumerableWrapper<T>(this);
    }
}
    public class ThreadSafeObservableCollectionEnumerableWrapper<T> : IEnumerable<T>
{
    private readonly ThreadSafeObservableCollection<T> m_Inner;


    public ThreadSafeObservableCollectionEnumerableWrapper(ThreadSafeObservableCollection<T> observable)
    {

        m_Inner = observable;
    }

    #region Implementation of IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new SafeReaderWriterEnumerator<T>(m_Inner, m_Inner.sync);
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
    private readonly IEnumerator<T> m_Inner;
    // this is the object we shall lock on. 
    private ReaderWriterLockSlim m_lock;

    public SafeReaderWriterEnumerator(ThreadSafeObservableCollection<T> inner, ReaderWriterLockSlim @lock)
    {
        m_lock = @lock;
        // entering lock in constructor
        m_lock.EnterReadLock();
        m_Inner = inner.GetEnumerator();

    }

    #region Implementation of IDisposable

    public void Dispose()
    {
        // .. and exiting lock on Dispose()
        // This will be called when foreach loop finishes
        m_lock.ExitReadLock();
        m_Inner.Dispose();
    }

    #endregion

    #region Implementation of IEnumerator

    // we just delegate actual implementation
    // to the inner enumerator, that actually iterates
    // over some collection

    public bool MoveNext()
    {
        return m_Inner.MoveNext();
    }

    public void Reset()
    {
        m_Inner.Reset();
    }

    public T Current { get { return m_Inner.Current; } }


    object IEnumerator.Current
    {
        get { return m_Inner.Current; }
    }

    #endregion
}