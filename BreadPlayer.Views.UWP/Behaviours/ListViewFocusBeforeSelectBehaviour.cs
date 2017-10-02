using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Behaviours
{
    public class ListViewService
    {
        // private static SelectionChangedEventHandler selectionChangedEventHandler;
        public static readonly DependencyProperty FocusBeforeSelectProperty;

        static ListViewService()
        {
            //selectionChangedEventHandler = new SelectionChangedEventHandler(OnSelectionChanged);
            FocusBeforeSelectProperty =
              DependencyProperty.RegisterAttached(
                  "FocusBeforeSelect", typeof(bool), typeof(ListViewService),
                   new PropertyMetadata(false, OnPropertyChanged));
        }

        public static bool GetFocusBeforeSelect(ListView listView)
        {
            return (bool)listView.GetValue(FocusBeforeSelectProperty);
        }

        public static void SetFocusBeforeSelect(ListView listView, bool value)
        {
            listView.SetValue(FocusBeforeSelectProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.SelectionChanged += OnSelectionChanged;
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                //listView.ScrollIntoView(listView.SelectedItem);
                listView.Focus(FocusState.Programmatic);
            }
        }
    }
}