using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalifa.Models
{
   public class Playlist : ViewModelBase
    {
        public string name;
        public string Name { get { return name; }set { Set(ref name, value); } }

        public Playlist()
        {

        }
    }
}
