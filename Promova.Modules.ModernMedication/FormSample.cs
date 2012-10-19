using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Markup;
using Promova.Modules.ModernMedication.ViewModel;

namespace Promova.Modules.ModernMedication
{
    public partial class FormSample : Form
    {
        public FormSample()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        public static MedicationEntryViewModel CreateSampleEntry()
        {
            var dat = new MedicationViewModel { MediType = "StandigeMedikamente" };
            dat.Fields["Verordnetab"] = DateTime.Now.AddDays(-10);

            var mediEntry = new MedicationEntryViewModel
            {
                Medications =
                    new System.Collections.ObjectModel.ObservableCollection<MedicationViewModel> { dat }
            };

            return mediEntry;
        }

        public static List<MedicationEntryViewModel> CreateSampleEntry(int count)
        {
            var output = new List<MedicationEntryViewModel>();
            for (int i = 0; i < count; i++)
            {
                var entry = CreateSampleEntry();
                entry.CurrentMedication = entry.Medications[0];
                output.Add(entry);
            }

            return output;
        }

        private MediControlViewModel mediControl;
        private void button1_Click(object sender, EventArgs e)
        {
            mediControl = new MediControlViewModel
            {
                MedicationEntries =
                    new System.Collections.ObjectModel.ObservableCollection
                    <MedicationEntryViewModel>(
                    CreateSampleEntry(1))
            };


            elementHost1.Child = new MediControl { DataContext = mediControl };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var entry = CreateSampleEntry(1)[0];
            entry.CurrentMedication = null;
            mediControl.MedicationEntries.Insert(0, entry);
        }
    }
}
