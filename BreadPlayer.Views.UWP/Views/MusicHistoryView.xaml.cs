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

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Any())
                (e.RemovedItems[0] as PivotItem).Content = null;
            (mainPivot.SelectedItem as PivotItem).Content = recentlyPlayedList;
            MusicHistoryVM.CurrentCollection = null;
            switch (mainPivot.SelectedIndex)
            {
                case 0:
                    await MusicHistoryVM.GetRecentlyPlayedSongs().ConfigureAwait(false);
                    break;
                case 1:
                    await MusicHistoryVM.GetRecentlyAddedSongs().ConfigureAwait(false);
                    break;
                case 2:
                    await MusicHistoryVM.GetMostPlayedSongs().ConfigureAwait(false);
                    break;
            }
        }
    }
}