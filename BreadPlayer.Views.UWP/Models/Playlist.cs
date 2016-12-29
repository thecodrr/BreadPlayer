﻿/* 
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

namespace BreadPlayer.Models
{
   public class Playlist : ObservableObject
    {
        private LiteDB.ObjectId id;
        public LiteDB.ObjectId _id { get { return id; } set { Set(ref id, value); } }

        string name;
        public string Name { get { return name; }set { Set(ref name, value); } }
        string description;
        public string Description { get { return description; } set { Set(ref description, value); } }
      
        public Playlist()
        {

        }
    }
}
