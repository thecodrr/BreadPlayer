using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Events
{
    public class MediaAboutToEndEventArgs
    {
        private Mediafile mediaFile;
        public MediaAboutToEndEventArgs(Mediafile mediafile)
        {
            mediaFile = mediafile;
        } // eo ctor

        public Mediafile MediaFile { get { return mediaFile; } }
    }
}
