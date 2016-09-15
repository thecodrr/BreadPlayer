using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;

namespace Macalifa.Tags.ID3.ID3v2Frames
{
    /// <summary>
    /// Provide collection for frames to use for filtering
    /// </summary>
    public class FilterCollection
    {
        private ArrayList _Frames;

        /// <summary>
        /// Create new cleared filter collection
        /// </summary>
        public FilterCollection()
        {
            _Frames = new ArrayList();
        }

        /// <summary>
        /// Add FrameID to FrameList if not exists
        /// </summary>
        /// <param name="FrameID">FrameID to add to list</param>
        public void Add(string FrameID)
        {
            if (!_Frames.Contains(FrameID))
                _Frames.Add(FrameID);
        }

        /// <summary>
        /// Remove Specific frame from list
        /// </summary>
        /// <param name="FrameID">FrameID to remove from list</param>
        public void Remove(string FrameID)
        {
            _Frames.Remove(FrameID);
        }

        /// <summary>
        /// Get list of frames
        /// </summary>
        public string[] Frames
        {
            get
            { return (string[])_Frames.ToArray(typeof(string)); }
        }

        /// <summary>
        /// Remove all frames from frame list
        /// </summary>
        public void Clear()
        {
            _Frames.Clear();
        }

        /// <summary>
        /// Indicate is specific frame in the list
        /// </summary>
        /// <param name="FrameID">FrameID to search</param>
        /// <returns>true if exists false if not</returns>
        public bool IsExists(string FrameID)
        {
            if (_Frames.Contains(FrameID))
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// A class for frames collection
    /// </summary>
    /// <typeparam name="FrameType"></typeparam>
    public class FrameCollection<FrameType> : FrameCollectionBase
    {
        /// <summary>
        /// Create a new FrameCollection class
        /// </summary>
        /// <param name="Name">Name of collection</param>
        public FrameCollection(string Name)
            : base(Name) { }

        /// <summary>
        /// Add new Item to current FrameCollection
        /// </summary>
        /// <param name="item">item to add to collection</param>
        public void Add(FrameType item)
        {
            InnerList.Remove(item);
            List.Add(item);
        }

        /// <summary>
        /// remove specific item from current FrameCollection
        /// </summary>
        /// <param name="item">item to remove from collection</param>
        public void Remove(FrameType item)
        {
            List.Remove(item);
        }

        /// <summary>
        /// Convert current collection to FrameType array
        /// </summary>
        /// <returns>FrameType array</returns>
        public FrameType[] ToArray()
        {
            return (FrameType[])InnerList.ToArray(typeof(FrameType));
        }

        /// <summary>
        /// Gets type of current frame 
        /// </summary>
        /// <returns>System.Type</returns>
        protected override Type OnGetType()
        {
            return typeof(FrameType);
        }

        /// <summary>
        /// Gets specific frame from list
        /// </summary>
        /// <param name="index">index of frame in list</param>
        /// <returns>Frame</returns>
        public FrameType this[int index]
        {
            get
            { return (FrameType)this.List[index]; }
        }
    }

    /// <summary>
    /// A base class for frame collection class
    /// </summary>
    public class FrameCollectionBase : CollectionBase
    {
        private string _Name;

        /// <summary>
        /// Create new FrameCollectionBase class
        /// </summary>
        /// <param name="Name">Name of collection</param>
        public FrameCollectionBase(string Name)
        {
            _Name = Name;
        }

        /// <summary>
        /// Sort Items
        /// </summary>
        public void Sort()
        {
            InnerList.Sort();
        }

        /// <summary>
        /// Name of current collection
        /// </summary>
        public string Name
        {
            get
            { return _Name; }
        }

        /// <summary>
        /// Get sum of lengths of items
        /// </summary>
        public int Length
        {
            get
            {
                int Len = 0;
                foreach (ILengthable IL in List)
                    Len += IL.Length;
                return Len;
            }
        }

        /// <summary>
        /// Convert current collection to Frame array
        /// </summary>
        /// <returns>Frame array</returns>
        public virtual Frame[] ToFrameArray()
        {
            return (Frame[])InnerList.ToArray(typeof(Frame));
        }

        /// <summary>
        /// Get type of items in collection
        /// </summary>
        public Type CollectionType
        {
            get
            { return OnGetType(); }
        }

        /// <summary>
        /// Gets type of current frame 
        /// </summary>
        /// <returns>System.Type</returns>
        protected virtual Type OnGetType()
        {
            return typeof(Frame);
        }

        /// <summary>
        /// Add specific frame to collection
        /// </summary>
        /// <param name="item">Frame to add to collection</param>
        public void Add(Frame item)
        {
            List.Add(item);
        }

        /// <summary>
        /// Remove specific frame from list
        /// </summary>
        /// <param name="item">Frame to remove from list</param>
        public void Remove(Frame item)
        {
            if (List.Contains(item))
                List.Remove(item);
        }

        /// <summary>
        /// Indicate if list contains specific frame
        /// </summary>
        /// <param name="item">Frame to search for</param>
        /// <returns>true if exists otherwise false</returns>
        public bool Contains(Frame item)
        { 
            foreach(Frame F in List)
                if(F.Equals(item))
                    return true;
            return false;
        }
    }
}