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
using BreadPlayer.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using BreadPlayer.Models;
using System.Text.RegularExpressions;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryView
    {
        LibraryViewModel LibVM = Core.CoreMethods.LibVM;
        public LibraryView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private async void fileBox_Drop(object sender, DragEventArgs e)
        {
            Core.CoreMethods Methods = new Core.CoreMethods();
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var files = await e.DataView.GetStorageItemsAsync();
                if (files.Any())
                {
                    foreach(var file in files)
                    {
                        Mediafile mp3file = null;
                        string path = file.Path;
                        var tempList = new List<Mediafile>();
                        if (LibVM.TracksCollection.Elements.All(t => t.Path != path))
                        {
                            try
                            {

                                mp3file = await Core.CoreMethods.CreateMediafile(file as StorageFile);

                            }
                            catch { }
                            tempList.Add(mp3file);
                          
                                LibVM.TracksCollection.AddRange(tempList);
                                LibVM.db.Insert(tempList);
                                tempList.Clear();
                           

                        }
                    }
                }
            }
        }
        private void fileBox_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Add files to library";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }

        private void semanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            // only interested in zoomed out->zoomed in transitions
            if (e.IsSourceZoomedInView)
            {
                return;
            }
            try
            {  // get the selected group
                var selectedGroup = e.SourceItem.Item as string;
                Grouping<string, Mediafile> myGroup = null;
                myGroup = LibVM.TracksCollection.FirstOrDefault(g => g.Key.StartsWith(selectedGroup));

                SemanticZoomLocation zoomloc = new SemanticZoomLocation();
                zoomloc.Bounds = new Windows.Foundation.Rect(0, 0, 1, 1);
                zoomloc.Item = myGroup;
                e.DestinationItem = zoomloc;
            }
            catch { }
        }
    }
}
