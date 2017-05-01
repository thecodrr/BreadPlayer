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
using BreadPlayer.Common;
using BreadPlayer.Services;
using BreadPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BreadPlayer.Helpers;

namespace BreadPlayer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {           
            this.InitializeComponent();
            CoreApplication.EnablePrelaunch(true);
            InitializeTheme();
            this.Suspending += OnSuspending;
            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
           
        }

        private void InitializeTheme()
        {
            var value = RoamingSettingsHelper.GetSetting<string>("SelectedTheme", "Light");
            var theme = Enum.Parse(typeof(ApplicationTheme), value.ToString());
            this.RequestedTheme = (ApplicationTheme)theme;
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            deferral.Complete();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            CoreWindowLogic.SaveSettings();
            CoreWindowLogic.UpdateSmtc();
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
            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                LoadFrame(e, e.Arguments);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
           throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            await LockscreenHelper.ResetLockscreenImage();
            CoreWindowLogic.SaveSettings();
            await Task.Delay(500);
            deferral.Complete();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Messengers.Messenger.Instance.NotifyColleagues(Messengers.MessageTypes.MSG_EXECUTE_CMD, new List<object> { args.Files[0], 0.0, true, 50.0 });
                // ShellVM.Play(args.Files[0]);
            }
            else
            {
                LoadFrame(args, args.Files[0]);
            }
        }

        void LoadFrame(IActivatedEventArgs args, object arguments)
        {
            try
            {
               // BLogger.Logger.Info("Loading frame started...");
                Frame rootFrame = Window.Current.Content as Frame;

                // Do not repeat app initialization when the Window already has content
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context
                    rootFrame = new Frame();
                  //  BLogger.Logger.Info("New frame created.");
                    if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //CoreWindowLogic.ShowMessage("HellO!!!!!", "we are here");
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
                
                 var view = ApplicationView.GetForCurrentView();
                 view.SetPreferredMinSize(new Size(360, 100));
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                }
                if (args.Kind != ActivationKind.File)
                {
                    CoreWindowLogic.LoadSettings();
                }
                else
                {
                    CoreWindowLogic.LoadSettings(true);
                }
                //var vm = (this.Resources["AccountsVM"] as AccountsViewModel);
                Window.Current.Activate();
            }
            catch (Exception)
            {
            }
        }

        void ReInitialize()
        {
            if (Window.Current.Content == null)
            {
                var frame = new Frame();
                frame.Navigate(typeof(Shell));
                Window.Current.Content = frame;
            }
        }
        public void ReduceMemoryUsage()
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
                NavigationService.Instance = null;
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
