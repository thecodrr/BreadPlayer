using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Interfaces
{
    public interface IFileSystem
    {
        bool Open(string name, ref uint fileSize, ref IntPtr handle);
        bool Close();
        bool Seek(uint pos);
        bool Read(IntPtr buffer, uint sizeBytes, ref uint bytesRead);
    }
}
