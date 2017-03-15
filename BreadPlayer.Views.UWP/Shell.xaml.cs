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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Core;
using BreadPlayer.Services;
using BreadPlayer.Common;
using Windows.Media;
using BreadPlayer.ViewModels;
using Windows.Storage;
using BreadPlayer.Models;
using BreadPlayer.Extensions;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using SamplesCommon;
using Windows.UI.Xaml.Hosting;
using Windows.UI;
using BreadPlayer.Web.Lastfm;
using IF.Lastfm.Core.Api;
using Windows.ApplicationModel.Store;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        ShellViewModel ShellVM;
        List<Mediafile> OldFiles = new List<Mediafile>();
        public Shell()
        {
            this.InitializeComponent();
            SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(this).Compositor);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            CoreWindowLogic logic = new CoreWindowLogic();
            ShellVM = DataContext as ShellViewModel;
        }
           
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is StorageFile)
             Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, new List<object> { e.Parameter, 0.0, true, 50.0 });
            
            base.OnNavigatedTo(e);
        }
        bool isDragging = false;
        private void VolSliderThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
            ShellVM.DontUpdatePosition = true;
        }

        private void VolSliderThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UpdatePosition();
            isDragging = false;
        }
        bool isPressed;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thumb volSliderThumb = positionSlider.FindChildOfType<Thumb>();
            if (volSliderThumb != null)
            {
                volSliderThumb.DragCompleted += VolSliderThumb_DragCompleted;
                volSliderThumb.DragStarted += VolSliderThumb_DragStarted;
            }
            Window.Current.CoreWindow.PointerPressed += (ea, a) =>
            {
                if (positionSlider.GetBoundingRect().Contains(a.CurrentPoint.Position) && !isDragging)
                {
                    isPressed = true;
                    ShellVM.DontUpdatePosition = true;
                }
            };

            Window.Current.CoreWindow.PointerReleased += (ea, a) => 
            {
                if (isPressed && !isDragging) { UpdatePosition(true); isPressed = false; }
            };
        }
                
        async void UpdatePosition(bool wait = false)
        {
            if (ShellVM != null)
            {
                ShellVM.CurrentPosition = positionSlider.Value < positionSlider.Maximum ? positionSlider.Value : positionSlider.Value - 1;
            }
            if (wait) await Task.Delay(500);
            ShellVM.DontUpdatePosition = false;
        }
            
        public async void ShowMessage(string msg)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }        
    }
}