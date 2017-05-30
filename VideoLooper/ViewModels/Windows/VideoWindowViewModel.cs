using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VideoLooper.Models;
using VideoLooper.Utils;
using VideoLooper.ViewModels.Base;

namespace VideoLooper.ViewModels {

    /// <summary>
    /// The view model for the media player and controls
    /// </summary>
    public class VideoWindowViewModel : BaseViewModel, IControllable {

        #region Private Properties

        /// <summary>
        /// whether or not the program is looping
        /// </summary>
        private bool _isLooping = false;

        /// <summary>
        /// whether or not the videois fullscreen mode
        /// </summary>
        private bool _isFullScreenMode = false;

        /// <summary>
        /// the dispatcher thread to use for looping the video
        /// </summary>
        private DispatcherTimer _loopTimer = new DispatcherTimer();

        /// <summary>
        /// the start of the current loop time in seconds
        /// </summary>
        private int _currentLoopStartTime = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// the viewmodel for the media player + controls
        /// </summary>
        public MediaPlayerViewModel Player { get; set; }

        /// <summary>
        /// the viewmodel for the playlist + controls
        /// </summary>
        public PlaylistViewModel Playlist { get; set; }

        /// <summary>
        /// whether or not the video is currently looping
        /// </summary>
        public bool IsLooping {
            get {
                return _isLooping;
            }
            set {
                _isLooping = value;

                //disable/enable buttons appropriately
                StartLoopingCommand.RaiseCanExecuteChanged();
                StopLoopingCommand.RaiseCanExecuteChanged();

                //disable/enable looping timer
                _loopTimer.IsEnabled = IsLooping;

                //change text color of position
                Player.CurrentPositionTextColor = IsLooping ? "Orange" : "Black";
            }
        }

        /// <summary>
        /// the height of the title bar
        /// </summary>
        public String TitleBarHeight { get; set; } = TITLE_BAR_HEIGHT;

        /// <summary>
        /// the margin of the outer border for the video window
        /// </summary>
        public String OuterBorderMargin { get; set; } = "5";

        /// <summary>
        /// the width of the playlist panel
        /// </summary>
        public String PlaylistColumnWidth { get; set; } = PLAYLIST_COLUMN_WIDTH;

        /// <summary>
        /// the height of the bottom row of controls
        /// </summary>
        public String ControlRowHeight { get; set; } = CONTROL_ROW_HEIGHT;

        /// <summary>
        /// the current state of the video window
        /// </summary>
        public WindowState VideoWindowState { get; set; } = WindowState.Normal;


        #endregion

        #region Public Commands

        #region Playlist Commands

        /// <summary>
        /// command to play the next video in the playlist
        /// </summary>
        public RelayCommand NextCommand { get; set; }

        /// <summary>
        /// command to get the previous video in the playlist
        /// </summary>
        public RelayCommand PrevCommand { get; set; }

        /// <summary>
        /// command to get the shuffle the current playlist
        /// </summary>
        public RelayCommand ShuffleCommand { get; set; }

        /// <summary>
        /// command to toggle the auto-play mode
        /// </summary>
        public RelayCommand TogglePlaybackModeCommand { get; set; }

        #endregion

        #region Looping Commands

        /// <summary>
        /// start looping the video
        /// </summary>
        public DelegateCommand StartLoopingCommand { get; set; }

        /// <summary>
        /// stop looping the video
        /// </summary>
        public DelegateCommand StopLoopingCommand { get; set; }

        /// <summary>
        /// set begin time for the loop
        /// </summary>
        public RelayCommand SetLoopBeginCommand { get; set; }

        /// <summary>
        /// set end time for the loop
        /// </summary>
        public RelayCommand SetLoopEndCommand { get; set; }



        #endregion

        #region Toggle UI Commands

        /// <summary>
        /// toggle fullscreen mode
        /// </summary>
        public RelayCommand ToggleFullScreenCommand { get; set; }

        /// <summary>
        /// show/hide playlist panel
        /// </summary>
        public RelayCommand TogglePlaylistCommand { get; set; }

        /// <summary>
        /// show/hide bottom control row
        /// </summary>
        public RelayCommand ToggleControlRowCommand { get; set; }

