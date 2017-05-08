using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadPlayer.Models;
using FMOD;
using BreadPlayer.Events;

namespace BreadPlayer.Core.PlayerEngines
{
    public class FMODPlayerEngine : IPlayerEngine
    {
        #region Fields
        FMOD.System FMODSys;
        Sound FMODSound;
        Channel FMODChannel;
        #endregion
        public bool IsVolumeMuted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Effects Effect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Length { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PlayerState PlayerState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Mediafile CurrentlyPlayingFile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IgnoreErrors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event OnMediaStateChanged MediaStateChanged;
        public event OnMediaEnded MediaEnded;
        public event OnMediaAboutToEnd MediaAboutToEnd;
        public event OnMediaChanging MediaChanging;

        public async Task Init(bool isMobile)
        {
            await Task.Run(() =>
            {
                Factory.System_Create(out FMODSys);
                FMODSys.init(1, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            });
        }

        public async Task<bool> Load(Mediafile mediaFile)
        {
            if (mediaFile != null && mediaFile.Length != "00:00")
            {
                await InitializeCore.Dispatcher.RunAsync(() => { MediaChanging?.Invoke(this, new EventArgs()); });
                return await Task.Run(() =>
                {
                    RESULT loadResult = FMODSys.CreateStream(mediaFile.Path, FMOD.MODE.DEFAULT, out FMOD.Sound FMODSound);

                    return loadResult == RESULT.OK;
                });
            }
            else
                return false;
        }

        public Task Pause()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Paused));
            return Task.Run(() =>
            {
                FMODChannel.SetPaused(true);
                PlayerState = PlayerState.Paused;
            });
        }

        public Task Play()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Playing));
            return Task.Run(() =>
            {
                FMODSys.PlaySound(FMODSound, null, false, out FMODChannel);
                PlayerState = PlayerState.Playing;
            });
        }

        public Task Stop()
        {
            MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(PlayerState.Stopped));
            return Task.Run(() =>
            {
                FMODChannel.Stop();
                Length = 0;
                Position = -1;
                CurrentlyPlayingFile = null;
                FMODSound = null;
                FMODChannel = null;
                PlayerState = PlayerState.Stopped;
            });
        }

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
    }
}
