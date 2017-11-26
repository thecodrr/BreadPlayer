using BreadPlayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Runtime.InteropServices;

namespace BreadPlayer.Helpers
{
    public class FmodFileSystem : IFileSystem
    {
        Stream fileStream;
        public bool Close()
        {
            fileStream.Dispose();
            return true;
        }

        public bool Open(string name, ref uint fileSize, ref IntPtr handle)
        {
            var storageFile = StorageFile.GetFileFromPathAsync(name).AsTask().Result;
            if (storageFile == null)
                return false;
            fileStream = storageFile.OpenAsync(FileAccessMode.Read).AsTask().Result.AsStreamForRead();
            if (fileStream == null || !fileStream.CanRead)
                return false;
            fileSize = (uint)fileStream.Length;
            handle = new IntPtr(fileStream.GetHashCode());
            return true;
        }
        public bool Read(IntPtr buffer, uint sizeBytes, ref uint bytesRead)
        {
            if (fileStream?.CanRead == false)
                return false;
            unsafe
            {
                // simply cast the given IntPtr to a native pointer to byte values
                byte* data = (byte*)buffer;
                // read the file into the data pointer directly
                int bytesread = (int)sizeBytes;
                for (int a = 0; a < sizeBytes; a++)
                {
                    int val = fileStream.ReadByte();
                    if (val != -1)
                    {
                        data[a] = (byte)val;   // set the value
                    }
                    else
                    {
                        bytesread = a;
                        break;
                    }
                }
                // end of the file/stream?
                if (bytesread < sizeBytes)
                {
                    fileStream.Dispose();
                    return false; // set indicator flag                 
                }
            }
            return true;
        }

        public bool Seek(uint pos)
        {
            try
            {
                fileStream.Seek(pos, SeekOrigin.Begin);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