        #endregion

        #endregion

        #region Constructor

        #region Default Constructor

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="mediaElement">the embedded mediaplayer from the view</param>
        public VideoWindowViewModel(MediaElement mediaElement, PlaylistViewModel playlist) {

            
            InitializeMediaPlayer(mediaElement);
            Playlist = playlist;

            InitializeCommands();
            InitializeLoopingTimer();

            //set the video's source and play the video
            Player.CurrentSource = Playlist.SelectedItem.FullPath;
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// create instances for all of the ICommands
        /// </summary>
        private void InitializeCommands() {
            

            //commands to control playlist features
            NextCommand = new RelayCommand(Next);
            PrevCommand = new RelayCommand(Prev);
            ShuffleCommand = new RelayCommand(Playlist.Shuffle);
            TogglePlaybackModeCommand = new RelayCommand(TogglePlaybackMode);

            //commands to set loop beginning and ending times
            SetLoopBeginCommand = new RelayCommand(Player.SetBeginTextBox);
            SetLoopEndCommand = new RelayCommand(Player.SetEndTextBox);

            //looping buttons delegate commands for enabling/disabling
            StartLoopingCommand = new DelegateCommand(InitiateLoopingTask, (param) => !IsLooping);
            StopLoopingCommand = new DelegateCommand(StopLoop, (param) => IsLooping);

            //show/hide components of the UI by changing their size
            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreenMode);
            TogglePlaylistCommand = new RelayCommand(TogglePlaylistVisibilty);
            ToggleControlRowCommand = new RelayCommand(ToggleControlRowVisibility);

        }

        /// <summary>
        /// Initialize the video player viewmodel
        /// </summary>
        /// <param name="mediaElement"></param>
        private void InitializeMediaPlayer(MediaElement mediaElement) {

            //passed in from the view for now, will come back to explore MVVM options
            Player = new MediaPlayerViewModel(mediaElement);

            //autoplay next in playlist on end of media
            Player.MediaPlayer.MediaEnded += (sender, e) => {

                if (Playlist.PlaybackMode == PlaybackMode.On) {
                    Next();
                } else if (Playlist.PlaybackMode == PlaybackMode.Repeat) {
                    Player.Stop();
                    Player.TogglePlayPause();
                } else {
                    Player.Stop();
                }
            };
        }

