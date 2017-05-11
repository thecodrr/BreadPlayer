/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using BreadPlayer.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using Windows.UI.Core;
using BreadPlayer.Core.Engines.Interfaces;

namespace BreadPlayer
{
   public class ViewModelBase : INotifyPropertyChanged
    {
        private NotificationManager.BreadNotificationManager notificationManager;
        public NotificationManager.BreadNotificationManager NotificationManager
        {
            get { if (notificationManager == null) notificationManager = SharedLogic.NotificationManager; return notificationManager; }
        }

        private IPlayerEngine player;
        public IPlayerEngine Player
        {
            get
            {
                if (player == null)
                    player = SharedLogic.Player;
                return player;
            }
        }

        private static SharedLogic logic;
        public static SharedLogic SharedLogic
        {
            get
            {
                if (logic == null)
                    logic = new SharedLogic();
                return logic;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await SharedLogic.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
