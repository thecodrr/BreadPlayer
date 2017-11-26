using System;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Core.Common;
using BreadPlayer.Core.Engines.BASSEngine;
using BreadPlayer.Core.Engines.Interfaces;
using BreadPlayer.Core.Events;
using BreadPlayer.Core.Models;
using BreadPlayer.Fmod.Enums;
using static BreadPlayer.Fmod.Callbacks;
using BreadPlayer.Fmod;
using BreadPlayer.Fmod.Structs;
using BreadPlayer.Interfaces;
using System.IO;
using System.Runtime.InteropServices;
using BreadPlayer.Parsers;

namespace BreadPlayer.Core.Engines
{
    public sealed class FmodPlayerEngine : ObservableObject, IPlayerEngine
    {
        #region Fields

        private FmodSystem _fmodSys;
        private Sound _fmodSound;
        private Channel _fmodChannel;
        private ChannelCallback _channelEndCallback;
        private IntPtr _endSyncPoint;
        private IntPtr _last5SyncPoint;
        private IntPtr _last15SyncPoint;
        private uint _last15Offset;
        private bool _isMobile;
        #endregion

        public FmodPlayerEngine(bool isMobile)
        {
            _isMobile = isMobile;
            Init(isMobile);
        }

        #region Methods 
        public async Task Init(bool isMobile)
        {
            await Task.Run(() =>
            {
                var res = Factory.SystemCreate(out _fmodSys);
                res = Fmod.Debug.Initialize(DebugFlags.Log | DebugFlags.Error, DebugMode.File, null, @"D:\Music\fmod.log");
                res = _fmodSys.Init(1, InitFlags.Normal, IntPtr.Zero);
                //res = _fmodSys.SetFileSystem(new FileOpenCallback(Open), new FileCloseCallback(Close), new FileReadCallback(Read), new FileSeekCallback(Seek), null, null,-1);
                _channelEndCallback = new ChannelCallback(ChannelEndCallback);
            });
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
                Result loadResult = Result.Ok;
                await Task.Run(() =>
                {
                    PlayerState = PlayerState.Stopped;
                    LoadMusicAction(); //loads the respective stream
                    //load the stream into the channel but don't play it yet.
                    loadResult = _fmodSys.PlaySound(_fmodSound, null, true, out _fmodChannel);
                });

                //get and update length of the track.
                Length = TimeSpan.FromMilliseconds(_fmodSound.LengthInMilliseconds).TotalSeconds;

                //set the channel callback for all the syncpoints
                loadResult = _fmodChannel.SetCallback(_channelEndCallback);

                //add all the sync points
                //1. when song ends
                loadResult = _fmodSound.AddSyncPoint(_fmodSound.LengthInMilliseconds, TimeUnit.Ms, "songended", out _endSyncPoint);

                //2. when song has reached the last 15 seconds
                loadResult = _fmodSound.AddSyncPoint(_fmodSound.LengthInMilliseconds - 15000, TimeUnit.Ms, "songabouttoended", out _last15SyncPoint);

                //3. when song has reached the last 5 seconds
                loadResult = _fmodSound.AddSyncPoint(_fmodSound.LengthInMilliseconds - 5000, TimeUnit.Ms, "fade", out _last5SyncPoint);

                //update the system once here so that 
                //all the sync points and callbacks are saved and updated.
                loadResult = _fmodSys.Update();

                await InitializeSwitch.Dispatcher.RunAsync(() =>
                {
                    if (Equalizer == null)
                    {
                        Equalizer = new FmodEqualizer(_fmodSys, _fmodChannel);
                    }
                    else
                    {
                        (Equalizer as FmodEqualizer).ReInit(_fmodSys, _fmodChannel);
                    }
                    MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
                });
                return true;
            }
            catch (Exception ex)
            {
                BLogger.E("An error occured while loading music. Action {action}", ex, LoadMusicAction.ToString());
                await InitializeSwitch.NotificationManager.ShowMessageAsync(ex.Message);
                return false;
            }
        }

