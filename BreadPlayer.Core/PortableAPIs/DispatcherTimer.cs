using BreadPlayer.Interfaces;
using System;
using System.Threading;

namespace BreadPlayer.Core.PortableAPIs
{
    public class DispatcherTimer
    {
        private IDispatcher _targetDispatcher;
        private long _interval;
#pragma warning disable CS0649 // Field 'DispatcherTimer._callback' is never assigned to, and will always have its default value null
        private EventHandler _callback;
#pragma warning restore CS0649 // Field 'DispatcherTimer._callback' is never assigned to, and will always have its default value null
        private Timer _timer;
        private object _tag;

        public DispatcherTimer(IDispatcher dispatcher)
        {
            _targetDispatcher = dispatcher;
        }

        public void Start()
        {
            if (_timer == null)
            {
                long repeatInterval = _interval;
                if (repeatInterval == 0)
                {
                    repeatInterval = 1;
                }

                _timer = new Timer(timer_tick,
                            null, new TimeSpan(_interval),
                            new TimeSpan(repeatInterval));
            }
        }

        private void timer_tick(object state)
        {
            _targetDispatcher.RunAsync(() =>
            {
                Tick?.Invoke(this, EventArgs.Empty);
                _callback?.Invoke(this, EventArgs.Empty);
            });
        }

        public void Stop()
        {
            if (_timer == null)
            {
                return;
            }

            _timer.Dispose();
            _timer = null;
        }

        public IDispatcher Dispatcher => _targetDispatcher;

        public TimeSpan Interval
        {
            get => new TimeSpan(_interval);

            set
            {
                if (_interval == value.Ticks)
                {
                    return;
                }

                _interval = value.Ticks;

                if (_timer != null)
                {
                    _timer.Change(new TimeSpan(_interval),
                            new TimeSpan(_interval));
                }
            }
        }

        public bool IsEnabled
        {
            get => _timer != null;

            set
            {
                if (value && _timer == null)
                {
                    Start();
                }

                if (value == false && _timer != null)
                {
                    Stop();
                }
            }
        }

        public object Tag
        {
            get => _tag;

            set => _tag = value;
        }

        public event EventHandler Tick;
    }
}