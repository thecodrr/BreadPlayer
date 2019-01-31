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

using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using BreadPlayer.Parsers;
using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Engines.BASSEngine
{
    public sealed class BassPlayerEngine : ObservableObject, IPlayerEngine
    {
        #region Fields

        private int _handle;
        private SyncProcedure _sync;
        private SyncProcedure _posSync;

        #endregion Fields

        #region Constructor

        public BassPlayerEngine(bool isMobile, bool crossFade, double deviceBufferSize)
        {
            DeviceBufferSize = deviceBufferSize;
            Init(isMobile);
            crossFade = CrossfadeEnabled;
            _sync = EndSync;
            _posSync = PositonReachedSync;
        }

        #endregion Constructor

        #region Initialize Methods

        /// <summary>
        /// Initializes the player to start playing audio
        /// </summary>
        /// <returns></returns>
        public async Task Init(bool isMobile)
        {
            await Task.Run(() =>
             {
                 Bass.UpdatePeriod = 230;
                 var ss = Bass.Start();

                 if (isMobile)
                 {
                     //we set it to a high value so that there are no cuts and breaks in the audio when the app is in background.
                     //This produces latency issue. When pausing a song, it will take 230ms. But I am sure, we can find a way around this later.
                     NativeMethods.BASS_SetConfig(NativeMethods.BassConfigDevBuffer, (int)DeviceBufferSize);
                 }
                 else
                     Bass.Configure(Configuration.IncludeDefaultDevice, true);

                 var s = Bass.Init();
             });
        }

        #endregion Initialize Methods

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

        public async Task ChangeDevice(string deviceName = null)
        {
            if (deviceName != null)
                await InitializeSwitch.NotificationManager.ShowStatusBarMessageAsync($"Transitioning to {deviceName}.");

            await Task.Run(async () =>
            {
                try
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

                            if (InitializeSwitch.IsMobile)
                                NativeMethods.BASS_SetConfig(NativeMethods.BassConfigDevBuffer, (int)DeviceBufferSize);

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
                }
                catch (Exception ex)
                {
                    BLogger.E($"Transition failed.", ex);
                    await InitializeSwitch.NotificationManager.ShowStatusBarMessageAsync($"Failed to transtion. Reason: {Bass.LastError}");
                }
                finally
                {
                    if (deviceName != null)
                        await InitializeSwitch.NotificationManager.ShowStatusBarMessageAsync($"Transition to {deviceName} complete.");
                }
            });

        }
        public async Task<bool> LoadURLAsync(Mediafile mediafile, string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return false;
            return await LoadMusicAsync(() =>
            {
                int metaSize = 1024 * 200; //1MB
                MemoryStream metadataStream = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(metadataStream);
                bool done = false;
                CurrentlyPlayingFile = mediafile;
                _handle = Bass.CreateStream(uri, 0, BassFlags.Default | BassFlags.Float | BassFlags.AutoFree, new DownloadProcedure((buffer, length, user) =>
                {
                    if (done)
                    {
                        return; //we are done here.
                    }
                    if (metadataStream.Length <= metaSize)
                    {
                        unsafe
                        {
                            // simply cast the given IntPtr to a native pointer to short values
                            // assuming you receive 16-bit sample data here
                            short* data = (short*)buffer;
                            for (int a = 0; a < length / 2; a++)
                            {
                                // write the received sample data to a local file
                                bw.Write(data[a]);
                            }
                        }
                    }
                    else
                    {
                        //there are HTTP Headers in the start of the recieved stream.
                        //we need to skip those so that we can parse ID3 tags.                        
                        //HTTP HEADER Removal START
                        var array = metadataStream.ToArray();
                        string str = Encoding.UTF8.GetString(array);
                        int l = str.IndexOf("ID3");
                        string headers = str.Substring(0, l);
                        byte[] id3TagArray = new byte[array.Length - l];
                        Buffer.BlockCopy(array, l, id3TagArray, 0, id3TagArray.Length);
                        //HTTP HEADER Removal END

                        var h = StringToHttpHeaders(headers);
                        ID3TagParser.WriteTagsToMediafile(mediafile, array, Length);
                        CurrentlyPlayingFile = mediafile;
                        MediaChanged?.Invoke(this, new EventArgs());

                        done = true;
                        metadataStream.Dispose();
                        bw.Dispose();
                    }
                }));
            });
        }
        private Dictionary<string, string> StringToHttpHeaders(string headerString)
        {
            var headers = headerString.Split(new string[] { ": ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            for (int i = 1; i < headers.Length; i++)
            {
                var key = headers[i];
                i++;
                if (i < headers.Length)
                {
                    var value = headers[i];
                    Headers.Add(key, value);
                }
            }
            return Headers;
        }
        public async Task<bool> LoadStreamAsync(Mediafile mediafile, byte[] array)
        {
            if (array?.Length <= 0)
                return false;
            return await LoadMusicAsync(() =>
            {
                _handle = Bass.CreateStream(array, 0, array.Length, BassFlags.Float);
                if (mediafile.MediaLocation != MediaLocationType.Local)
                {
                    ID3TagParser.WriteTagsToMediafile(mediafile, array, Length);
                    CurrentlyPlayingFile = mediafile;
                    MediaChanged?.Invoke(this, new EventArgs());
                }
            });
        }
        public async Task<bool> LoadLocalFileAsync(Mediafile mediaFile)
        {
            if ((mediaFile != null && mediaFile.Length != "00:00"))
            {
                return await LoadMusicAsync(() =>
                {
                    _handle = Bass.CreateStream(mediaFile.Path, 0, 0, BassFlags.AutoFree | BassFlags.Float);
                    CurrentlyPlayingFile = mediaFile;
                    MediaChanged?.Invoke(this, new EventArgs());
                });
            }
            else
            {
                string error = "The file " + mediaFile?.OrginalFilename + " is either corrupt, incomplete or unavailable. \r\n\r\n Exception details: No data available.";
                await InitializeSwitch.NotificationManager.ShowMessageAsync(error);
                return false;
            }
        }
        /// <summary>
        /// Loads the specified file into the player.
        /// </summary>
        /// <returns>Boolean</returns>
        private async Task<bool> LoadMusicAsync(Action LoadMusicAction)
        {
            try
            {
                await InitializeSwitch.Dispatcher.RunAsync(() =>
                {
                    MediaChanging?.Invoke(this, new EventArgs());
                });
                await Stop();
                await Task.Run(() =>
                    {
                        PlayerState = PlayerState.Stopped;

                        LoadMusicAction(); //loads the respective stream
                        if (Length <= 1)
                            Length = Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetLength(_handle));
                        IsSeekable = Bass.ChannelGetLength(_handle) != -1;
                        Bass.FloatingPointDSP = true;
                        Bass.ChannelSetDevice(_handle, 1);
                        Bass.ChannelSetSync(_handle, SyncFlags.End | SyncFlags.Mixtime, 0, _sync);
                        Bass.ChannelSetSync(_handle, SyncFlags.Position, Bass.ChannelSeconds2Bytes(_handle, Length - 5), _posSync);
                        Bass.ChannelSetSync(_handle, SyncFlags.Position, Bass.ChannelSeconds2Bytes(_handle, Length - 15), _posSync);

                    });
                if (InitializeSwitch.IsMobile)
                    await ChangeDevice();
                if (Equalizer == null)
                {
                    Equalizer = new BassEqualizer(_handle);
                }
                else
                {
                    (Equalizer as BassEqualizer).ReInit(_handle);
                }
                MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));

                return true;
            }
            catch (Exception ex)
            {
                BLogger.E("An error occured while loading music. Action {action}", ex, LoadMusicAction.ToString());
                await InitializeSwitch.NotificationManager.ShowMessageAsync(ex.Message);
                return false;
            }
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

        #endregion Methods

        #region Properties

        public bool CrossfadeEnabled
        {
            get; set;
        }

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

        private double _volume = 50;

        public double Volume
        {
            get => _volume;
            set
            {
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
            get
            {
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
        bool _isSeekable;
        public bool IsSeekable
        {
            get => _isSeekable;
            set => Set(ref _isSeekable, value);
        }
        public double DeviceBufferSize { get; set; }

        #endregion Properties

        private void PositonReachedSync(int handle, int channel, int data, IntPtr user)
        {
            if (Position >= Length - 15 && Position < Length - 5)
            {
                MediaAboutToEnd?.Invoke(this, new MediaAboutToEndEventArgs(CurrentlyPlayingFile));
            }
            else if (Position >= Length - 5 && CrossfadeEnabled)
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