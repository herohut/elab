using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace Promova.Modules.ModernMedication.ViewModel
{
    public class MediControlViewModel: ViewModelBase
    {
        public static MedicationEntryViewModel CreateTriggerEntry()
        {
            var dat = new MedicationViewModel { MediType = "StandigeMedikamente" };
            dat.Fields["Verordnetab"] = DateTime.Now.AddDays(-10);

            var mediEntry = new MedicationEntryViewModel
            {
                Medications =
                    new ObservableCollection<MedicationViewModel> { dat }
            };

            return mediEntry;
        }

        public MediControlViewModel()
        {
            if(IsInDesignMode)
            {
                
            }else
            {
                MedicationEntries =
                    new ObservableCollection
                        <MedicationEntryViewModel>();
                MedicationEntries.Add(CreateTriggerEntry());

            }
        }

        #region MedicationEntries

        public const string MedicationEntriesPropertyName = "MedicationEntries";

        private ObservableCollection<MedicationEntryViewModel> _medicationEntries = null;

        public ObservableCollection<MedicationEntryViewModel> MedicationEntries
        {
            get { return _medicationEntries; }

            set
            {
                if (_medicationEntries == value) { return; }
                _medicationEntries = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MedicationEntriesPropertyName);
            }
        }
        #endregion MedicationEntries

    }
}
