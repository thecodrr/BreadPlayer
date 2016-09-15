using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
namespace Macalifa.Common
{
    public static class FrameworkElementExtensions
    {
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var MyQueue = new Queue<DependencyObject>();
            MyQueue.Enqueue(root);
            while (MyQueue.Count > 0)
            {
                DependencyObject current = MyQueue.Dequeue();
                for (int i = 0; i < Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    MyQueue.Enqueue(child);
                }
            }
            return null;
        }
    }
}
