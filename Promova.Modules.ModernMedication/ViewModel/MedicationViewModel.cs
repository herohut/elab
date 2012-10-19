using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace Promova.Modules.ModernMedication.ViewModel
{
    public class MedicationViewModel : ViewModelBase
    {
        public override string ToString()
        {
            return this.MediType;
        }
        #region MediType

        public const string MediTypePropertyName = "MediType";

        private string _mediType = null;

        public string MediType
        {
            get { return _mediType; }

            set
            {
                if (_mediType == value) { return; }
                _mediType = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MediTypePropertyName);
            }
        }
        #endregion MediType

        #region Medikament

        public const string MedikamentPropertyName = "Medikament";

        private string _medikament = null;

        public string Medikament
        {
            get { return _medikament; }

            set
            {
                if (_medikament == value) { return; }
                _medikament = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(MedikamentPropertyName);
            }
        }
        #endregion Medikament

        public object Me
        {
            get { return this; }
            set
            {
                var val = (KeyValuePair<string, object>)value;
                if (Fields.ContainsKey(val.Key))
                    Fields[val.Key] = val.Value;

            }
        }

        public Dictionary<string, object> Fields = new Dictionary<string, object>();

    }
}
