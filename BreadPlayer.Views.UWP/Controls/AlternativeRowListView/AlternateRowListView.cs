using BreadPlayer.Core.Models;
using BreadPlayer.Extensions;
using BreadPlayer.Interfaces;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Controls
{
    public class AlternatingRowListView : ListView
    {
        public static readonly DependencyProperty OddRowBackgroundProperty =
            DependencyProperty.Register("OddRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);

        public static readonly DependencyProperty EvenRowBackgroundProperty =
            DependencyProperty.Register("EvenRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);

        public Brush OddRowBackground

        {
            get => (Brush)GetValue(OddRowBackgroundProperty);

            set => SetValue(OddRowBackgroundProperty, value);
        }

        public Brush EvenRowBackground

        {
            get => (Brush)GetValue(EvenRowBackgroundProperty);

            set => SetValue(EvenRowBackgroundProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            try
            {
                if (element is ListViewItem listViewItem)
                {
                    var index = IndexFromContainer(element);

                    var isOdd = (index + 1) % 2 == 1;

                    //support for adjusting to groups(each group should be treated individually)

                    var collectionViewSource = Tag as CollectionViewSource;
                    if (collectionViewSource?.Source is GroupedObservableCollection<IGroupKey, Mediafile> groups)
                    {
                        var o = Items?[index];
                        if (o != null)
                        {
                            var currentGroup = groups.FirstOrDefault(p => p.Contains(o));
                            index = currentGroup.IndexOf(o as Mediafile);
                            isOdd = (index + 1) % 2 == 1;
                        }
                    }

                    listViewItem.Background = isOdd
                        ? OddRowBackground
                        : EvenRowBackground;
                }
            }
            catch (Exception ex)
            {
                BLogger.E("Error while preparing alternative listview.", ex);
            }
        }
    }
}