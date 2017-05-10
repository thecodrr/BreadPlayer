using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;
using BreadPlayer.Fmod;
using BreadPlayer.Events;
using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using static BreadPlayer.Fmod.Callbacks;

namespace BreadPlayer.Core.PlayerEngines
{
    public class FMODPlayerEngine : ObservableObject, IPlayerEngine
    {
        #region Fields
        FMODSystem FMODSys;
        Sound FMODSound;
        Channel FMODChannel;
        private CHANNEL_CALLBACK channelEndCallback;
        IntPtr EndSyncPoint;
        IntPtr Last5SyncPoint;
        IntPtr Last15SyncPoint;
        uint Last15Offset;
        bool isMobile;
        #endregion

        public FMODPlayerEngine(bool IsMobile)
        {
            isMobile = IsMobile;
            Init(IsMobile);
        }

        #region Methods
        public async Task Init(bool isMobile)
        {
            await Task.Run(() =>
            {
                Factory.SystemCreate(out FMODSys);
                FMODSys.Init(1, InitFlags.NORMAL, IntPtr.Zero);
                channelEndCallback = new CHANNEL_CALLBACK(ChannelEndCallback);
            });
        }
        public async Task<bool> Load(Mediafile mediaFile)
        {
            if (mediaFile != null && mediaFile.Length != "00:00")
            {
                //tell all listeners that we are about to change media
                await InitializeCore.Dispatcher.RunAsync(() => { MediaChanging?.Invoke(this, new EventArgs()); });

                //stop currently playing track and free the channel
                await Stop();

                //create a stream of the new track
                Result loadResult = FMODSys.CreateStream(mediaFile.Path, isMobile ? Mode.LOWMEM & Mode.IGNORETAGS : Mode.DEFAULT, out FMODSound);

                //load the stream into the channel but don't play it yet.
                loadResult = FMODSys.PlaySound(FMODSound, null, true, out FMODChannel);

                //FMODSys.CreateDSPByType(Fmod.CoreDSP.DspType.NORMALIZE, out DSP dsp);

                //FMODChannel.addDSP(ChannelControlDspIndex.HEAD, dsp);

                //dsp.setParameterFloat((int)Fmod.CoreDSP.DspNormalize.THRESHHOLD, 1.0f);
                //dsp.setParameterFloat((int)Fmod.CoreDSP.DspNormalize.MAXAMP, 2.0f);

                //dsp.setActive(true);

                //load equalizer
                if(Equalizer == null)
                    Equalizer = new FmodEqualizer(FMODSys, FMODChannel);
                else
                    (Equalizer as FmodEqualizer).ReInit(FMODSys, FMODChannel); 
                
                //get and update length of the track.
                Length = TimeSpan.FromMilliseconds(FMODSound.LengthInMilliseconds).TotalSeconds;

                //set the channel callback for all the syncpoints
                loadResult = FMODChannel.setCallback(channelEndCallback);

                //add all the sync points
                //1. when song ends
                loadResult = FMODSound.addSyncPoint(FMODSound.LengthInMilliseconds, TimeUnit.MS, "songended", out EndSyncPoint);

                //2. when song has reached the last 15 seconds
                loadResult = FMODSound.addSyncPoint(FMODSound.LengthInMilliseconds - 15000, TimeUnit.MS, "songabouttoended", out Last15SyncPoint);

                //3. when song has reached the last 5 seconds
                loadResult = FMODSound.addSyncPoint(FMODSound.LengthInMilliseconds - 5000, TimeUnit.MS, "fade", out Last5SyncPoint);

                //update the system once here so that 
                //all the sync points and callbacks are saved and updated.
                loadResult = FMODSys.Update();

                PlayerState = PlayerState.Stopped;
                CurrentlyPlayingFile = mediaFile;

                //check if all was successful
                return loadResult == Result.OK;
            }
            else
            {
                string error = "The file " + mediaFile.OrginalFilename + " is either corrupt, incomplete or unavailable. \r\n\r\n Exception details: No data available.";
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
                FMODChannel.SetFadePoint(FMODChannel.Volume, 0f, FMODSound.ConvertSecondsToPCM(0.5));

                //wait for the fade to over.
                await Task.Delay(500);

                //set paused to true
                FMODChannel.SetPaused(true);
            });
        }

