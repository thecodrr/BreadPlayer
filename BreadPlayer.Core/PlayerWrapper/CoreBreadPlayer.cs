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
using ManagedBass;
using BreadPlayer.Events;
using BreadPlayer.Models;

namespace BreadPlayer.Core
{
    public sealed class CoreBreadPlayer : ObservableObject, IDisposable
    {
        #region Fields
        int handle = 0;
        private SyncProcedure _sync;
        private SyncProcedure _posSync;
        #endregion

        #region Constructor
        public CoreBreadPlayer()
        {
            Init();
            _sync = new SyncProcedure(EndSync);
            _posSync = new SyncProcedure(PositonReachedSync);
        }        
        #endregion

        #region Initialize Methods
        /// <summary>
        /// Initializes the player to start playing audio
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
           await Task.Run(() => 
            {
                Bass.UpdatePeriod = 1000;              
                Bass.Start();
                Bass.Init();
            });                   
        }
        private void InitializeExtensions(string path)
        {
            Effect = new Effects(handle);
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
                Bass.ChannelStop(handle); // Stop Playback.
                Bass.Stop();
                Bass.MusicFree(handle); // Free the Stream.
                Bass.Free(); // Frees everything (will have to call init again to play audio)              
                handle = 0;
                CurrentlyPlayingFile = null;
                PlayerState = PlayerState.Stopped;
            });
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
                    await Stop();
                    await Task.Run(() =>
                    {
                        handle = ManagedBass.Bass.CreateStream(path, 0, 0, BassFlags.AutoFree | BassFlags.Float);
                        PlayerState = PlayerState.Stopped;
                        Length = 0;
                        Length = Bass.ChannelBytes2Seconds(handle, Bass.ChannelGetLength(handle));
                        InitializeExtensions(path);
                        MediaStateChanged(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
                        Bass.ChannelSetSync(handle, SyncFlags.End | SyncFlags.Mixtime, 0, _sync);
                        Bass.ChannelSetSync(handle, SyncFlags.Position, Bass.ChannelSeconds2Bytes(handle, Length - 5), _posSync);
                        CurrentlyPlayingFile = mediaFile;
                    });

                    return true;
                }
                catch(Exception ex)
                {
                    await InitializeCore.NotificationManager.ShowMessageAsync(ex.Message + "||" + mediaFile.OrginalFilename);
                }
            }
            else
            { 
                string error = "The file " + mediaFile.OrginalFilename + " is either corrupt, incomplete or unavailable. \r\n\r\n Exception details: No data available.";
                if (IgnoreErrors == false)
                {
                    await InitializeCore.NotificationManager.ShowMessageBoxAsync(error, "File corrupt");
                }
                else
                {
                    await InitializeCore.NotificationManager.ShowMessageAsync(error);
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
            MediaStateChanged(this, new MediaStateChangedEventArgs(PlayerState.Paused));
            await Task.Run(async () =>
            {
                Bass.ChannelSlideAttribute(handle, ChannelAttribute.Volume, 0, 700);
                await Task.Delay(700);
                Bass.ChannelPause(handle);
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
                var vol = (float)Volume / 100f;
                Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, 0f);
                ManagedBass.Bass.ChannelPlay(handle);
                Bass.ChannelSlideAttribute(handle, ChannelAttribute.Volume, vol, 3000);
                PlayerState = PlayerState.Playing;
                MediaStateChanged(this, new MediaStateChangedEventArgs(PlayerState.Playing));
            });

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
                Bass.StreamFree(handle);
                Bass.ChannelStop(handle); // Stop Playback.
                Bass.MusicFree(handle);                
                handle = 0;
                CurrentlyPlayingFile = null;
                PlayerState = PlayerState.Stopped;
                MediaStateChanged(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
            });

        }
        #endregion

        #region Properties
        public Effects Effect { get; set; }
        double _volume = 50;
        public double Volume
        {
            get { return _volume; }
            set {
                Set(ref _volume, value);
                Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, _volume / 100);               
            }
        }    

        double _seek = 0;
        public double Position
        {
            get { return Bass.ChannelBytes2Seconds(handle, Bass.ChannelGetPosition(handle)); }
            set
            {
                Task.Run(() =>
                {
                    Set(ref _seek, value);
                    Bass.ChannelSetPosition(handle, ManagedBass.Bass.ChannelSeconds2Bytes(handle, _seek));
                });
            }            
        }
        double _length;
        public double Length
        {
            get {
                if (_length <= 0)
                    _length = 1;
                return _length;
            }
            set
            {
                Set(ref _length, value);
            }
        }      

        public PlayerState PlayerState
        {
            get; set;
        }
        Mediafile _currentPlayingFile;
        public Mediafile CurrentlyPlayingFile
        {
            get { return _currentPlayingFile; }
            set
            {
                Set(ref _currentPlayingFile, value);
            }
        }
        bool _ignoreErrors = false;
        public bool IgnoreErrors
        {
            get { return _ignoreErrors; }
            set { Set(ref _ignoreErrors, value); }
        }

        #endregion
        private void PositonReachedSync(int handle, int channel, int data, IntPtr user)
        {
            Bass.ChannelSlideAttribute(handle, ChannelAttribute.Volume, 0, 5000);
        }
        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            MediaEnded(this, new MediaEndedEventArgs(PlayerState.Ended));
        }
        public event OnMediaStateChanged MediaStateChanged;
        public event OnMediaEnded MediaEnded;
    }

    public delegate void OnMediaStateChanged(object sender, MediaStateChangedEventArgs e);
    public delegate void OnMediaEnded(object sender, MediaEndedEventArgs e);
}
