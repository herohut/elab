using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Promova.Modules.ModernMedication.ViewModel;
using WindowsClient.UI.General;

namespace Promova.Modules.ModernMedication
{
    public class ModernMedicationApplication : GenericApplicationControl
    {
        private readonly ElementHost _wpfHost = new ElementHost{Dock = DockStyle.Fill};
        private readonly MediControl _mediControl = new MediControl();
        protected override void SetApplicationItem()
        {
            if (!this.bodyPanel.Controls.Contains(_wpfHost))
            {
                var mediControl = new MediControlViewModel
                {
                    MedicationEntries =
                        new System.Collections.ObjectModel.ObservableCollection
                        <MedicationEntryViewModel>(
                        FormSample.CreateSampleEntry(200))
                };
                _mediControl.DataContext = mediControl;
                _wpfHost.Child = _mediControl;
                this.bodyPanel.Controls.Add(_wpfHost);
            }
        }

        protected override void OnAfterReadSettings()
        {
            applicationItem.GenericApplicationData.ShowHelpControl = false;
        }
    }
}
