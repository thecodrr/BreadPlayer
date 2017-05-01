using BreadPlayer.Extensions;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BreadPlayer.Helpers
{
    public static class SliderHelper
    {
        private static bool isDragging;
        public static bool IsDragging(this Slider slider)
        {
            return isDragging;
        }
        public static void InitEvents(this Slider slider, Action actionOnComplete, Action actionOnStart)
        {
            Thumb volSliderThumb = slider.FindChildOfType<Thumb>();
            if (volSliderThumb != null)
            {
                volSliderThumb.DragCompleted += (sender, e)=> 
                {
                    actionOnComplete.Invoke();
                    isDragging = false;
                };
                volSliderThumb.DragStarted += (sender, e) => 
                {
                    actionOnStart.Invoke();
                    isDragging = true;
                };
            }
        }
        public static async void UpdatePosition(this Slider slider, ProgressBar positionProgressBar, ShellViewModel ShellVM, bool wait = false, bool progressBar = false)
        {
            if (ShellVM != null)
            {
                if (!progressBar)
                    ShellVM.CurrentPosition = slider.Value < slider.Maximum ? slider.Value : slider.Value - 1;
                else
                    ShellVM.CurrentPosition = positionProgressBar.Value < positionProgressBar.Maximum ? positionProgressBar.Value : positionProgressBar.Value - 1;
            }
            if (wait) await Task.Delay(500);
            ShellVM.DontUpdatePosition = false;
        }
    }
}
