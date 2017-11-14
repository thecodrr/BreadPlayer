using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ObservableObject : INotifyPropertyChanged
{
    /// <summary>Occurs when a property value changes. </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
    /// <param name="propertyName">The property name as lambda. </param>
    /// <param name="oldValue">A reference to the backing field of the property. </param>
    /// <param name="newValue">The new value. </param>
    /// <returns>True if the property has changed. </returns>
    protected bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] String propertyName = null)
    {
        return Set(propertyName, ref oldValue, newValue);
    }

    /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
    /// <param name="propertyName">The property name as lambda. </param>
    /// <param name="oldValue">A reference to the backing field of the property. </param>
    /// <param name="newValue">The new value. </param>
    /// <returns>True if the property has changed. </returns>
    protected virtual bool Set<T>(String propertyName, ref T oldValue, T newValue)
    {
        if (Equals(oldValue, newValue))
        {
            return false;
        }

        oldValue = newValue;
        RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        return true;
    }

    /// <summary>Raises the property changed event. </summary>
    /// <param name="propertyName">The property name. </param>
    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Raises the property changed event. </summary>
    /// <param name="args">The arguments. </param>
    protected async virtual void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        if (InitializeSwitch.Dispatcher != null)
        {
            await InitializeSwitch.Dispatcher?.RunAsync(() => { PropertyChanged?.Invoke(this, args); });
        }
        else
        {
            PropertyChanged?.Invoke(this, args);
        }
    }

    /// <summary>Raises the property changed event for all properties (string.Empty). </summary>
    protected void RaiseAllPropertiesChanged()
    {
        RaisePropertyChanged(new PropertyChangedEventArgs(string.Empty));
    }
}