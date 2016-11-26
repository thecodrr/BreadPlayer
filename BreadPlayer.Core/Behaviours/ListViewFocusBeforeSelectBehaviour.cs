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
              DependencyProperty.RegisterAttached("FocusBeforeSelect", typeof(bool), typeof(ListViewService),
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
            var listView = d as ListView;
            if (listView != null)
            {
                listView.SelectionChanged += OnSelectionChanged;
            }
        }
        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;

            if (listView != null)
            {
                //listView.ScrollIntoView(listView.SelectedItem);
                listView.Focus(FocusState.Programmatic);
            }
        }        
    }
}

