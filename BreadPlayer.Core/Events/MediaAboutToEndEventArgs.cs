using BreadPlayer.Core.Models;

namespace BreadPlayer.Core.Events
{
    public class MediaAboutToEndEventArgs
    {
        private Mediafile _mediaFile;

        public MediaAboutToEndEventArgs(Mediafile mediafile)
        {
            _mediaFile = mediafile;
        } // eo ctor

        public Mediafile MediaFile => _mediaFile;
    }
}