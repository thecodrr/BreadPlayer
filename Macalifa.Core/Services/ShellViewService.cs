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
