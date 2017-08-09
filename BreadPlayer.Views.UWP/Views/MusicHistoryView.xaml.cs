using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BreadPlayer
{
    public partial class MusicHistoryView : Page
    {
        MusicHistoryViewModel MusicHistoryVM;
        public MusicHistoryView()
        {
            InitializeComponent();
            MusicHistoryVM = new MusicHistoryViewModel();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (mainPivot.SelectedIndex)
            {
                case 0:
                    RemoveRecentlyPlayedList();
                    recentlyPlayedPivotItem.Content = recentlyPlayedList;
                    (this.Resources["cvs"] as CollectionViewSource).Source = MusicHistoryVM.GetRecentlyPlayedSongsAsync();
                    break;
                case 1:
                    RemoveRecentlyPlayedList();
                    recentlyAddedPivotItem.Content = recentlyPlayedList;
                    (this.Resources["cvs"] as CollectionViewSource).Source = MusicHistoryVM.GetRecentlyAddedSongsAsync();
                    break;
                case 2:
                    RemoveRecentlyPlayedList();
                    mostPlayedPivotItem.Content = recentlyPlayedList;
                    (this.Resources["cvs"] as CollectionViewSource).Source = MusicHistoryVM.GetMostPlayedSongsAsync();
                    break;
            }
        }
        private void RemoveRecentlyPlayedList()
        {
            recentlyAddedPivotItem.Content = null;
            recentlyPlayedPivotItem.Content = null;
            mostPlayedPivotItem.Content = null;
        }
    }
}
