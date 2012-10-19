using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using Microsoft.Silverlight.Windows.Platform;
using System.Linq;

namespace Worem.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                _notWindow = new NotificationWindow();
                var popControl = new PopupControl { DataContext = this };
                _notWindow.Content = popControl;
                // Code runs "for real"
                initTime = DateTime.Now;
                mainTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(timerSleep) };
                mainTimer.Tick += new EventHandler(timer_Tick);
                mainTimer.Start();
                wc = new WebClient();

                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringAsync(vocaUrl);

            }
        }

        private bool _focal = false;

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ManageText(e.Result);
        }

        void ManageText(string txt)
        {
            var words = txt.Split(new[] { "\n" }, StringSplitOptions.None).Where(t => !string.IsNullOrEmpty(t.Trim()));
            wordList.Clear();
            foreach (var w in words)
            {
                if (!wordList.Contains(w))
                    wordList.Add(w);
            }
        }

        Uri vocaUrl = new Uri("http://eking.vn/eNote/NoteMain/GetNoteText?f=voca");
        WebClient wc;
        DispatcherTimer mainTimer;
        const int resolusion = 10;
        double timerSleep = 1 * resolusion;
        void timer_Tick(object sender, EventArgs e)
        {
            if (_pause)
                return;
            var mins = (DateTime.Now - initTime).TotalSeconds;
            var cal = mins % DOWNLOAD_SLEEP;
            if (cal <= timerSleep)
            {
                if (_focal)
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "vocabulary.txt");
                }
                else
                {
                    // Do download logic
                    wc = new WebClient();
                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                    wc.DownloadStringAsync(vocaUrl);
                }
            }

            cal = mins % SHOW_SLEEP;
            if (cal <= timerSleep)
            {
                // Do notify logic
                if (wordList.Count <= 0)
                    return;
                if (currentIndex >= wordList.Count - 1)
                    currentIndex = 0;
                else
                    currentIndex = currentIndex + 1;
                NotText = wordList[currentIndex];

                Notify();
            }
        }

        DateTime initTime;
        List<string> wordList = new List<string>() { "Hero", "Hut" };
        bool reset;
        int currentIndex;
        public void Close()
        {
            if (MessageBox.Show("Do you really want to close?", "Info", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            SaveConfig();
            if (mainTimer != null && mainTimer.IsEnabled)
                mainTimer.Stop();

            NotificationArea.Current.RemoveNotificationIcon(Guid.Parse(((App)Application.Current).ApplicationID));
            App.Current.MainWindow.Close();
        }

        bool _pause;
        public void Minimize()
        {
            if (!_setNotifyIcon)
            {
                //read the icon image
                using (var br = new BinaryReader(Application.GetResourceStream
                                                     (new Uri("/Worem;component/Media/Logo-tray.png",
                                                              UriKind.RelativeOrAbsolute)).Stream))
                {
                    //add the icon 
                    NotificationArea.Current.AddNotificationIcon(
                        Guid.Parse(((App)Application.Current).ApplicationID),
                        "Word Remember. http://eking.vn", false,
                        br.ReadBytes((int)br.BaseStream.Length), ImageDataType.PNG);
                    NotificationArea.Current.IconClick +=
                        (s, a) =>
                        {
                            App.Current.MainWindow.Visibility = Visibility.Visible;
                        };

                    var pause = new MenuItem(2, "Pause");
                    var exit = new MenuItem(1, "Exit");

                    NotificationArea.Current.CreateContextMenu(new List<MenuItem> { pause, exit });
                    NotificationArea.Current.ContextMenuSelection +=
                        (s, a) =>
                        {
                            if (a.CommandID == 1)
                                Close();
                            if (a.CommandID == 2)
                            {
                                pause.Text = pause.Text == "Resume" ? "Exit" : "Resume";
                                _pause = pause.Text == "Resume";
                                NotificationArea.Current.UpdateMenuItem(pause);
                            }


                        };
                    NotificationArea.Current.IconRightClick +=
                        (s, a) =>
                        {
                            NotificationArea.Current.TrackContextMenu();
                        };
                }
                _setNotifyIcon = true;
            }

            App.Current.MainWindow.Visibility = Visibility.Collapsed;
        }

        private bool _setNotifyIcon;


        const int DOWNLOAD_SLEEP = 30 * resolusion; // Seconds
        const int SHOW_SLEEP = 5 * resolusion; // Seconds
        const int SHOW_DURA = 7000; // Milliseconds
        readonly NotificationWindow _notWindow;

        public void Notify()
        {
            var txt = NotText.Length > 30 ? NotText.Substring(0, 30) + "..." : NotText;
            NotificationArea.Current.UpdateNotificationIconToolTip(Guid.Parse(((App)Application.Current).ApplicationID), txt);
            _notWindow.Show(SHOW_DURA);
        }


        private void SaveConfig()
        {
            //throw new System.NotImplementedException();
        }

        #region NotText

        public const string NotTextPropertyName = "NotText";

        private string _notText = "Worem";

        public string NotText
        {
            get { return _notText; }

            set
            {
                if (_notText == value) { return; }
                _notText = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(NotTextPropertyName);
            }
        }
        #endregion NotText


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}