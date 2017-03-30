using SplitViewMenu;
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

namespace BreadPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingView : Page
    {
        public NowPlayingView()
        {
            this.InitializeComponent();
            List<SimpleNavMenuItem> Items = new List<SimpleNavMenuItem>()
            {
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE8B1",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE8EE",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE8E5",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE995",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE892",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE102",
                },
                new SimpleNavMenuItem()
                {
                    FontGlyph = "\uE893",
                },
                
            };
            playcontrolsListView.ItemsSource = Items;
            playcontrolsListView.DataContext = Items;
        }
    }
}
