using BreadPlayer.ViewModels;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer
{
    public partial class MusicHistoryView : Page
    {
        private MusicHistoryViewModel MusicHistoryVM;

        public MusicHistoryView()
        {
            InitializeComponent();
            MusicHistoryVM = new MusicHistoryViewModel();
            this.DataContext = MusicHistoryVM;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Any())
                (e.RemovedItems[0] as PivotItem).Content = null;
            (mainPivot.SelectedItem as PivotItem).Content = recentlyPlayedList;
            MusicHistoryVM.CurrentCollection = null;
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