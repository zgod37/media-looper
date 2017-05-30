using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using VideoLooper.Utils;
using VideoLooper.ViewModels.Base;

namespace VideoLooper.Models {

    /// <summary>
    /// object model for playlist
    /// </summary>
    public class PlaylistViewModel : BaseViewModel {

        #region Private Properties

        /// <summary>
        /// the image for continous playback mode
        /// </summary>
        private BitmapImage _continuousImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/continuous_playback_button.png"));

        /// <summary>
        /// the image for continous playback mode
        /// </summary>
        private BitmapImage _repeatImage = new BitmapImage(new Uri(@"pack://application:,,,/Images/repeat_playback_button.png"));

        /// <summary>
        /// the playback mode of the playlist [ON/OFF/REPEAT]
        /// </summary>
        private PlaybackMode _playbackMode;

        #endregion

        #region Public Properties

        /// <summary>
        /// the list of files in the playlist
        /// </summary>
        public ObservableCollection<PlaylistItem> Items { get; set; }
        
        /// <summary>
        /// the index of the item that is currently loaded
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// the item that is currently selected in the list box
        /// </summary>
        public PlaylistItem SelectedItem { get; set; }

        /// <summary>
        /// whether or not the playlist is auto playing
        /// </summary>
        public PlaybackMode PlaybackMode {
            get {
                return _playbackMode;
            }
            set {
                _playbackMode = value;

                switch (_playbackMode) {
                    case PlaybackMode.On:
                        AutoplayButtonImage = _continuousImage;
                        break;
                    case PlaybackMode.Repeat:
                        AutoplayButtonImage = _repeatImage;
                        break;
                    case PlaybackMode.Off:
                        AutoplayButtonImage = null;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// the background color of the auto play button
        /// using color for now, need to come back and switch the icon
        /// </summary>
        public BitmapImage AutoplayButtonImage { get; set; }

        #endregion

        #region Construtor

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="filePaths">the list of files in the playlist</param>
        public PlaylistViewModel(String[] filePaths) {
            Items = new ObservableCollection<PlaylistItem>(filePaths.Select(path => new PlaylistItem(path)));
            SelectedItem = Items[0];

            //start playlist in continuous playback mode by default;
            PlaybackMode = PlaybackMode.On;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// add files to the current playlist
        /// </summary>
        /// <param name="filePaths">the list of files to add</param>
        public void Add(String[] filePaths) {

            //creating new list as janky work-around to notify property change
            foreach (String path in filePaths) {
                Items.Add(new PlaylistItem(path));
            }
        }

        /// <summary>
        /// shuffle the playlist
        /// </summary>
        public void Shuffle() {
            
            //save current item
            var currentItem = SelectedItem;

            //make copy of items and shuffle to notify property changed
            VideoUtils.shuffleInPlace(Items);

            //update the current info
            CurrentIndex = Items.IndexOf(currentItem);
            SelectedItem = Items[CurrentIndex];
        }

        /// <summary>
        /// return the next item in the playlist
        /// </summary>
        public PlaylistItem GetNext() {
            if (CurrentIndex >= Items.Count-1) {
                CurrentIndex = 0;
            } else {
                CurrentIndex++;
            }
            SelectedItem = Items[CurrentIndex];
            return SelectedItem;
        }

        /// <summary>
        /// return the previous item in the playlist
        /// </summary>
        public PlaylistItem GetPrev() {
            if (CurrentIndex <= 0) {
                CurrentIndex = Items.Count - 1;
            } else {
                CurrentIndex--;
            }
            SelectedItem = Items[CurrentIndex];
            return SelectedItem;
        }

        #endregion

    }
}
