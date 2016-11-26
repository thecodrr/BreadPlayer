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
using System.IO;

namespace BreadPlayer.Tags.ID3
{
	/// <summary>
	/// Provide ID3 Information for a file
	/// </summary>
	public class ID3Info : ITagInfo
    {
        private ID3v1 _ID3v1;
        private ID3v2 _ID3v2;

        /// <summary>
        /// Create new ID3 Info class
        /// </summary>
        /// <param name="FilePath">Path to read file</param>
        /// <param name="LoadData">Indicate load data in constructor or not</param>
        public ID3Info(bool LoadData, Stream FS)
        {
            _ID3v1 = new ID3v1(LoadData, FS);
            _ID3v2 = new ID3v2(LoadData, FS);
        }

        /// <summary>
        /// Get ID3 version 2 Tags
        /// </summary>
        public ID3v2 ID3v2Info
        {
            get
            { return _ID3v2; }
        }

        /// <summary>
        /// Gets ID3 version Tag
        /// </summary>
        public ID3v1 ID3v1Info
        {
            get
            { return _ID3v1; }
        }

        /// <summary>
        /// Load both ID3v1 and ID3v2 information from file
        /// </summary>
        public bool Load()
        {
            ID3v2Info.Load();
            ID3v1Info.Load();
            return true;
        }

        /// <summary>
        /// Save both ID3v2 and ID3v1
        /// </summary>
        public bool Save()
        {
            ID3v2Info.Save();
            ID3v1Info.Save();
            return true;
        }

        /// <summary>
        /// Return true if file had error while reading
        /// </summary>
        public bool HaveException
        {
            get { return ID3v2Info.HaveError; }
        }

        /// <summary>
        /// Save both ID3v2 and ID3v1
        /// </summary>
        /// <param name="Formula">Formula to rename file while saving</param>
        public bool Save(string Formula)
        {
            ID3v1Info.Save();
            ID3v2Info.Save(Formula);
            ID3v1Info.FilePath = ID3v2Info.FilePath;
            return true;
        }

        /// <summary>
        /// Save current ID3Info in specific location
        /// </summary>
        /// <param name="path">Path to save</param>
        /// <returns>True if save successfull</returns>
        public bool SaveAs(string path)
        {
            ID3v2Info.SaveAs(path);
            ID3v1Info.FilePath = ID3v2Info.FilePath;
            ID3v1Info.Save();
            return true;
        }

        /// <summary>
        /// Get filename according to specific formula
        /// </summary>
        /// <param name="Formula">Formula to make filename</param>
        /// <returns>System.String Contains filename</returns>
        public string MakeFileName(string Formula)
        {
            return ID3v2Info.MakeFileName(Formula);
        }

        /// <summary>
        /// Get FilePath current ID3Info
        /// </summary>
        public string FilePath
        {
            get
            { return ID3v2Info.FilePath; }
        }

        /// <summary>
        /// Get FileName of current ID3Info file
        /// </summary>
        public string FileName
        {
            get
            { return ID3v1Info.FileName; }
        }

        /// <summary>
        /// Indicate if current ID3Info is ezual to specific one
        /// </summary>
        /// <param name="obj">Object to check equality</param>
        /// <returns>True if equal otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            if (this.FilePath == ((ID3Info)obj).FilePath)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get hash code for current ID3Info
        /// </summary>
        /// <returns>int contains hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Indicate if current ID3Info is template
        /// </summary>
        public bool IsTemplate
        {
            get
            { return _ID3v1.IsTemplate; }
            set
            {
                _ID3v1.IsTemplate = value;
                _ID3v2.IsTemplate = value;
            }
        }
    }
}
