using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EyeKeeper.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            var fo = @"C:\Users\Public\Pictures\Sample Pictures";
            if (Directory.Exists(fo))
            {
                _imgList.AddRange(Directory.EnumerateFiles(fo, "*.jpg", SearchOption.AllDirectories).ToList());
                //_imgList.AddRange(Directory.EnumerateFiles(fo, "*.bmp", SearchOption.AllDirectories));
                _imgList.AddRange(Directory.EnumerateFiles(fo, "*.png", SearchOption.AllDirectories));
            }
            fo = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _imgList.AddRange(Directory.EnumerateFiles(fo, "*.jpg", SearchOption.AllDirectories).ToList());
            //_imgList.AddRange(Directory.EnumerateFiles(fo, "*.bmp", SearchOption.AllDirectories));
            _imgList.AddRange(Directory.EnumerateFiles(fo, "*.png", SearchOption.AllDirectories));
            if (_imgList.Count != 0)
                CurrentImg = _imgList[0];

            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(10)};
            timer.Tick += (s, a) =>
                              {
                                  var i = _imgList.IndexOf(CurrentImg);
                                  if (i == _imgList.Count - 1)
                                      i = 0;
                                  else i = i + 1;
                                  CurrentImg = _imgList[i];
                              };
            timer.Start();
        }
     
        private readonly List<string> _imgList = new List<string>();

        #region CurrentImg

        public const string CurrentImgPropertyName = "CurrentImg";

        private string _currentImg = null;

        public string CurrentImg
        {
            get { return _currentImg; }

            set
            {
                if (_currentImg == value) { return; }
                _currentImg = value;
                var stream = new MemoryStream(File.ReadAllBytes(_currentImg));
                var img = new BitmapImage();
                img.SetSource(stream);
                CurrentImgSource = img;
                stream.Close();
                NotifyPropertyChanged(CurrentImgPropertyName);
            }
        }
        #endregion CurrentImg

        #region CurrentImgSource

        public const string CurrentImgSourcePropertyName = "CurrentImgSource";

        private ImageSource _currentImgSource = null;

        public ImageSource CurrentImgSource
        {
            get { return _currentImgSource; }

            set
            {
                if (_currentImgSource == value) { return; }
                _currentImgSource = value;
                // Update bindings, no broadcast
                NotifyPropertyChanged(CurrentImgSourcePropertyName);
            }
        }
        #endregion CurrentImgSource




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}