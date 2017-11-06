using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public class DiskItem : ObservableObject
    {
        public string Size { get; set; }
        public string Icon { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public object Cache { get; set; }
        public bool IsFile { get; set; }
        public DiskItemLocationType DiskItemLocation { get; set; }
        bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set => Set(ref isPlaying, value);
        }
    }
}
