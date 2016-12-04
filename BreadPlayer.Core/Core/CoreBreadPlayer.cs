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
using Windows.Storage;
using ManagedBass;
using BreadPlayer.Events;
using BreadPlayer.Models;
using Windows.UI.Xaml.Media;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

namespace BreadPlayer.Core
{
	public class CoreBreadPlayer : ViewModelBase, IDisposable
    {
        #region DllImports
        [DllImport("bass.dll")]
        static extern bool BASS_SetConfig(int config, int newValue);
        const int BASS_CONFIG_DEV_BUFFER = 27;
        #endregion

        #region Fields
        int handle = 0;
        private SyncProcedure _sync;
        #endregion

        #region Constructor
        public CoreBreadPlayer()
        {
            Init();
            _sync = new SyncProcedure(EndSync);
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
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                {
                    //we set it to a high value so that there are no cuts and breaks in the audio when the app is in background.
                    //This produces latency issue. When pausing a song, it will take 700ms. But I am sure, we can find a way around this later. 
                    BASS_SetConfig(BASS_CONFIG_DEV_BUFFER, 230); 
                }
                Bass.Start();
                Bass.Init();
            });                   
        }
        private void InitializeExtensions(string path)
        {
            ////Tags = new CoreTags(path);
            ////Effect = new Effects(handle);
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
        public async Task<bool> Load(Mediafile mp3file)
        {            
            if (mp3file != null && mp3file.Length != "00:00")
            {
                try
                {
                    string path = mp3file.Path;
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
                        CurrentlyPlayingFile = mp3file;
                        CoreWindowLogic.UpdateSmtc();
                        CoreWindowLogic.Stringify();
                    });

                    return true;
                }
                catch(Exception ex)
                {
                    await NotificationManager.ShowAsync(ex.Message + "||" + mp3file.OrginalFilename);
                }
            }
            else
            { 
                string error = "The file " + mp3file.OrginalFilename + " is either corrupt, incomplete or unavailable. \r\n\r\n Exception details: No data available.";
                if (IgnoreErrors == false)
                {
                    CoreWindowLogic.ShowMessage(error, "File corrupt");
                }
                else
                {
                    await NotificationManager.ShowAsync(error);
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
            await Task.Run(() =>
            {
                Bass.ChannelPause(handle);
                PlayerState = PlayerState.Paused;
                MediaStateChanged(this, new MediaStateChangedEventArgs(PlayerState.Paused));
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
                //Bass.Start();
                ManagedBass.Bass.ChannelPlay(handle);
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
        double _volume = 50;
        public double Volume
        {
            get { return _volume; }
            set {
                Set(ref _volume, value);
                Bass.Volume =  _volume / 100;
            }
        }
        public Effects Effect
        {
            get; set;
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
