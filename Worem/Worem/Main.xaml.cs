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


namespace Worem
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
    }
}
