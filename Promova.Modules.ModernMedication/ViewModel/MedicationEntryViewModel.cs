using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using XtremeMvvm;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace Promova.Modules.ModernMedication.ViewModel
{

    public class MedicationEntryViewModel : ViewModelBase
    {
        #region IsTrigger

        public const string IsTriggerPropertyName = "IsTrigger";

        private bool _isTrigger = false;

        public bool IsTrigger
        {
            get { return _isTrigger; }

            set
            {
                if (_isTrigger == value) { return; }
                _isTrigger = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(IsTriggerPropertyName);
            }
        }
        #endregion IsTrigger

        #region Medications

        public const string MedicationsPropertyName = "Medications";

        private ObservableCollection<MedicationViewModel> _medications = null;

        public ObservableCollection<MedicationViewModel> Medications
        {
            get { return _medications; }

            set
            {
                if (_medications == value) { return; }
                _medications = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MedicationsPropertyName);
            }
        }
        #endregion Medications

        #region CurrentMediacation

        public const string CurrentMediacationPropertyName = "CurrentMedication";

        private MedicationViewModel _currentMedication = null;

        public MedicationViewModel CurrentMedication
        {
            get { return _currentMedication; }

            set
            {
                if (_currentMedication == value) { return; }
                _currentMedication = value;
                // Update bindings, no broadcast
                MediView = BuildMediView(value);
                RaisePropertyChanged(CurrentMediacationPropertyName);
            }
        }
        #endregion CurrentMediacation

        #region MediView

        public const string MediViewPropertyName = "MediView";

        private FrameworkElement _mediView = null;

        public FrameworkElement MediView
        {
            get { return _mediView; }

            set
            {
                if (_mediView == value) { return; }
                _mediView = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MediViewPropertyName);
            }
        }
        #endregion MediView


        private static FrameworkElement BuildMediView(MedicationViewModel medication)
        {
            if (medication == null)
                return new TextBlock { Text = "Dont support this Type " + DateTime.Now };
            if (medication.MediType == "StandigeMedikamente")
            {
                //var path = @"E:\PROMOVA\src\trunk_espas_newPrcs\Promova.DynamicXaml\ModernMedication\StandigeMedikamente.xaml";
                

                //var stream = File.OpenRead(path
                //    );
                //var control = XamlReader.Load(stream);

                //return control as FrameworkElement;

                
                return new StandigeMedikamente { DataContext = medication };
            }

            return new TextBlock { Text = "Dont support this Type" };
        }

    }
}
