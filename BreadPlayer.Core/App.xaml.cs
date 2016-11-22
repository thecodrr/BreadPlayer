/* 
	BreadPlayer. A music player made for Windows 10 store.
    Copyright (C) 2016  theweavrs (Abdullah Atta)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BreadPlayer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        bool isInBackgroundMode;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            var value = ApplicationData.Current.LocalSettings.Values["SelectedTheme"];
            if (value != null)
            {
                var theme = Enum.Parse(typeof(ApplicationTheme), value.ToString());
                this.RequestedTheme = (ApplicationTheme)theme;
                Debug.Write("ApplicationTheme: " + RequestedTheme.ToString());
            }
            else
            {
                this.RequestedTheme = ApplicationTheme.Light;
            }
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(10, 10));
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
           // RegisterTask();



        }
       
        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            CoreWindowLogic.EnableDisableSmtc();
            CoreWindowLogic.isBackground = false;
            deferral.Complete();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            CoreWindowLogic.Stringify();
            CoreWindowLogic.UpdateSmtc(true);
            CoreWindowLogic.EnableDisableSmtc();
            await Task.Delay(200);
            CoreWindowLogic.isBackground = true;
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;                
            }
#endif
          if(e.PreviousExecutionState != ApplicationExecutionState.Running)
            LoadFrame(e, e.Arguments);           
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            CoreWindowLogic.Stringify();
            await Task.Delay(500);
            deferral.Complete();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
               // ShellVM.Play(args.Files[0]);
            }
            else
            {
                LoadFrame(args, args.Files[0]);
            }
        }

        void LoadFrame(IActivatedEventArgs args, object arguments)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (args.Kind != ActivationKind.File)
            {
                CoreWindowLogic.Replay();
            }
            else
            {
                CoreWindowLogic.Replay(true);
            }
            // Do not repeat app initialization when the Window already has content
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                rootFrame.NavigationFailed += OnNavigationFailed;
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }               
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(Shell), arguments);
            }
            
            // CoreWindowLogic logic = new CoreWindowLogic();
            var view = ApplicationView.GetForCurrentView();
            if (RequestedTheme == ApplicationTheme.Dark)
            {
                view.TitleBar.BackgroundColor = Color.FromArgb(20, 20, 20, 1);
                view.TitleBar.ButtonBackgroundColor = Color.FromArgb(20, 20, 20, 1);                
            }
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {              
                //view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = RequestedTheme == ApplicationTheme.Light ? (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color : Color.FromArgb(20, 20, 20, 1);
                statusBar.BackgroundOpacity = 1;
                statusBar.ForegroundColor = Colors.White;
            }
           
            Window.Current.Activate();
            
          
        }

        public void ReduceMemoryUsage(ulong limit)
        {
            // If the app has caches or other memory it can free, it should do so now.
            // << App can release memory here >>

            // Additionally, if the application is currently
            // in background mode and still has a view with content
            // then the view can be released to save memory and
            // can be recreated again later when leaving the background.
            if (Window.Current.Content != null)
            {
                // Some apps may wish to use this helper to explicitly disconnect
                // child references.
                 VisualTreeHelper.DisconnectChildrenRecursive(Window.Current.Content);

                // Clear the view content. Note that views should rely on
                // events like Page.Unloaded to further release resources.
                // Release event handlers in views since references can
                // prevent objects from being collected.
                Window.Current.Content = null;
            }

            // Run the GC to collect released resources.
            GC.Collect();
        }

    }
}