        public Task Pause()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Paused));
            return Task.Run(async () =>
            {
                //set state to paused before we pause
                //this is to update the UI quickly.
                PlayerState = PlayerState.Paused;

                //set fade points to first 3 seconds of the track.
                //we simply slide the volume from default value to 0 in the next 0.5 second.
                _fmodChannel.SetFadePoint(_fmodChannel.Volume, 0f, _fmodSound.ConvertSecondsToPcm(0.5));

                //wait for the fade to over.
                await Task.Delay(500);

                //set paused to true
                _fmodChannel.SetPaused(true);
            });
        }

        public Task Play()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Playing));
            return Task.Run(() =>
            {
                //set paused to false
                _fmodChannel.SetPaused(false);

                //update volume.
                Volume = Volume;

                //set fade points to first 3 seconds of the track.
                //we simply slide the volume from 0 to the default value
                //in the next 1 second.
                _fmodChannel.SetFadePoint(0f, _fmodChannel.Volume, _fmodSound.ConvertSecondsToPcm(1));

                PlayerState = PlayerState.Playing;
            });
        }

        public Task Stop()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
            return Task.Run(() =>
            {
                _fmodChannel?.Stop();
                _fmodSound?.Release();
                Length = 0;
                Position = -1;
                CurrentlyPlayingFile = null;
                PlayerState = PlayerState.Stopped;
            });
        }
        private void SetLoop()
        {
            _fmodChannel.SetMode(_isLoopingEnabled ? Mode.LoopNormal : Mode.LoopOff);
            _fmodChannel.SetLoopCount(_isLoopingEnabled ? -1 : 0);
        }
        #endregion

        #region Callbacks
        private Result ChannelEndCallback(IntPtr channelraw, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2)
        {
            if (type == ChannelControlCallbackType.Syncpoint)
            {
                _fmodSound?.GetSyncPointInfo(_last15SyncPoint, new StringBuilder("songabouttoend"), 0, out _last15Offset, TimeUnit.Ms);
                uint last5Offset = 0;
                _fmodSound?.GetSyncPointInfo(_last5SyncPoint, new StringBuilder("fade"), 0, out last5Offset, TimeUnit.Ms);

                if (_position >= _fmodSound?.LengthInMilliseconds)
                {
                    MediaEnded?.Invoke(this, new MediaEndedEventArgs(PlayerState.Ended));
                }
                else if (_position >= last5Offset)
                {
                    _fmodChannel.SetFadePoint(_fmodChannel.Volume, 0f, _fmodChannel.GetTotalSamplesLeft(_fmodSound));
                }
                else if (_position >= _last15Offset && _position < last5Offset)
                {
                    MediaAboutToEnd?.Invoke(this, new MediaAboutToEndEventArgs(CurrentlyPlayingFile));
                }
            }
            return Result.Ok;
        }
        #endregion

        #region Properties

        private bool _isLoopingEnabled;
        public bool IsLoopingEnabled
        {
            get => _isLoopingEnabled;
            set
            {
                Set(ref _isLoopingEnabled, value);
                SetLoop();
            }
        }

        private bool _isVolumeMuted;
        public bool IsVolumeMuted
        {
            get => _isVolumeMuted;
            set
            {
                Set(ref _isVolumeMuted, value);
                _fmodChannel.SetMute(_isVolumeMuted);
            }
        }
        private double _volume = 50;
        public double Volume
        {
            get => _volume;
            set
            {
                Set(ref _volume, value);
                if (_fmodChannel != null)
                {
                    _fmodChannel.Volume = (float)(_volume / 100);
                }
            }
        }

        private double _seek;
        private uint _position;
        public double Position
        {
            get
            {

                _fmodChannel?.GetPosition(out _position, TimeUnit.Ms);
                _fmodSys?.Update();
                return TimeSpan.FromMilliseconds(_position).TotalSeconds;
            }
            set
            {
                Set(ref _seek, value);
                _fmodChannel?.SetPosition(Convert.ToUInt32(TimeSpan.FromSeconds(value < 0 ? 0 : value).TotalMilliseconds), TimeUnit.Ms);
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

                return _length <= 0 ? 1 : _length;
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
        public bool CrossfadeEnabled { get; set; }
        public double DeviceBufferSize { get; set; }
        public bool IsSeekable { get; set; } = true;
        #endregion

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FMODPlayerEngine() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public async Task<bool> LoadLocalFileAsync(Mediafile mediaFile)
        {
            if ((mediaFile != null && mediaFile.Length != "00:00"))
            {
                return await LoadMusicAsync(() =>
                {
                    //create a stream of the new track
                    var loadResult = _fmodSys.CreateStream(mediaFile.Path, _isMobile ? Mode.LowMem | Mode.IgnoreTags : Mode.Default | Mode.CreateCompressedSample, out _fmodSound);
                    //load the stream into the channel but don't play it yet.
                    loadResult = _fmodSys.PlaySound(_fmodSound, null, true, out _fmodChannel);
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

        public async Task<bool> LoadURLAsync(Mediafile mediafile, string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return false;
            return await LoadMusicAsync(() =>
            {
                //create a stream of the new track
                _fmodSys.SetStreamBufferSize(64 * 1024, TimeUnit.Rawbytes);
                
                var loadResult = _fmodSys.CreateStream(uri, Mode.CreateStream, out _fmodSound);
                while (_fmodSound.GetTag(null, -1, out Tag tag) == Result.Ok)
                {
                    if(tag.datatype == TagDataType.String)
                    {

                    }
                }
                CurrentlyPlayingFile = mediafile;
                MediaChanged?.Invoke(this, new EventArgs());
            });
        }
        public async Task<bool> LoadStreamAsync(Mediafile mediafile, byte[] array)
        {
            if (array?.Length <= 0)
                return false;
            return await LoadMusicAsync(() =>
            {
                var exInfo = new CreateSoundExInfo();
                exInfo.cbsize = Marshal.SizeOf(exInfo);
                exInfo.length = (uint)array.Length;
                var loadResult = _fmodSys.CreateStream(array, Mode.OpenMemory, ref exInfo, out _fmodSound);

                if (mediafile.MediaLocation != MediaLocationType.Device)
                {
                    ID3TagParser.WriteTagsToMediafile(mediafile, array, Length);
                    CurrentlyPlayingFile = mediafile;
                    MediaChanged?.Invoke(this, new EventArgs());
                }
            });
        }

        public Task ChangeDevice(string deviceName)
        {
            return null;
            //throw new NotImplementedException();
        }
        #endregion

        public event OnMediaStateChanged MediaStateChanged;
        public event OnMediaEnded MediaEnded;
        public event OnMediaAboutToEnd MediaAboutToEnd;
        public event OnMediaChanging MediaChanging;
        public event OnMediaChanging MediaChanged;
    }
}
