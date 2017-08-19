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
using System.Threading.Tasks;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using ManagedBass;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public sealed class BassPlayerEngine : ObservableObject, IPlayerEngine
    {
        #region Fields

        private int _handle;
        private SyncProcedure _sync;
        private SyncProcedure _posSync;
        #endregion

        #region Constructor
        public BassPlayerEngine(bool isMobile)
        {
            Init(isMobile);
            _sync = EndSync;
            _posSync = PositonReachedSync;
        }
        #endregion

        #region Initialize Methods
        /// <summary>
        /// Initializes the player to start playing audio
        /// </summary>
        /// <returns></returns>
        public async Task Init(bool isMobile)
        {
           await Task.Run(async() => 
            {
                try
                {
                    Bass.UpdatePeriod = 230;
                    Bass.Start();

                    if (isMobile)
                    {
                        //we set it to a high value so that there are no cuts and breaks in the audio when the app is in background.
                        //This produces latency issue. When pausing a song, it will take 230ms. But I am sure, we can find a way around this later. 
                        NativeMethods.BASS_SetConfig(NativeMethods.BassConfigDevBuffer, 230);
                    }
                    else
                    {
                        Bass.Configure(Configuration.IncludeDefaultDevice, true);
                    }
                    Bass.Init();
                }
                catch (Exception)
                {
                    await Init(isMobile);
                }
            });                   
        }
        #endregion

        #region Methods
        /// <summary>
        /// Swipes the player memory clean.
        /// </summary>
        /// <returns></returns>
        public async void Dispose()
        {
            await Task.Run(() =>
            {                
                Bass.ChannelStop(_handle); // Stop Playback.
                Bass.Stop();
                Bass.MusicFree(_handle); // Free the Stream.
                Bass.Free(); // Frees everything (will have to call init again to play audio)              
                _handle = 0;
                CurrentlyPlayingFile = null;
                PlayerState = PlayerState.Stopped;
            });
        }


        public async Task ChangeDevice(string deviceName)
        {
            await InitializeCore.NotificationManager.ShowMessageAsync($"Transitioning to {deviceName}.", 5);

            await Task.Run(() =>
            {
                var count = Bass.DeviceCount;
                for (var i = 0; i < count; i++)
                {
                    var deviceInfo = Bass.GetDeviceInfo(i);
                    if (deviceInfo.IsDefault && deviceInfo.IsEnabled)
                    {
                        var isPlaying = PlayerState == PlayerState.Playing;
                        if (isPlaying)
                        {
                            Bass.ChannelPause(_handle);
                            PlayerState = PlayerState.Paused;
                        }

                        if (InitializeCore.IsMobile)
                            NativeMethods.BASS_SetConfig(NativeMethods.BassConfigDevBuffer, 230);

                        Bass.Init();
                        Bass.ChannelSetDevice(_handle, i);
                        if (isPlaying)
                        {
                            Bass.ChannelPlay(_handle);
                            PlayerState = PlayerState.Playing;
                        }
                        return;
                    }
                }
            });

            await InitializeCore.NotificationManager.ShowMessageAsync($"Transition to {deviceName} complete.", 5);
        }

        /// <summary>
        /// Loads the specified file into the player.
        /// </summary>
        /// <param name="fileName">Path to the music file.</param>
        /// <returns>Boolean</returns>
        public async Task<bool> Load(Mediafile mediaFile)
        {
            if (mediaFile != null && mediaFile.Length != "00:00")
            {
                try
                {
                    string path = mediaFile.Path;
                    
                    await InitializeCore.Dispatcher.RunAsync(() =>
                    {
                        MediaChanging?.Invoke(this, new EventArgs());
                    });
                    await Stop();
                    await Task.Run(() =>
                        {
                            _handle = Bass.CreateStream(path, 0, 0, BassFlags.AutoFree | BassFlags.Float);
                            PlayerState = PlayerState.Stopped;
                            Length = 0;
                            Length = Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetLength(_handle));
                            Bass.FloatingPointDSP = true;
                            Bass.ChannelSetDevice(_handle, 1);
                            Bass.ChannelSetSync(_handle, SyncFlags.End | SyncFlags.Mixtime, 0, _sync);
                            Bass.ChannelSetSync(_handle, SyncFlags.Position, Bass.ChannelSeconds2Bytes(_handle, Length - 5), _posSync);
                            Bass.ChannelSetSync(_handle, SyncFlags.Position, Bass.ChannelSeconds2Bytes(_handle, Length - 15), _posSync);

                            CurrentlyPlayingFile = mediaFile;
                        });
                    if (Equalizer == null)
                    {
                        Equalizer = new BassEqualizer(_handle);
                    }
                    else
                    {
                        (Equalizer as BassEqualizer).ReInit(_handle);
                    }
                    MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
                    MediaChanged?.Invoke(this, new EventArgs());
                    return true;
                }
                catch (Exception ex)
                {
                    await InitializeCore.NotificationManager.ShowMessageAsync(ex.Message + "||" + mediaFile.OrginalFilename);
                }
            }
            else
            {
                string error = "The file " + mediaFile?.OrginalFilename + " is either corrupt, incomplete or unavailable. \r\n\r\n Exception details: No data available.";
                if (IgnoreErrors)
                {
                    await InitializeCore.NotificationManager.ShowMessageAsync(error);
                }
                else
                {
                    await InitializeCore.NotificationManager.ShowMessageBoxAsync(error, "File corrupt");
                }
            }
            return false;
        }

        
        /// <summary>
        /// Pauses the audio playback.
        /// </summary>
        /// <returns></returns>
        public async Task Pause()
        {
            PlayerState = PlayerState.Paused;
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Paused));
            await Task.Run(async () =>
            {
                Bass.ChannelSlideAttribute(_handle, ChannelAttribute.Volume, 0, 500);
                await Task.Delay(500);
                Bass.ChannelPause(_handle);
                //var vol = (float)Volume / 100f;
                //Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, vol);
            });

        }
        /// <summary>
        /// Starts the audio playback.
        /// </summary>
        /// <returns></returns>
        public async Task Play()
        {
            await Task.Run(() =>
            {
                Bass.ChannelSetAttribute(_handle, ChannelAttribute.Volume, 0f);
                Bass.ChannelPlay(_handle);
                var vol = (float)Volume / 100f;
                Bass.ChannelSlideAttribute(_handle, ChannelAttribute.Volume, vol, 1000);
               
            });
            PlayerState = PlayerState.Playing;
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Playing));
        }
        /// <summary>
        /// Stops the playback if it is playing.
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            await Task.Run(() =>
            {
                Length = 0;
                Position = -1;
                Bass.StreamFree(_handle);
                Bass.ChannelStop(_handle); // Stop Playback.
                Bass.MusicFree(_handle);                
                _handle = 0;
                CurrentlyPlayingFile = null;
            });
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
            PlayerState = PlayerState.Stopped;
        }
        #endregion

        #region Properties

        private bool _isVolumeMuted;
        public bool IsVolumeMuted
        {
            get => _isVolumeMuted;
            set
            {
                Set(ref _isVolumeMuted, value);
                if (value)
                {
                    Bass.Volume = 0;
                }
                else
                {
                    Bass.Volume = 1;
                }
            }
        }

        private Effects _effect;
        public Effects Effect
        {
            get => _effect;
            set => Set(ref _effect, value);
        }

        private double _volume = 50;
        public double Volume
        {
            get => _volume;
            set {
                Set(ref _volume, value);
                Bass.ChannelSetAttribute(_handle, ChannelAttribute.Volume, _volume / 100);               
            }
        }


        private double _seek;
        public double Position
        {
            get => Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetPosition(_handle));
            set
            {
                Task.Run(() =>
                {
                    Set(ref _seek, value);
                    Bass.ChannelSetPosition(_handle, Bass.ChannelSeconds2Bytes(_handle, _seek));
                });
            }            
        }

        private double _length;
        public double Length
        {
            get {
                if (_length <= 0)
                {
                    _length = 1;
                }

                return _length;
            }
            set => Set(ref _length, value);
        }      

        public PlayerState PlayerState
        {
            get; set;
        }

        private Mediafile _currentPlayingFile;
        public Mediafile CurrentlyPlayingFile
        {
            get => _currentPlayingFile;
            set => Set(ref _currentPlayingFile, value);
        }

        private bool _ignoreErrors;
        public bool IgnoreErrors
        {
            get => _ignoreErrors;
            set => Set(ref _ignoreErrors, value);
        }

        private Equalizer _fmodEqualizer;
        public Equalizer Equalizer
        {
            get => _fmodEqualizer;
            set => Set(ref _fmodEqualizer, value);
        }
        private bool _isLoopingEnabled;
        public bool IsLoopingEnabled
        {
            get => _isLoopingEnabled;
            set
            {
                Set(ref _isLoopingEnabled, value);
                //SetLoop();
            }
        }        
        #endregion
        private void PositonReachedSync(int handle, int channel, int data, IntPtr user)
        {
            if (Position >= Length - 15 && Position < Length - 5)
            {
                MediaAboutToEnd?.Invoke(this, new MediaAboutToEndEventArgs(CurrentlyPlayingFile));
            }
            else if(Position >= Length - 5)
            {
                Bass.ChannelSlideAttribute(handle, ChannelAttribute.Volume, 0, 5000);
            }
            //MediaEnded?.Invoke(this, new MediaEndedEventArgs(PlayerState.Ended));
        }
        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            MediaEnded?.Invoke(this, new MediaEndedEventArgs(PlayerState.Ended));
        }
        public event OnMediaStateChanged MediaStateChanged;
        public event OnMediaEnded MediaEnded;
        public event OnMediaAboutToEnd MediaAboutToEnd;
        public event OnMediaChanging MediaChanging;
        public event OnMediaChanging MediaChanged;
    }

    public delegate void OnMediaStateChanged(object sender, MediaStateChangedEventArgs e);
    public delegate void OnMediaEnded(object sender, MediaEndedEventArgs e);
    public delegate void OnMediaAboutToEnd(object sender, MediaAboutToEndEventArgs e);
    public delegate void OnMediaChanging(object sender, EventArgs e);
}
