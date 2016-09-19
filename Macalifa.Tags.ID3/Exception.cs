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
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Static class contain error occured
    /// </summary>
    public class ExceptionCollection : CollectionBase
    {
        /// <summary>
        /// Create new Exception collection class
        /// </summary>
        public ExceptionCollection() { }

        /// <summary>
        /// Add new exception to current collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(Exception item)
        { InnerList.Add(item); }

        /// <summary>
        /// Get list of Errors
        /// </summary>
        public Exception[] ToArray()
        {
            return (Exception[])InnerList.ToArray(typeof(Exception));
        }
    }

    /// <summary>
    /// Exception class for ASF reading and writing
    /// </summary>
    public class ASFException : Exception
    {
        private string _GUID;
        private ExceptionLevels _Level;

        /// <summary>
        /// Create new ASFException
        /// </summary>
        /// <param name="message">message of exception</param>
        /// <param name="GUID">GUID</param>
        /// <param name="Level">Level of exception</param>
        public ASFException(string message, string GUID, ExceptionLevels Level)
            : base(message)
        {
            _GUID = GUID;
            _Level = Level;
        }

        /// <summary>
        /// Level of exception that occured
        /// </summary>
        public ExceptionLevels Level
        {
            get
            { return _Level; }
        }

        /// <summary>
        /// GUID that exception occured in
        /// </summary>
        public string GUID
        {
            get
            { return _GUID; }
        }
    }

    /// <summary>
    /// Exception class for exceptions that occur in frames
    /// </summary>
    public class ID3Exception : Exception
    {
        private string _FrameID;
        private ExceptionLevels _Level;

        /// <summary>
        /// Create new TagException
        /// </summary>
        /// <param name="message">message of exception</param>
        /// <param name="FrameID">FrameID that exception occured in</param>
        /// <param name="ExceptionLevel">Exception Level for current exception</param>
        public ID3Exception(string message, string FrameID, ExceptionLevels ExceptionLevel)
            : base(message)
        {
            _FrameID = FrameID;
            _Level = ExceptionLevel;
        }

        /// <summary>
        /// Create ne tag exception
        /// </summary>
        /// <param name="message">Message of exception</param>
        /// <param name="ExceptionLevel">Exception level</param>
        public ID3Exception(string message, ExceptionLevels ExceptionLevel)
            : this(message, "", ExceptionLevel) { }

        /// <summary>
        /// FrameID that current exception occured in
        /// </summary>
        public string FrameID
        {
            get
            { return _FrameID; }
        }

        /// <summary>
        /// Get Exception level for current exception
        /// </summary>
        public ExceptionLevels Level
        {
            get
            { return _Level; }
        }
    }

    /// <summary>
    /// Handle mode of exceptions
    /// </summary>
    public enum ExceptionLevels
    {
        /// <summary>
        /// Show MessageBox
        /// </summary>
        Warning,
        /// <summary>
        /// Try to repair
        /// </summary>
        Repaired,
        /// <summary>
        /// Throw exception
        /// </summary>
        Error
    }
}
