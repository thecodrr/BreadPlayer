using BreadPlayer.Core.Models;
using BreadPlayer.Interfaces;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Controls
{
    public class GroupHeaderDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommonDataTemplate { get; set; }
        public DataTemplate ArtistDataTemplate { get; set; }
        public DataTemplate AlbumDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var group = item as IGrouping<IGroupKey, Mediafile>;
            if (group.Key is AlbumGroupKey)
                return AlbumDataTemplate;
            else if (group.Key is ArtistGroupKey)
                return ArtistDataTemplate;
            else
                return CommonDataTemplate;
        }
    }
}