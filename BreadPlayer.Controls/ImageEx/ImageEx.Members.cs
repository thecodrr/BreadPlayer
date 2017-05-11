using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.Media.Casting;

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
        #endregion

        #region NineGrid
        public Thickness NineGrid
        {
            get => (Thickness)GetValue(NineGridProperty);
            set => SetValue(NineGridProperty, value);
        }

        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(ImageEx), new PropertyMetadata(null));
        #endregion        

        public ProgressRing Progress => Content as ProgressRing;

        public Image Image => Content as Image;

        public CastingSource GetAsCastingSource()
        {
            if (Image != null)
            {
                return Image.GetAsCastingSource();
            }
            return null;
        }
    }
}
