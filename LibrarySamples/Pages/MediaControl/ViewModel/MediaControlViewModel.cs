﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LibrarySamples.Command;
using LibrarySamples.ViewModel;
using Microsoft.Win32;

namespace LibrarySamples.Pages.MediaControl.ViewModel
{
    public class MediaControlViewModel : BaseViewModel
    {
        private string _filename;
        private TimeSpan _playTime;
        private bool _isFullScreenEnabled;
        private bool _isEjectEnabled;
        private bool _canPlay;
        private TimeSpan _rewindSpeed;
        private CancellationTokenSource _source;

        public MediaControlViewModel()
        {
            InitCommands();
            FileName = "No Video";
            IsFullScreenEnabled = true;
            IsEjectEnabled = true;
            CanPlay = true;
            RewindSpeed = new TimeSpan(0,0,0,0);
        }

        /// <summary>
        /// function used to determine the speed the rewind speed needs to be at
        /// </summary>
        public void IncreaseRewindTime()
        {
            // don't increase above 12 seconds
            if (RewindSpeed.Seconds <= 12)
                RewindSpeed = RewindSpeed.Add(RewindSpeed);

            // first press of rewind so set it to 1
            if (RewindSpeed.Seconds == 0)
                RewindSpeed = new TimeSpan(0, 0, 0, 1);
        }

        public void CancelRewind()
        {
            _source?.Cancel();

            RewindSpeed = new TimeSpan(0, 0, 0, 0);
        }

        // Event handlers to be fired by the view model to update the media element
        public event EventHandler StopRequested;
        public event EventHandler RewindRequested;
        public event EventHandler PlayRequested;
        public event EventHandler PauseRequested;
        public event EventHandler FastForwardRequested;
        public event EventHandler EjectRequested;
        public event EventHandler FullScreenRequested;
        public event EventHandler UpdateTime;

        public string FileName
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan PlayTime
        {
            get { return _playTime; }
            set
            {
                _playTime = value;
                OnPropertyChanged();
            }
        }

        public bool IsFullScreenEnabled
        {
            get { return _isFullScreenEnabled; }
            set
            {
                _isFullScreenEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsEjectEnabled
        {
            get { return _isEjectEnabled; }
            set
            {
                _isEjectEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool CanPlay
        {
            get { return _canPlay; }
            set
            {
                _canPlay = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan RewindSpeed
        {
            get {  return _rewindSpeed; }
            set { _rewindSpeed = value; }
        }

        public CancellationTokenSource Source
        {
            get { return _source; }
            set { _source = value; }
        }

        #region commands
        public void InitCommands()
        {
            StopCommand = new DelegateCommand(ExecuteStopCommand, CanExecuteStopCommand);
            RewindCommand = new DelegateCommand(ExecuteRewindCommand, CanExecuteRewindCommand);
            PlayCommand = new DelegateCommand(ExecutePlayCommand, CanExecutePlayCommand);
            PauseCommand = new DelegateCommand(ExecutePauseCommand, CanExecutePauseCommand);
            FastForwardCommand = new DelegateCommand(ExecuteFastForwardCommand, CanExecuteFastforwardCommand);
            EjectCommand = new DelegateCommand(ExecuteEjectCommand, CanExecuteEjectCommand);
            FullScreenCommand = new DelegateCommand(ExecuteFullScreenCommand, CanExecuteFullScreenCommand);
        }

        public ICommand StopCommand { get; private set; }
        public ICommand RewindCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand FastForwardCommand { get; private set; }
        public ICommand EjectCommand { get; private set; }
        public ICommand FullScreenCommand { get; private set; }

        public bool CanExecuteStopCommand(object parameter)
        {
            return true;
        }

        public void ExecuteStopCommand(object parameter)
        {
            CancelRewind();
            CanPlay = true;

            StopRequested?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecuteRewindCommand(object parameter)
        {
            return true;
        }

        public void ExecuteRewindCommand(object parameter)
        {
            CanPlay = true;

            _source?.Cancel();

            IncreaseRewindTime();

            _source = new CancellationTokenSource();

            RewindRequested?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecutePlayCommand(object parameter)
        {
            return true;
        }

        public void ExecutePlayCommand(object parameter)
        {
            CancelRewind();

            PlayRequested?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecutePauseCommand(object parameter)
        {
            return true;
        }

        public void ExecutePauseCommand(object parameter)
        {
            CancelRewind();

            CanPlay = true;

            PauseRequested?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecuteFastforwardCommand(object parameter)
        {
            return true;
        }

        public void ExecuteFastForwardCommand(object parameter)
        {
            CanPlay = true;

            CancelRewind();

            FastForwardRequested?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecuteEjectCommand(object parameter)
        {
            return true;
        }

        public void ExecuteEjectCommand(object parameter)
        {
            CanPlay = true;

            CancelRewind();

            EjectRequested?.Invoke(this, EventArgs.Empty);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".mp4";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                FileName = dlg.FileName;

                FireTimeEvent();
            }
        }

        private async void FireTimeEvent()
        {
            while (FileName != "No Video")
            {
                await Task.Delay(100);

                UpdateTime?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecuteFullScreenCommand(object parameter)
        {
            return true;
        }

        public void ExecuteFullScreenCommand(object parameter)
        {
            FullScreenRequested?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
