using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using ManagedBass;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace MusicTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MusicTest.MainPageViewModel();         
        }

        //private void MyThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        //{
        //    //player.Effect.GainDB = (float)volumeSlider.Value / 10;
        //    player.Position = (long)volumeSlider.Value;
        //}

        //async void OnStart(object sender, RoutedEventArgs e)
        //{
        //    FileOpenPicker openPicker = new FileOpenPicker();
        //    openPicker.ViewMode = PickerViewMode.Thumbnail;
        //    openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
        //    openPicker.FileTypeFilter.Add(".mp3");
        //    StorageFile file = await openPicker.PickSingleFileAsync();
        //    await player.Load(file.Path);
        //    await player.Play();
        //    Thumb MyThumb = MyFindSilderChildOfType<Thumb>(volumeSlider);
        //    MyThumb.DragCompleted += MyThumb_DragCompleted;
        //    volumeSlider.Maximum = player.Length;
        //    ss.Text = player.Length.ToString();
        //}

        //private void volumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        //{
        //}

        //private void AppBarButton_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //public static T MyFindSilderChildOfType<T>(DependencyObject root) where T : class
        //{
        //    var MyQueue = new Queue<DependencyObject>();
        //    MyQueue.Enqueue(root);
        //    while (MyQueue.Count > 0)
        //    {
        //        DependencyObject current = MyQueue.Dequeue();
        //        for (int i = 0; i < Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(current); i++)
        //        {
        //            var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(current, i);
        //            var typedChild = child as T;
        //            if (typedChild != null)
        //            {
        //                return typedChild;
        //            }
        //            MyQueue.Enqueue(child);
        //        }
        //    }
        //    return null;
        //}
    }
}