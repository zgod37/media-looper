using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoLooper.Models;

namespace VideoLooper {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IRemote {

        private List<IControllable> _children = new List<IControllable>();

        public MainWindow() {
            System.Diagnostics.Debug.AutoFlush = true;
            InitializeComponent();
        }

        public void Register(IControllable child) {
            _children.Add(child);
        }

        public void Unregister(IControllable child) {
            _children.Remove(child);
        }

        /// <summary>
        /// open a new video window with the files specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_New_Button_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            var result = dialog.ShowDialog();
            
            if (result == true) {
                Window videoWindow = new VideoWindow(this, new PlaylistViewModel(dialog.FileNames));
                videoWindow.Show();
            }
        }

        private void Play_All_Button_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Debug.WriteLine($"number of children = {_children.Count}");
            _children.ForEach(child => child.StrictPlay());
        }

        private void Pause_All_Button_Click(object sender, RoutedEventArgs e) {
            _children.ForEach(child => child.StrictPause());
        }

        private void Prev_All_Button_Click(object sender, RoutedEventArgs e) {
            _children.ForEach(child => child.Prev());
        }

        private void Next_All_Button_Click(object sender, RoutedEventArgs e) {
            _children.ForEach(child => child.Next());
        }

        private void Stop_All_Button_Click(object sender, RoutedEventArgs e) {
            _children.ForEach(child => child.Stop());
        }

        private void Jump_All_Button_Click(object sender, RoutedEventArgs e) {
            _children.ForEach(child => child.JumpTo());
        }
    }
}
