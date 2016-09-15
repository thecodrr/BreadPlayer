using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macalifa.Core;
namespace Macalifa.Events
{
   public class MediaEndedEventArgs : EventArgs
    {
        private PlayerState state;
        public MediaEndedEventArgs(PlayerState PlayerState)
        {
            state = PlayerState;
        }

        public PlayerState PlayerState { get { return state; } }
    }
}
