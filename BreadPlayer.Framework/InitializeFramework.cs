using BreadPlayer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class InitializeFramework
    {
        static IDispatcher dispatcher;
        public static IDispatcher Dispatcher
        {
            get { return dispatcher; }
            set { dispatcher = value; }
        }
        public InitializeFramework(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
    }

