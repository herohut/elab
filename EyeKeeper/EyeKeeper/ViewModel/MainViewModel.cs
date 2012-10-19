using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using Microsoft.Silverlight.Windows.Platform;

namespace EyeKeeper.ViewModel
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
            ForeBrush = Black;
            BackBrush = White;
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                ReadConfig();
            }
        }

        void ReadConfig()
        {
            if (File.Exists("EyeKeeperConfig.dat"))
            {
                var lines = File.ReadLines("EyeKeeperConfig.dat").ToList();
                this.WorkingTime = int.Parse(lines[0]);
                this.BreakTime = int.Parse(lines[1]);
            }
            else
                SaveConfig();
        }


        void SaveConfig()
        {
            var lines = new[] { WorkingTime.ToString(), BreakTime.ToString() };
            File.WriteAllLines("EyeKeeperConfig.dat", lines);
        }

        #region WorkingTime

        public const string WorkingTimePropertyName = "WorkingTime";

        private int _workingTime = 20;

        public int WorkingTime
        {
            get { return _workingTime; }

            set
            {
                if (_workingTime == value) { return; }
                _workingTime = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(WorkingTimePropertyName);
                StopBusiness();
            }
        }

        private void StopBusiness()
        {
            IsWindowEnable = true;
            if (_workTimer != null && _workTimer.IsEnabled)
                _workTimer.Stop();
            if (_countDownTimer != null && _countDownTimer.IsEnabled)
                _countDownTimer.Stop();

            _isWorking = false;
        }

        #endregion WorkingTime


        #region BreakTime

        public const string BreakTimePropertyName = "BreakTime";

        private int _breakTime = 20;

        public int BreakTime
        {
            get { return _breakTime; }

            set
            {
                if (_breakTime == value) { return; }
                _breakTime = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(BreakTimePropertyName);
                StopBusiness();
            }
        }
        #endregion BreakTime


        #region WaitingText

        public const string WaitingTextPropertyName = "WaitingText";

        private string _waitingText = "? SECONDS";

        public string WaitingText
        {
            get { return _waitingText; }

            set
            {
                if (_waitingText == value) { return; }
                _waitingText = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(WaitingTextPropertyName);
            }
        }
        #endregion WaitingText

        #region IsWindowEnable

        public const string IsWindowEnablePropertyName = "IsWindowEnable";

        private bool _isWindowEnable = true;

        public bool IsWindowEnable
        {
            get { return _isWindowEnable; }

            set
            {
                if (_isWindowEnable == value) { return; }
                _isWindowEnable = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(IsWindowEnablePropertyName);
            }
        }
        #endregion IsWindowEnable


        private void CountDown()
        {
            ForeBrush = White;
            BackBrush = Black;

            IsWindowEnable = false;
            
            App.Current.Host.Content.IsFullScreen = true;
            _countDownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            var current = DateTime.Now;
            _countDownTimer.Tick += (s, a) =>
                              {
                                  var seconds = (DateTime.Now - current).TotalSeconds;
                                  if (seconds < BreakTime)
                                  {
                                      App.Current.MainWindow.WindowState = WindowState.Maximized;
                                      App.Current.MainWindow.Visibility = Visibility.Visible;
                                      WaitingText = (int)(BreakTime - seconds) + " SECONDS";


                                      return;
                                  }
                                  IsWindowEnable = true;
                                  _countDownTimer.Stop();
                                  Working();
                              };
            _countDownTimer.Start();
        }

        private readonly SolidColorBrush Black = new SolidColorBrush { Color = Colors.Black };
        private readonly SolidColorBrush White = new SolidColorBrush { Color = Colors.White };

        #region ForeBrush

        public const string ForeBrushPropertyName = "ForeBrush";

        private Brush _foreBrush = null;

        public Brush ForeBrush
        {
            get { return _foreBrush; }

            set
            {
                if (_foreBrush == value) { return; }
                _foreBrush = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ForeBrushPropertyName);
            }
        }
        #endregion ForeBrush

        #region BackBrush

        public const string BackBrushPropertyName = "BackBrush";

        private Brush _backBrush = null;

        public Brush BackBrush
        {
            get { return _backBrush; }

            set
            {
                if (_backBrush == value) { return; }
                _backBrush = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(BackBrushPropertyName);
            }
        }
        #endregion BackBrush




        private DispatcherTimer _workTimer;
        private DispatcherTimer _countDownTimer;
        private void Working()
        {
            ForeBrush = Black;
            BackBrush = White;

            App.Current.Host.Content.IsFullScreen = false;
            IsWindowEnable = true;
            App.Current.MainWindow.WindowState = WindowState.Normal;
            App.Current.MainWindow.Visibility = Visibility.Collapsed;

            _workTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(WorkingTime) };

#if DEBUG
            _workTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
#endif

            

            _workTimer.Tick += (s, a) =>
            {
                _workTimer.Stop();
                CountDown();
            };
            _workTimer.Start();
        }

        private bool _isWorking = false;
        public void DoBusiness()
        {
            if (!_isWorking)
            {
                Working();
                _isWorking = true;
            }
        }

        #region CanGoBack

        public const string CanGoBackPropertyName = "CanGoBack";

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                if (_canGoBack == value) { return; }
                _canGoBack = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(CanGoBackPropertyName);
            }
        }
        #endregion CanGoBack

        public void Close()
        {
            if (MessageBox.Show("Do you really want to close?", "Info", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            SaveConfig();
            NotificationArea.Current.RemoveNotificationIcon(Guid.Parse(((App)Application.Current).ApplicationID));
            App.Current.MainWindow.Close();
        }


        public void Minimize()
        {
            if (!_setIco)
            {
                //read the icon image
                using (var br = new BinaryReader(Application.GetResourceStream
                                                     (new Uri("/EyeKeeper;component/Media/Logo-tray.png",
                                                              UriKind.RelativeOrAbsolute)).Stream))
                {
                    //add the icon 
                    NotificationArea.Current.AddNotificationIcon(
                        Guid.Parse(((App)Application.Current).ApplicationID),
                        "Eye Keeper. http://eking.vn", false,
                        br.ReadBytes((int)br.BaseStream.Length), ImageDataType.PNG);
                    NotificationArea.Current.IconClick +=
                        (s, a) =>
                        {
                            App.Current.MainWindow.Visibility = Visibility.Visible;
                            StopBusiness();
                        };
                }
                _setIco = true;
            }

            App.Current.MainWindow.Visibility = Visibility.Collapsed;
            DoBusiness();
        }

        private bool _setIco;

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}