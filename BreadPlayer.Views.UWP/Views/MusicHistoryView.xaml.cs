using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace BreadPlayer
{
    public partial class MusicHistoryView : Page
    {
        MusicHistoryViewModel MusicHistoryVM;
        public MusicHistoryView()
        {
            InitializeComponent();
            MusicHistoryVM = new MusicHistoryViewModel();
            this.DataContext = MusicHistoryVM;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Any())
                (e.RemovedItems[0] as PivotItem).Content = null;
            (mainPivot.SelectedItem as PivotItem).Content = recentlyPlayedList;

            switch (mainPivot.SelectedIndex)
            {
                case 0:
                    MusicHistoryVM.GetRecentlyPlayedSongs();
                    break;
                case 1:
                    MusicHistoryVM.GetRecentlyAddedSongs();
                    break;
                case 2:
                    MusicHistoryVM.GetMostPlayedSongs();
                    break;
            }
        }
    }
}
