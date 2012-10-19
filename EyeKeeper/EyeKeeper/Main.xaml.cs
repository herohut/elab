using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using EyeKeeper.ViewModel;

namespace EyeKeeper
{
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage == null)
                return;
            if (_currentPage.NavigationService.CanGoBack)
                _currentPage.NavigationService.GoBack();
        }

        private Page _currentPage;
        private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _currentPage = (Page)frame.Content;

            ((MainViewModel) this.DataContext).CanGoBack = _currentPage.NavigationService.CanGoBack;
        }
    }
}
