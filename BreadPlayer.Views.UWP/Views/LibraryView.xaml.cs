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

using BreadPlayer.Core.Models;
using BreadPlayer.ViewModels;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Extensions;
using Windows.UI.Xaml.Media;
using BreadPlayer.Helpers;
using System;
using Windows.UI.Xaml.Data;
using BreadPlayer.Interfaces;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryView
    {
        public LibraryView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            this.Loaded += LibraryView_Loaded;
        }

        private void LibraryView_Loaded(object sender, RoutedEventArgs e)
        {
            fileBox.GetScrollViewer().ViewChanged += LibraryView_ViewChanged;
        }

        private void LibraryView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if ((sender as ScrollViewer).VerticalOffset <= 0)
            {
                scrollHeaderPanel.Background = null;
            }
            else
            {
                if (scrollHeaderPanel.Background == null)
                    scrollHeaderPanel.Background = App.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
            }
        }
        
        private LibraryViewModel LibVM => App.Current.Resources["LibVM"] as LibraryViewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LibVM.MusicLibraryLoaded += async (s, a) =>
            {
                var pVm = App.Current.Resources["PlaylistsCollectionVM"];
            };
        }
        private void fileBox_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Add file/folder(s) to library";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }

        private void semanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                (this.FindName("backBtn") as Button).Visibility = Visibility.Visible;
                (this.FindName("alphabetList") as GridView).Visibility = Visibility.Visible;
                return;
            }
            try
            {
                // get the selected group
                var selectedGroup = e.SourceItem.Item as string;
                Grouping<IGroupKey, Mediafile> myGroup = (DataContext as LibraryViewModel).TracksCollection.FirstOrDefault(g => g.Key.Key.StartsWith(selectedGroup));
                backBtn.Visibility = Visibility.Collapsed;
                e.DestinationItem = new SemanticZoomLocation
                {
                    Bounds = new Rect(0, 0, 1, 1),
                    Item = myGroup
                };
            }
            catch { }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ISelectable record in e.RemovedItems)
            {
                record.IsSelected = false;
            }
            foreach (ISelectable record in e.AddedItems)
            {
                record.IsSelected = true;
            }
        }

        private void OnArtistClicked(object sender, ItemClickEventArgs e)
        {
            BreadPlayer.Core.SharedLogic.Instance.NavigateToArtistPageCommand.Execute(e.ClickedItem);
        }
    }
}