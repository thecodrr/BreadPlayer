using BreadPlayer.Models;

namespace BreadPlayer.Core.Events
{
    public class MediaAboutToEndEventArgs
    {
        private Mediafile mediaFile;
        public MediaAboutToEndEventArgs(Mediafile mediafile)
        {
            this.mediaFile = mediafile;
        } // eo ctor

        public Mediafile MediaFile { get { return mediaFile; } }
    }
}
