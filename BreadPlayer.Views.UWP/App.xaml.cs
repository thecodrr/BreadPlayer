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
using BreadPlayer.Core;
using BreadPlayer.Core.Common;
using BreadPlayer.Helpers;
using BreadPlayer.Messengers;
using BreadPlayer.Services;
using BreadPlayer.Themes;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using BreadPlayer.Dispatcher;

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
            InitializeComponent();
            InitializeTheme();
            InitializeEverything();
        }
        private async void InitializeEverything()
        {
            if ((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music))?.SaveFolder != null)
            {
                var logPath = System.IO.Path.Combine((await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music))?.SaveFolder?.Path, ".breadplayerLogs", "BreadPlayer.log");
                BLogger.InitLogger(logPath, new Helpers.LogReportSender());
                CoreApplication.EnablePrelaunch(true);
                Suspending += OnSuspending;
                EnteredBackground += App_EnteredBackground;
                LeavingBackground += App_LeavingBackground;
                UnhandledException += App_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                BLogger.I("App started.");
                BLogger.I("Events initialized.");
            }
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            BLogger.E(string.Format("Task ({0}) terminating...", e.Exception.Source), e.Exception);
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            BLogger.F("Something caused the app to crash! Sender: {sender}", e.Exception, sender);
        }

        private void InitializeTheme()
        {
            var value = SettingsHelper.GetLocalSetting<string>("SelectedTheme", null);
            if (value != null)
            {
                BLogger.I("Setting theme: {theme}.", value);

                var theme = Enum.Parse(typeof(ApplicationTheme), value);

                RequestedTheme = (ApplicationTheme)theme;
                BLogger.I("Theme set: {theme}", theme);
            }
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            BLogger.I("App left background and is now in foreground...");
            deferral.Complete();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            BLogger.I("App has entered background...");
            CoreWindowLogic.SaveSettings();
            CoreWindowLogic.UpdateSmtc();
            deferral.Complete();
        }

        private Stopwatch _sessionWatch;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                _sessionWatch = Stopwatch.StartNew();
                BLogger.I("App launched and session started...");
                LoadFrame(e, e.Arguments);
            }
            else if (e.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                BLogger.I("Launched with arguments: {arguments}", e.Arguments);
                CoreWindowLogic.LoadAppWithArguments(e.Arguments);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            BLogger.E("Navigation failed while navigating to: {page}", e.Exception, e.SourcePageType);
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
            BLogger.I("App suspending. Saving state.");
            await LockscreenHelper.ResetLockscreenImage();
            CoreWindowLogic.SaveSettings();
            CoreWindowLogic.DisposeObjects();
            _sessionWatch?.Stop();
            BLogger.I("App suspended and session terminated. Session length: {length}.", _sessionWatch.Elapsed.TotalMinutes);
            deferral.Complete();
        }
        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Messenger.Instance.NotifyColleagues(MessageTypes.MsgExecuteCmd, new List<object> { args.Files, 0.0, true, 50.0 });
                BLogger.I("File was loaded successfully while app was running...");
            }
            else
            {
                LoadFrame(args, args.Files);
                BLogger.I("Player opened successfully with file as argument...");
            }
        }

        private async void LoadFrame(IActivatedEventArgs args, object arguments)
        {
            try
            {
                BLogger.I("Loading frame started...");
                //we need to init this in the UI thread regardless of this being the launch code. (Consult issue #207)
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    BLogger.I("Initializing switch...");
                    new InitializeSwitch(SharedLogic.Instance.NotificationManager, new SettingsHelper(), new BreadDispatcher(), ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1));
                });
                Frame rootFrame = Window.Current.Content as Frame;
                new InternetConnectivityHelper();
                var vm = Current.Resources["AlbumArtistVM"];
                ThemeManager.SetThemeColor(SettingsHelper.GetLocalSetting<string>("NowPlayingPicture", null));


                // Do not repeat app initialization when the Window already has content
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context
                    rootFrame = new Frame();
                    BLogger.I("New frame created.");
                    //if (args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                    //{
                    //    //CoreWindowLogic.ShowMessage("HellO!!!!!", "we are here");
                    //    //TODO: Load state from previously suspended application
                    //}

                    rootFrame.NavigationFailed += OnNavigationFailed;
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;

                    BLogger.I("Content set to Window successfully...");
                }
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    BLogger.I("Navigating to Shell...");
                    rootFrame.Navigate(typeof(Shell), arguments);
                }

                var view = ApplicationView.GetForCurrentView();
                view.SetPreferredMinSize(new Size(360, 100));
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    BLogger.I("Trying to hide status bar.");
                    await StatusBar.GetForCurrentView().HideAsync();
                    BLogger.I("Status bar hidden.");
                }
                else
                {
                    CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                    coreTitleBar.ExtendViewIntoTitleBar = true;
                }
                if (args.Kind != ActivationKind.File)
                {
                    CoreWindowLogic.LoadSettings();
                }
                else
                {
                    CoreWindowLogic.LoadSettings(true);
                }
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
                BLogger.E("Exception occured in LoadFrame Method. Arguments: {arguments}.", ex, arguments);
            }
        }
    }
}