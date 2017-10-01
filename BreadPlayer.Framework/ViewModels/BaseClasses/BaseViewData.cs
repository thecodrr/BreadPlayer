namespace ViewModels
{
    /// <summary>
    /// The base class from which all View Data objects inherit.
    /// Just an Observable Object right now - but a separate abstract class in case we want to add
    /// to it while not modifying ObservableObject itslef.
    /// </summary>
    public abstract class BaseViewData : ObservableObject
    {
    }
}