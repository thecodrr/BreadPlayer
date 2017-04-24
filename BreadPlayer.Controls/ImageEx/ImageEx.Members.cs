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
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform));
        #endregion

        #region NineGrid
        public Thickness NineGrid
        {
            get { return (Thickness)GetValue(NineGridProperty); }
            set { SetValue(NineGridProperty, value); }
        }

        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register("NineGrid", typeof(Thickness), typeof(ImageEx), new PropertyMetadata(null));
        #endregion        

        public ProgressRing Progress
        {
            get { return this.Content as ProgressRing; }
        }

        public Image Image
        {
            get { return this.Content as Image; }
        }        

        public CastingSource GetAsCastingSource()
        {
            if (this.Image != null)
            {
                return this.Image.GetAsCastingSource();
            }
            return null;
        }
    }
}
