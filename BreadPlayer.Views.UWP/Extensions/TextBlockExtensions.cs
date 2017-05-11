using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace BreadPlayer.Extensions
{
    public static class TextBlockExtensions
    {
        public static string GetTextType(DependencyObject obj)
        {
            return (string)obj.GetValue(TextTypeProperty);
        }
        public static void SetTextType(DependencyObject obj, string value)
        {
            obj.SetValue(TextTypeProperty, value);
        }
        public static readonly DependencyProperty TextTypeProperty =
              DependencyProperty.Register("TextType",
                  typeof(string),
                  typeof(TextBlockExtensions),
                  new PropertyMetadata("Normal", (sender, e) => {

                      var textBlock = sender as TextBlock;
                      var value = (string)e.NewValue;
                      if (value == "All Capitals")
                      {
                          if (textBlock.Tag?.ToString() != "Heading")
                          {
                              textBlock.SetValue(TextBlock.FontSizeProperty, 18);
                          }

                          if (textBlock.Tag?.ToString() == "Numerical" && value == "All Capitals")
                          {
                              textBlock.SetValue(TextBlock.FontSizeProperty, 15);
                          }

                          textBlock.SetValue(Typography.CapitalsProperty, FontCapitals.AllSmallCaps);
                          textBlock.SetValue(TextBlock.FontWeightProperty, FontWeights.ExtraLight);
                      }
                      else if (value == "Normal")
                      {
                          textBlock.SetValue(Typography.CapitalsProperty, FontCapitals.Titling);

                          if (textBlock.Tag?.ToString() == "Heading")
                          {
                              textBlock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
                          }
                          else
                          {
                              textBlock.SetValue(TextBlock.FontWeightProperty, FontWeights.Normal);
                              textBlock.SetValue(TextBlock.FontSizeProperty, 15);
                          }
                      }

                  }));
    }
}
