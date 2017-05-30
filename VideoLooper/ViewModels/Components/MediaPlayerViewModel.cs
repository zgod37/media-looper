using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VideoLooper.Models;
using VideoLooper.Utils;
using VideoLooper.ViewModels.Base;

namespace VideoLooper.ViewModels {

    /// <summary>
    /// view model for the media player and controls
    /// </summary>
    public class MediaPlayerViewModel : BaseViewModel {

        #region Private Properties

        /// <summary>
        /// whether or not the current video is playing
        /// </summary>
        private bool _isPlaying = false;

        /// <summary>
        /// the uri of the current source loaded in the player
        /// </summary>
        private String _currentSource;

        /// <summary>
        /// the current volume of the media player
        /// </summary>
        private double _currentVolume = 0.1;

        /// <summary>
        /// whether or not the user is changing the position of video via the slider
        /// </summary>
        private bool _userIsChangingPosition = false;

        /// <summary>
        /// the image shown on the play button while the video is not playing
        /// </summary>
        private static BitmapImage _playButtonImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/play_button.png"));

        /// <summary>
        /// the image shown on the play button while the video is playing
        /// </summary>
        private static BitmapImage _pauseButtonImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/pause_button.png"));

        /// <summary>
        /// the image shown on the play button while mouse is over and video is not playing
        /// </summary>
        private static BitmapImage _playButtonHoverImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/play_button_hover.png"));

        /// <summary>
        /// the image shown on the play button while mouse is over and video is playing
        /// </summary>
        private static BitmapImage _pauseButtonHoverImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/pause_button_hover.png"));

        #endregion

        #region Public Properties

        public MediaElement MediaPlayer { get; set; }

        /// <summary>
        /// whether or not the current video is playing
        /// </summary>
        public bool IsPlaying {
            get {
                return _isPlaying;
            }
            set {
                _isPlaying = value;

                //update UI appropriately
                if (_isPlaying == true) {
                    PlayButtonImage = _pauseButtonImage;
                    PlayButtonHoverImage = _pauseButtonHoverImage;
                } else {
                    PlayButtonImage = _playButtonImage;
                    PlayButtonHoverImage = _playButtonHoverImage;
                }
                CurrentPositionTextColor = _isPlaying ? "Black" : "DarkRed";
            }
        }

        /// <summary>
        /// the uri of the current video
        /// </summary>
        public String CurrentSource {
            get {
                return _currentSource;
            }
            set {
                _currentSource = value;
                IsPlaying = false;
                MediaPlayer.Source = new Uri(value);
                TogglePlayPause();
            }
        }

        /// <summary>
        /// the current stretch property of the video
        /// </summary>
        public Stretch Stretch { get; set; } = Stretch.Fill;

        /// <summary>
        /// the current shown image of the play/pause button
        /// </summary>
        public BitmapImage PlayButtonImage { get; set; }

        /// <summary>
        /// the hover image shown when mouse is over button
        /// </summary>
        public BitmapImage PlayButtonHoverImage { get; set; }

        /// <summary>
        /// the current position of the video as a string
        /// </summary>
        public String CurrentPositionText { get; set; } = "00:00:00";

        /// <summary>
        /// the current volume of the video
        /// </summary>
        public double CurrentVolume {
            get {
                return _currentVolume;
            }
            set {
                _currentVolume = value;
                MediaPlayer.Volume = value;
            }
        }

        /// <summary>
        /// the text color of the current position string
        /// </summary>
        public String CurrentPositionTextColor { get; set; } = "Black";

        /// <summary>
        /// the current position of the slider
        /// </summary>
        public double SliderPosition { get; set; }

        /// <summary>
        /// the total time of the currently loaded video
        /// </summary>
        public double TotalDuration { get; set; }

        /// <summary>
        /// the text for the beginning time of the loop
        /// </summary>
        public String BeginText { get; set; } = "00:00:00";

        /// <summary>
        /// the text for the ending time of the loop
        /// </summary>
        public String EndText { get; set; } = "00:00:00";

        /// <summary>
        /// the text for the ending time of the loop
        /// </summary>
        public String JumpToText { get; set; } = "00:00:00";

        #endregion

        #region Playback Commands

        /// <summary>
        /// command to start/resume playback
        /// </summary>
        public RelayCommand PlayCommand { get; set; }

        /// <summary>
        /// command to pause the video
        /// </summary>
        public DelegateCommand PauseCommand { get; set; }

        /// <summary>
        /// command to stop the video
        /// </summary>
        public RelayCommand StopCommand { get; set; }

        /// <summary>
        /// command to move the current position back a static amount
        /// </summary>
        public RelayCommand JumpBackCommand { get; set; }

        /// <summary>
        /// command to move the current position ahead a static amount
        /// </summary>
        public RelayCommand JumpAheadCommand { get; set; }

        /// <summary>
        /// set jump to text box for jump command
        /// </summary>
        public RelayCommand SetJumpToCommand { get; set; }

        /// <summary>
        /// jump to desired position in video
        /// </summary>
        public RelayCommand JumpToCommand { get; set; }

        /// <summary>
        /// command to toggle the aspect ratio stretch of the video
        /// </summary>
        public RelayCommand ToggleStretchCommand { get; set; }

        /// <summary>
        /// command to increase the volume
        /// </summary>
        public RelayCommand IncreaseVolumeCommand { get; set; }

