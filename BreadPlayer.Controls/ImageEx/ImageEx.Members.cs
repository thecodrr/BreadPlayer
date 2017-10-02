using Windows.Media.Casting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BreadPlayer.Controls
{
    partial class ImageEx
    {
        #region Stretch

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform));

        #endregion Stretch

        #region NineGrid

        public Thickness NineGrid
        {
            get => (Thickness)GetValue(NineGridProperty);
            set => SetValue(NineGridProperty, value);
        }

        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(ImageEx), new PropertyMetadata(null));
        #endregion NineGrid



        public ProgressRing Progress => Content as ProgressRing;

        public Image Image => Content as Image;

        public CastingSource GetAsCastingSource() => Image?.GetAsCastingSource();
    }
}