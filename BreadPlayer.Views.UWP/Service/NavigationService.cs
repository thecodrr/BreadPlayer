using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BreadPlayer.Services
{
    public class NavigationService
    {
        public Type HomePage { get; set; }
        /// &lt;summary>
        /// This holds the instance to the Only NavigationService in this app.
        /// &lt;/summary>
        public static NavigationService Instance { get; set; }

        /// &lt;summary>
        /// This will hold the reference to the frame that is to be manipulated.
        /// &lt;/summary>
        public Frame Frame { get; set; }

        /// &lt;summary>
        /// The Stack of pages to enable Stack based Navigation.
        /// &lt;/summary>
        public Stack<Type> PageStack { get; protected set; }

        #region CTOR
        /// &lt;summary>
        /// The default constructor to instantiate this class with reference to a frame
        /// &lt;/summary>
        /// &lt;param name="frame">The referenced frame&lt;/param>
        public NavigationService(ref Frame frame, Type homePage)
        {
            //Check is the instance doesnt already exist.
            if (Instance != null)
            {
                //if there is an instance in the app already present then simply throw an error.
                throw new Exception("Only one navigation service can exist in a App.");
            }
            //setting the instance to the static instance field.
            Instance = this;
            //setting the frame reference.
            this.Frame = frame;
            //setting the homepage
            this.HomePage = homePage;
            //initializing the stack.
            this.PageStack = new Stack<Type>();

            //Hooking up the events for BackRequest both for Big Windows and for Phone.

            //SystemNavigationManager.GetForCurrentView().BackRequested +=
            //                NavigationService_BackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {

            }
        }

        #endregion

        #region Navigation Methods

        public void NavigateTo(Type pageType, object parameter)
        {
            if (PageStack.Count > 0 && PageStack.Peek() == pageType)
            {
                return;
            }
            PageStack.Push(pageType);
            Frame.Navigate(pageType, parameter);
            UpdateBackButtonVisibility();
        }

        public void NavigateBack()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            PageStack.Pop();
            UpdateBackButtonVisibility();
        }

        public void NavigateToHome()
        {
            if (HomePage != null)
                Frame.Navigate(HomePage, "Home");
        }        
        public bool Reload(object param)
        {
            Type type = this.Frame.CurrentSourcePageType;
            
            try { return this.Frame.Navigate(type, param); }
            finally { this.Frame.BackStack.Remove(this.Frame.BackStack.Last()); }
        }
        #endregion


        #region BackButtonVisibilty Region
        void UpdateBackButtonVisibility()
        {
                SystemNavigationManager.GetForCurrentView().
                    AppViewBackButtonVisibility = Frame.CanGoBack ?
                     AppViewBackButtonVisibility.Visible :
                         AppViewBackButtonVisibility.Collapsed;
        }
        #endregion

        #region Event Methods for windows and phone

        private void NavigationService_BackRequested
            (object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        //private void HardwareButtons_BackPressed
        //(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        //{
        //    this.NavigateBack();
        //}

        #endregion
    }
}
