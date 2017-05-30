using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using VideoLooper.Models;
using VideoLooper.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace VideoLooper {
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window {

        /// <summary>
        /// a reference to the remote for registering as observer
        /// </summary>
        private IRemote _remote;

        public VideoWindow(IRemote remote, PlaylistViewModel playlist) {
            InitializeComponent();

            this._remote = remote;
            this.DataContext = new VideoWindowViewModel(mediaElement, playlist);

            
            _remote.Register(DataContext as IControllable);
        }

        #region Event Handlers

        protected override void OnClosing(CancelEventArgs e) {
            _remote.Unregister(DataContext as IControllable);
            base.OnClosing(e);
        }
        
        private void MoveWindow(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && WindowState != WindowState.Maximized) {
                DragMove();
            }
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            var result = dialog.ShowDialog();

            if (result == true) {
                VideoWindowViewModel vm = (VideoWindowViewModel)DataContext;
                vm.Playlist.Add(dialog.FileNames);
            }
        }

        private void Slider_DragStarted(object sender, RoutedEventArgs e) {
            VideoWindowViewModel vm = (VideoWindowViewModel)DataContext;
            vm.Player.SliderDragStarted();
        }

        private void Slider_DragCompleted(object sender, RoutedEventArgs e) {
            VideoWindowViewModel vm = (VideoWindowViewModel)DataContext;
            vm.Player.SliderDragCompleted();
        }

        private void Slider_MouseLeftButtonUp(object sender, RoutedEventArgs e) {
            VideoWindowViewModel vm = (VideoWindowViewModel)DataContext;
            vm.Player.SliderPositionClicked();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            VideoWindowViewModel vm = (VideoWindowViewModel)DataContext;
            vm.ListBoxItemDoubleClicked();
        }

        #endregion

    }
}
