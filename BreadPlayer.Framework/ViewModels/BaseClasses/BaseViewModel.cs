using System.Collections.Generic;

namespace ViewModels
{
    /// <summary>
    /// When the VM is closed, the associated V needs to close too
    /// </summary>
    /// <param name="sender"></param>
    public delegate void ViewModelClosingEventHandler(bool? dialogResult);
    /// <summary>
    /// When a pre-existing VM is activated the View needs to activate itself
    /// </summary>
    public delegate void ViewModelActivatingEventHandler();
    /// <summary>
    /// A base class for all view models
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        public event ViewModelClosingEventHandler ViewModelClosing;
        public event ViewModelActivatingEventHandler ViewModelActivating;

        /// <summary>
        /// Keep a list of any children ViewModels so we can safely remove them when this ViewModel gets closed
        /// </summary>
        private List<BaseViewModel> _childViewModels = new List<BaseViewModel>();
        public List<BaseViewModel> ChildViewModels => _childViewModels;

        #region Bindable Properties

        #region ViewData
        private BaseViewData _viewData;
        public BaseViewData ViewData
        {
            get => _viewData;
            set
            {
                if (value != _viewData)
                {
                    _viewData = value;
                    RaisePropertyChanged("ViewData");
                }

            }
        }
        #endregion
        #endregion
      
        #region Constructor

        #endregion

        #region public methods
        /// <summary>
        /// De-Register the VM from the Messenger to avoid non-garbage collected VMs receiving messages
        /// Tell the View (via the ViewModelClosing event) that we're closing.
        /// </summary>
        public void CloseViewModel(bool? dialogResult)
        {
           // Controller.Messenger.DeRegister(this);
            ViewModelClosing?.Invoke(dialogResult);
            foreach (var childViewModel in _childViewModels)
            {
                childViewModel.CloseViewModel(dialogResult);
            }
        }

        public void ActivateViewModel()
        {
            ViewModelActivating?.Invoke();
        }
        #endregion

    }
}
