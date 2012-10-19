using System;
using System.Collections.Generic;
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

namespace Promova.Modules.ModernMedication
{
    /// <summary>
    /// Interaction logic for MediControl.xaml
    /// </summary>
    public partial class MediControl : UserControl
    {
        public MediControl()
        {
            InitializeComponent();
        }

        private void btnPrint_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                new System.Windows.Controls.PrintDialog().PrintVisual(this, "HERO TEST");
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
