using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalifa.Services
{
    class MacalifaPlayerService
    {
        static MacalifaPlayerService instance;

        public static MacalifaPlayerService Instance
        {
            get
            {
                if (instance == null)
                    instance = new MacalifaPlayerService();

                return instance;
            }
        }

        public Macalifa.Core.MacalifaPlayer Player { get; private set; }

        public MacalifaPlayerService()
        {
            // Create the player instance
            Player = new Macalifa.Core.MacalifaPlayer();
        }
    }
}
