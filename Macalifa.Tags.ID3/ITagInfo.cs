using System;
using System.Collections.Generic;
using System.Text;

namespace Macalifa.Tags.ID3
{
    /// <summary>
    /// Provide an interface for Tag streams
    /// </summary>
    public interface ITagInfo
    {
        /// <summary>
        /// Gets file path for reading and writing data
        /// </summary>
        string FilePath
        { get;        }

        /// <summary>
        /// Gets file name part of file path
        /// </summary>
        string FileName
        { get;}

        /// <summary>
        /// Saves Tag information to file
        /// </summary>
        /// <returns>True if save successfully otherwise false</returns>
        bool Save();

        /// <summary>
        /// Load information from file
        /// </summary>
        /// <returns>true if loads successfully otherwise false</returns>
        bool Load();

        /// <summary>
        /// Indicate if file contained exception while loading
        /// </summary>
        bool HaveException
        { get;}

        /// <summary>
        /// Save file with specific formula
        /// </summary>
        /// <param name="Formula">Formula to save file</param>
        bool Save(string Formula);

        /// <summary>
        /// Save file at specific location
        /// </summary>
        bool SaveAs(string path);

        /// <summary>
        /// Indicate if current ITagInfo is template
        /// </summary>
        bool IsTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Make filename with specific formula
        /// </summary>
        /// <param name="Formula">Formula for rename</param>
        /// <returns>System.String Contains filename</returns>
        string MakeFileName(string Formula);
    }

    /// <summary>
    /// Indicates diffrent type of tags list
    /// </summary>
    public enum TagListTypes
    {
        /// <summary>
        /// List contains ID3
        /// </summary>
        ID3,
        /// <summary>
        /// List Contains ASF
        /// </summary>
        ASF,
        /// <summary>
        /// List Contains both ID3 and ASF
        /// </summary>
        Both
    }

    /// <summary>
    /// Indicates diffrent types of tags system
    /// </summary>
    public enum TagTypes
    {
        /// <summary>
        /// ID3 tag
        /// </summary>
        ID3,
        /// <summary>
        /// WMA tag
        /// </summary>
        ASF
    }
}
