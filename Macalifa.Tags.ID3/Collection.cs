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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Linq;
namespace BreadPlayer.Tags.ID3.ID3v2Frames
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
        /// Convert current collection to FrameType array
        /// </summary>
        /// <returns>FrameType array</returns>
        public FrameType[] ToArray()
        {
            Array arr = null;
            base.CopyTo(arr, 0);
            return (FrameType[])arr;
        }

        /// <summary>
        /// Gets type of current frame 
        /// </summary>
        /// <returns>System.Type</returns>
        protected override Type OnGetType()
        {
            return typeof(FrameType);
        }
        
    }

    /// <summary>
    /// A base class for frame collection class
    /// </summary>
    public class FrameCollectionBase : SortedList
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
                foreach (ILengthable IL in base.Values)
                    Len += IL.Length;
                return Len;
            }
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
        
    }
}