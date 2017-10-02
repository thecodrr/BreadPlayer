using BreadPlayer.Extensions;
using BreadPlayer.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BreadPlayer.Helpers
{
    public static class SliderHelper
    {
        private static bool _isDragging;

        public static bool IsDragging(this Slider slider)
        {
            return _isDragging;
        }

        public static void InitEvents(this Slider slider, Action actionOnComplete, Action actionOnStart)
        {
            Thumb volSliderThumb = slider.FindChildOfType<Thumb>();
            if (volSliderThumb != null)
            {
                volSliderThumb.DragCompleted += (sender, e) =>
                {
                    actionOnComplete.Invoke();
                    _isDragging = false;
                };
                volSliderThumb.DragStarted += (sender, e) =>
                {
                    actionOnStart.Invoke();
                    _isDragging = true;
                };
            }
        }

        public static async void UpdatePosition(this Slider slider, ShellViewModel shellVm, bool wait = false, bool progressBar = false)
        {
            if (shellVm != null)
            {
                if (!progressBar)
                {
                    shellVm.CurrentPosition = slider.Value < slider.Maximum ? slider.Value : slider.Value - 1;
                }
            }
            if (wait)
            {
                await Task.Delay(500);
            }

            shellVm.DontUpdatePosition = false;
        }
    }
}