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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using BreadPlayer.Core;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.NotificationManager;

namespace BreadPlayer
{
   public class ViewModelBase : INotifyPropertyChanged
    {
        private BreadNotificationManager _notificationManager;
        public BreadNotificationManager NotificationManager
        {
            get { if (_notificationManager == null) { _notificationManager = SharedLogic.NotificationManager; } return _notificationManager; }
        }

        private IPlayerEngine _player;
        public IPlayerEngine Player
        {
            get
            {
                if (_player == null)
                {
                    _player = SharedLogic.Player;
                }

                return _player;
            }
        }

        private static SharedLogic _logic;
        public static SharedLogic SharedLogic
        {
            get
            {
                if (_logic == null)
                {
                    _logic = new SharedLogic();
                }

                return _logic;
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
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