        /// <summary>
        /// initialize the looping dispatcher
        /// </summary>
        private void InitializeLoopingTimer() {
            _loopTimer = new DispatcherTimer();
            _loopTimer.IsEnabled = false;
            _loopTimer.Tick += (sender, e) => {
                Loop();
            };
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// play the selected item in the playlist
        /// </summary>
        public void ListBoxItemDoubleClicked() {
            Player.CurrentSource = Playlist.SelectedItem.FullPath;
            Playlist.CurrentIndex = Playlist.Items.IndexOf(Playlist.SelectedItem);
        }

        /// <summary>
        /// play the media if its not playing
        /// </summary>
        public void StrictPlay() {
            if (Player.IsPlaying == false) {
                Player.TogglePlayPause();
            }
        }

        /// <summary>
        /// pause the media if its playing
        /// </summary>
        public void StrictPause() {
            if (Player.IsPlaying == true) {
                Player.TogglePlayPause();
            }
        }

        /// <summary>
        /// stop the media
        /// </summary>
        public void Stop() {
            Player.Stop();
        }

        /// <summary>
        /// plays the next item in playlist
        /// </summary>
        public void Next() {
            if (Playlist.Items.Count == 1 || Playlist.PlaybackMode == PlaybackMode.Repeat) {
                Player.Stop();
                Player.TogglePlayPause();
            } else {
                Player.CurrentSource = Playlist.GetNext().FullPath;
            }
        }

        /// <summary>
        /// play the previous item in the playlist
        /// </summary>
        public void Prev() {
            if (Playlist.Items.Count == 1 || Playlist.PlaybackMode == PlaybackMode.Repeat) {
                Player.Stop();
                Player.TogglePlayPause();
            } else {
                Player.CurrentSource = Playlist.GetPrev().FullPath;
            }
        }

        /// <summary>
        /// jumps to the user-specified timestamp
        /// </summary>
        public void JumpTo() {
            Player.JumpTo();
        }

        #endregion

        #region Private Methods

        private void TogglePlaybackMode() {
            if (Playlist.PlaybackMode == PlaybackMode.On) {
                Playlist.PlaybackMode = PlaybackMode.Repeat;
            } else if (Playlist.PlaybackMode == PlaybackMode.Repeat) {
                Playlist.PlaybackMode = PlaybackMode.Off;
            } else if (Playlist.PlaybackMode == PlaybackMode.Off) {
                Playlist.PlaybackMode = PlaybackMode.On;
            }
        }

        /// <summary>
        /// prepare for loop and enable the looping timer
        /// </summary>
        public void InitiateLoopingTask() {

            //convert timestamp to seconds
            int start = VideoUtils.ConvertTimestampToSeconds(Player.BeginText);
            int end = VideoUtils.ConvertTimestampToSeconds(Player.EndText);

            //bail out if the timestamps are invalid
            if (start < 0 || start > Player.TotalDuration ||
                end < 0 || end > Player.TotalDuration ||
                start >= end) {
                return;
            }

            //calculate duration, adding once second for padding
            int duration = end - start + 1;

            //store start position in case user changes start time during loop
            _currentLoopStartTime = start;

            //notify looping has started
            IsLooping = true;

            //move current position to start position
            Player.MediaPlayer.Position = TimeSpan.FromSeconds(_currentLoopStartTime);

            //****LOOP STARTED HERE****
            //set duration of loop
            //activate looping dispatcher timer
            _loopTimer.Interval = TimeSpan.FromSeconds(duration);
            _loopTimer.IsEnabled = true;
        }

        /// <summary>
        /// stop the loop
        /// </summary>
        public void StopLoop() {
            IsLooping = false;
        }

        /// <summary>
        /// loops the video by setting the position back to the start
        /// </summary>
        /// <param name="start"></param>
        public void Loop() {

            //bail out if user has canceled
            //****NOTE***** 
            //need to come back and add checks to see if video is still playing
            //also could check if video's position is outside of loop
            if (Player.IsPlaying == false) {
                IsLooping = false;
            }

            if (IsLooping == true) {
                Player.MediaPlayer.Position = TimeSpan.FromSeconds(_currentLoopStartTime);
            }
        }

        /// <summary>
        /// Toggle fullscreen mode
        /// </summary>
        private void ToggleFullScreenMode() {
            if (_isFullScreenMode == false) {
                _isFullScreenMode = true;
                TitleBarHeight = "0";
                PlaylistColumnWidth = "0";
                ControlRowHeight = "0";
                OuterBorderMargin = "0";
                VideoWindowState = WindowState.Maximized;
            } else {

                //restore window state
                //**TODO: come back and restore all properities of window, currently does not do this
                _isFullScreenMode = false;
                OuterBorderMargin = "5";
                TitleBarHeight = TITLE_BAR_HEIGHT;
                VideoWindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// show/hide playlist by changing width of grid column
        /// </summary>
        private void TogglePlaylistVisibilty() {
            if (PlaylistColumnWidth == "0") {
                PlaylistColumnWidth = PLAYLIST_COLUMN_WIDTH;
            } else {
                PlaylistColumnWidth = "0";
            }
        }

        /// <summary>
        /// show/hide the bottom control row
        /// </summary>
        private void ToggleControlRowVisibility() {
            if (ControlRowHeight == "0") {
                ControlRowHeight = CONTROL_ROW_HEIGHT;
            } else {
                ControlRowHeight = "0";
            }
        }

        #endregion

        #region Constants

        /// <summary>
        /// the height of the title bar
        /// </summary>
        private const String TITLE_BAR_HEIGHT = "20";

        /// <summary>
        /// the height of the lower control row
        /// </summary>
        private const String CONTROL_ROW_HEIGHT = "60";

        /// <summary>
        /// the height of the loop control row
        /// </summary>
        private const String LOOP_CONTROL_ROW_HEIGHT = "30";

        /// <summary>
        /// the width of the playlist panel column
        /// </summary>
        private const String PLAYLIST_COLUMN_WIDTH = "*";

        #endregion

    }
}
