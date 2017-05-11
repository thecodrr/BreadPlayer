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

using System.Collections.Generic;
using BreadPlayer.Core.Common;

namespace BreadPlayer.Core.Models
{
    public class Playlist : ObservableObject, IDbRecord
    {
        private long _id;
        public long Id { get => _id;
            set => Set(ref _id, value);
        }
        private string _name;
        public string Name { get => _name;
            set => Set(ref _name, value);
        }
        private bool _isPrivate;
        public bool IsPrivate { get => _isPrivate;
            set => Set(ref _isPrivate, value);
        }
        private string _description;
        public string Description { get => _description;
            set => Set(ref _description, value);
        }
        private string _hash;
        public string Hash { get => _hash;
            set => Set(ref _hash, value);
        }
        private string _salt;
        public string Salt { get => _salt;
            set => Set(ref _salt, value);
        }
        public List<long> SongsIds { get; set; } = new List<long>();

        public string GetTextSearchKey()
        {
            return Name;
        }
    }
}
