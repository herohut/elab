using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using GoogleGrabber2.ViewModel;
using mshtml;

namespace GoogleGrabber2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<PropertyChangedMessage<string>>(this, a =>
                {
                    if (!(a.Sender is MainViewModel))
                        return;


                    navigator.Navigate(a.NewValue);
                });

            navigator.LoadCompleted += navigator_LoadCompleted;
        }

        void navigator_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var doc = ((IHTMLDocument3)navigator.Document);
            var txt = doc.documentElement.innerHTML;

            
            
            //((ViewModelLocator)this.DataContext).Main.HandleHtml1(txt);
            ((ViewModelLocator)this.DataContext).Main.HandleHtml2(txt);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //((ViewModelLocator) this.DataContext).Main.Start1();
            ((ViewModelLocator)this.DataContext).Main.Start2();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ((ViewModelLocator) this.DataContext).Main.WritePhp();
        }
    }
}
