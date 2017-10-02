using System.IO;
using Windows.Storage;

namespace BreadPlayer.Core
{
    public class SimpleFileAbstraction : TagLib.File.IFileAbstraction
    {
        private StorageFile _file;

        public SimpleFileAbstraction(StorageFile file)
        {
            _file = file;
        }

        public string Name => _file.Name;

        public Stream ReadStream => _file?.OpenStreamForReadAsync().Result;

        public Stream WriteStream => _file?.OpenStreamForWriteAsync().Result;

        public void CloseStream(Stream stream)
        {
            stream.Position = 0;
        }
    }
}