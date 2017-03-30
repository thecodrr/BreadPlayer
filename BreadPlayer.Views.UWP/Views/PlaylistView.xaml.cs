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
using BreadPlayer.Models;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using BreadPlayer.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using BreadPlayer.Extensions;
using Windows.UI.Xaml.Media.Animation;
using BreadPlayer.Core;

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistView
    {
        double MaxFontSize;
        double MinFontSize;
        PlaylistViewModel PlaylistVM;
        public PlaylistView()
        {
            this.InitializeComponent();
            var deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            MaxFontSize = deviceFamily.Contains("Mobile") ? 44 : 60;
            MinFontSize = deviceFamily.Contains("Mobile") ? 34 : 50;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlaylistVM = new PlaylistViewModel(e.Parameter);
            this.DataContext = PlaylistVM;
            base.OnNavigatedTo(e);
        }
        private void fileBox_Loaded(object sender, RoutedEventArgs e)
        {
            fileBox.FindChildOfType<ScrollViewer>().ViewChanging += PlaylistView_ViewChanging; ;
        }
     
        private void PlaylistView_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.NextView.VerticalOffset < 15)// > (sender as ScrollViewer).VerticalOffset)
            {
                if(art.Height == 254)
                {
                   art.ZoomAnimate(254, 354, "Height");
                }
                if (headerText.FontSize < MaxFontSize)
                {
                    headerText.FontSize = headerText.FontSize + 2;
                    headerDesc.FontSize++;
                }
            }
            else
            {
                if (art.Height == 354)
                {
                    art.ZoomAnimate(354, 254, "Height");
                }
                if (headerText.FontSize > MinFontSize)
                {
                    headerText.FontSize = headerText.FontSize - 2;
                    headerDesc.FontSize--;
                }
            }
        }
    }
}
