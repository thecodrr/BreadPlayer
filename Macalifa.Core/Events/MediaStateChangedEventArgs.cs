using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macalifa.Core;
namespace Macalifa.Events
{
   
    public class MediaStateChangedEventArgs : EventArgs
    {
        private Macalifa.Core.PlayerState newState;
        public MediaStateChangedEventArgs(PlayerState NewState)
        {
            newState = NewState;
        } // eo ctor

        public PlayerState NewState { get { return newState; } }
    }
}
