/* 
	Macalifa. A music player made for Windows 10 store.
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
using Macalifa.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalifa.Services
{
    class ShellViewService
    {
        static ShellViewService instance;

        public static ShellViewService Instance
        {
            get
            {
                if (instance == null)
                    instance = new ShellViewService();

                return instance;
            }
        }

        public Macalifa.ViewModels.ShellViewModel ShellVM { get; private set; }

        public ShellViewService()
        {
            // Create the player instance
            ShellVM = new Macalifa.ViewModels.ShellViewModel();
        }
    }

    class LibraryViewService
    {
        static LibraryViewService instance;
        public static LibraryViewModel vm;
        public static LibraryViewService Instance
        {
            get
            {
                if (instance == null)
                    instance = new LibraryViewService(vm);

                return instance;
            }
        }

        public Macalifa.ViewModels.LibraryViewModel LibVM { get; private set; }

        public LibraryViewService(LibraryViewModel vm)
        {
            // Create the player instance
            LibVM = vm;
        }
    }
}
