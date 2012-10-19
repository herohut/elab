using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwfPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            context = new MainViewModel();
            context.LoadXml();
            context.PropertyChanged += context_PropertyChanged;
            this.DataContext = context;
        }


        private MainViewModel context;
        void context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainViewModel.SelectedFilePropertyName)
            {
                player.Movie = context.SelectedFile.FilePath;
                if (!File.Exists(context.SelectedFile.FilePath))
                    return;

                context.IsPlaying = true;
                context.SliderMaximum = player.TotalFrames;
                player.OnProgress += (s, a) => context.SliderValue = player.FrameNum;
            }
            else if(e.PropertyName == MainViewModel.SliderValuePropertyName)
            {
                player.GotoFrame((int)context.SliderValue);
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            player = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = player;
            player.BGColor = "000000";

        }

        private AxShockwaveFlashObjects.AxShockwaveFlash player;

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            player.StopPlay();
            context.IsPlaying = false;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            player.Play();
            context.IsPlaying = true;
        }
    }
}