        public Task Play()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Playing));
            return Task.Run(() =>
            {
                //set paused to false
                FMODChannel.SetPaused(false);

                //update volume.
                Volume = Volume;

                //set fade points to first 3 seconds of the track.
                //we simply slide the volume from 0 to the default value
                //in the next 1 second.
                FMODChannel.SetFadePoint(0f, FMODChannel.Volume, FMODSound.ConvertSecondsToPCM(1));

                PlayerState = PlayerState.Playing;
            });
        }

        public Task Stop()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
            return Task.Run(() =>
            {
                FMODChannel?.Stop();
                FMODSound?.release();
                Length = 0;
                Position = -1;
                CurrentlyPlayingFile = null;
                PlayerState = PlayerState.Stopped;
            });
        }
        #endregion

        #region Callbacks
        private Result ChannelEndCallback(IntPtr channelraw, ChannelControlType controltype, ChannelControlCallbackType type, IntPtr commanddata1, IntPtr commanddata2)
        {
            if (type == ChannelControlCallbackType.SYNCPOINT)
            {
                FMODSound?.getSyncPointInfo(Last15SyncPoint, new StringBuilder("songabouttoend"), 0, out Last15Offset, TimeUnit.MS);
                uint Last5Offset = 0;
                FMODSound?.getSyncPointInfo(Last5SyncPoint, new StringBuilder("fade"), 0, out Last5Offset, TimeUnit.MS);

                if (position == FMODSound?.LengthInMilliseconds)
                {
                    MediaEnded?.Invoke(this, new MediaEndedEventArgs(PlayerState.Ended));
                }
                else if (position >= Last5Offset)
                {
                    FMODChannel.SetFadePoint(FMODChannel.Volume, 0f, FMODChannel.GetTotalSamplesLeft(FMODSound));
                }
                else if (position >= Last15Offset && position < Last5Offset)
                {
                    MediaAboutToEnd?.Invoke(this, new Events.MediaAboutToEndEventArgs(CurrentlyPlayingFile));
                }
            }
            return Result.OK;
        }
        #endregion

        #region Properties
        bool isVolumeMuted;
        public bool IsVolumeMuted
        {
            get { return isVolumeMuted; }
            set
            {
                Set(ref isVolumeMuted, value);
                FMODChannel.setMute(isVolumeMuted);
            }
        }
        public Effects Effect { get; set; }
        double _volume = 50;
        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);
                if (FMODChannel != null)
                    FMODChannel.Volume = (float)(_volume / 100);
            }
        }
        double _seek = 0;
        uint position = 0;
        public double Position
        {
            get
            {

                FMODChannel?.getPosition(out position, TimeUnit.MS);
                FMODSys?.Update();
                return TimeSpan.FromMilliseconds(position).TotalSeconds;
            }
            set
            {
                Set(ref _seek, value);
                FMODChannel?.setPosition(Convert.ToUInt32(TimeSpan.FromSeconds(value < 0 ? 0 : value).TotalMilliseconds), TimeUnit.MS);
            }
        }
        double _length;
        public double Length
        {
            get
            {
                if (_length <= 0)
                    _length = 1;
                return _length <= 0 ? 1 : _length;
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
        IEqualizer fmodEqualizer;
        public IEqualizer Equalizer
        {
            get { return fmodEqualizer; }
            set
            {
                Set(ref fmodEqualizer, value);
            }
        }
        #endregion
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
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
        #endregion

        public event OnMediaStateChanged MediaStateChanged;
        public event OnMediaEnded MediaEnded;
        public event OnMediaAboutToEnd MediaAboutToEnd;
        public event OnMediaChanging MediaChanging;
    }
}
