using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Macalifa
{
  public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}