        /// <summary>
        /// command to decrease the voume
        /// </summary>
        public RelayCommand DecreaseVolumeCommand { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="mediaElement">the media element passed in from the view</param>
        public MediaPlayerViewModel(MediaElement mediaElement) {

            //initialize the media player
            MediaPlayer = mediaElement;
            MediaPlayer.Volume = _currentVolume;

            //start the dispatcher timer to update current position
            StartCurrentPositionDispatcher();

            //initialize Icommands
            InitializeCommands();

        }


        /// <summary>
        /// start dispatcher timer to keep track of update the display of the current position
        /// </summary>
        private void StartCurrentPositionDispatcher() {
            DispatcherTimer progressTimer = new DispatcherTimer();
            progressTimer.Interval = TimeSpan.FromMilliseconds(1000);
            progressTimer.Tick += (sender, e) => {
                UpdateSliderPosition();
                UpdateCurrentPositionText();
            };
            progressTimer.Start();
        }

        private void InitializeCommands() {
            //commands to control media playback
            PlayCommand = new RelayCommand(TogglePlayPause);
            StopCommand = new RelayCommand(Stop);
            SetJumpToCommand = new RelayCommand(SetJumpToTextBox);
            JumpToCommand = new RelayCommand(JumpTo);
            JumpBackCommand = new RelayCommand(JumpBack);
            JumpAheadCommand = new RelayCommand(JumpAhead);
            ToggleStretchCommand = new RelayCommand(ToggleStretch);
            IncreaseVolumeCommand = new RelayCommand(IncreaseVolume);
            DecreaseVolumeCommand = new RelayCommand(DecreaseVolume);
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// play/pause/resume the currently loaded video
        /// </summary>
        public void TogglePlayPause() {
            if (MediaPlayer != null && CurrentSource != null) {
                if (IsPlaying == false) {
                    MediaPlayer.Play();
                    IsPlaying = true;
                } else {
                    MediaPlayer.Pause();
                    IsPlaying = false;
                }
            }
        }

        /// <summary>
        /// stops the currently loaded video
        /// </summary>
        public void Stop() {
            MediaPlayer.Stop();
            IsPlaying = false;
        }

        /// <summary>
        /// move the current position of the video back a static amount
        /// </summary>
        public void JumpBack() {
            //use constant of 10 seconds for now
            if (IsPlaying == true) {
                MediaPlayer.Position -= TimeSpan.FromSeconds(DEFAULT_JUMP_SECONDS);
            }
        }

        /// <summary>
        /// move the current position of the video back a static amount
        /// </summary>
        public void JumpAhead() {
            //use constant of 10 seconds for now
            if (IsPlaying == true) {
                MediaPlayer.Position += TimeSpan.FromSeconds(DEFAULT_JUMP_SECONDS);
            }
        }

        /// <summary>
        /// jump to the position stored in the jump text box
        /// </summary>
        public void JumpTo() {
            int position = VideoUtils.ConvertTimestampToSeconds(JumpToText);
            if (MediaPlayer.Source != null && position >= 0 && position < TotalDuration) {
                MediaPlayer.Position = TimeSpan.FromSeconds(position);
            }
        }

        /// <summary>
        /// toggle the stretch/aspect ratio of the video
        /// </summary>
        public void ToggleStretch() {
            if (Stretch == Stretch.Fill) {
                Stretch = Stretch.Uniform;
            } else if (Stretch == Stretch.Uniform) {
                Stretch = Stretch.UniformToFill;
            } else if (Stretch == Stretch.UniformToFill) {
                Stretch = Stretch.None;
            } else if (Stretch == Stretch.None) {
                Stretch = Stretch.Fill;
            }
        }

        /// <summary>
        /// increase the volume
        /// </summary>
        public void IncreaseVolume() {
            CurrentVolume += 0.05;
        }

        /// <summary>
        /// decrease the volume
        /// </summary>
        public void DecreaseVolume() {
            CurrentVolume -= 0.05;
        }

        /// <summary>
        /// update the slider's position
        /// </summary>
        public void UpdateSliderPosition() {
            if ((MediaPlayer.Source != null) && (MediaPlayer.NaturalDuration.HasTimeSpan) && (_userIsChangingPosition == false)) {
                TotalDuration = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SliderPosition = MediaPlayer.Position.TotalSeconds;
            }
        }

        /// <summary>
        /// fired when user starts dragging the slider
        /// </summary>
        public void SliderDragStarted() {
            _userIsChangingPosition = true;
        }

        /// <summary>
        /// fired when user finishes dragging the slider
        /// </summary>
        public void SliderDragCompleted() {
            _userIsChangingPosition = false;
            MediaPlayer.Position = TimeSpan.FromSeconds(SliderPosition);
        }

        /// <summary>
        /// fired when user clicks a position on the slider
        /// </summary>
        public void SliderPositionClicked() {
            MediaPlayer.Position = TimeSpan.FromSeconds(SliderPosition);
        }

        /// <summary>
        /// update the text representation of the current position
        /// </summary>
        public void UpdateCurrentPositionText() {
            CurrentPositionText = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// set the begin text box to the current position of the video
        /// </summary>
        public void SetBeginTextBox() {
            if (MediaPlayer.Source != null) {
                BeginText = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// set the begin text box to the current position of the video
        /// </summary>
        public void SetEndTextBox() {
            if (MediaPlayer.Source != null) {
                EndText = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// set the begin text box to the current position of the video
        /// </summary>
        public void SetJumpToTextBox() {
            if (MediaPlayer.Source != null) {
                JumpToText = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
            }
        }


        #endregion

        #region Constants

        /// <summary>
        /// the number of seconds used for jump back/jump ahead
        /// </summary>
        private const int DEFAULT_JUMP_SECONDS = 7;

        #endregion
    }
}
