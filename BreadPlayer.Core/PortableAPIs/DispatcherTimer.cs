using BreadPlayer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BreadPlayer.Core
{
    public class DispatcherTimer
    {
        IDispatcher target_dispatcher;
        long interval;
        EventHandler callback;
        Timer timer;
        object tag;

        public DispatcherTimer(IDispatcher dispatcher)
        {
            target_dispatcher = dispatcher;
        }


        public void Start()
        {
            if (timer == null)
            {
                long repeat_interval = interval;
                if (repeat_interval == 0)
                    repeat_interval = 1;
                timer = new Timer(new TimerCallback(timer_tick),
                            null, new TimeSpan(interval),
                            new TimeSpan(repeat_interval));
            }
        }

        void timer_tick(object state)
        {
            target_dispatcher.RunAsync(() => { 
                Tick?.Invoke(this, EventArgs.Empty);
                callback?.Invoke(this, EventArgs.Empty);
            });
        }

        public void Stop()
        {
            if (timer == null)
                return;

            timer.Dispose();
            timer = null;
        }

        public IDispatcher Dispatcher
        {
            get
            {
                return target_dispatcher;
            }
        }

        public TimeSpan Interval
        {
            get
            {
                return new TimeSpan(interval);
            }

            set
            {
                if (interval == value.Ticks)
                    return;

                interval = value.Ticks;

                if (timer != null)
                    timer.Change(new TimeSpan(interval),
                            new TimeSpan(interval));
            }
        }

        public bool IsEnabled
        {
            get
            {
                return timer != null;
            }

            set
            {
                if (value && timer == null)
                    Start();
                if (value == false && timer != null)
                    Stop();
            }
        }

        public object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
            }
        }
        public event EventHandler Tick;
    }
}
