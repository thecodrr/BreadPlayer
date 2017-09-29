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

namespace BreadPlayer.Services
{
    public class GenericService<T> where T : new()
    {
        private static GenericService<T> _instance;

        public static GenericService<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GenericService<T>();
                }

                return _instance;
            }
        }

        public T GenericClass { get; private set; }

        public GenericService()
        {
            // Create the generic class instance
            GenericClass = new T();
        }
    }
}