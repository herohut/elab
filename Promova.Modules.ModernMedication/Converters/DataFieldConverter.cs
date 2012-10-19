using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Promova.Modules.ModernMedication.ViewModel;

namespace Promova.Modules.ModernMedication.Converters
{
    public class DataFieldConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MedicationViewModel))
                return value;
            var param = (string)parameter;
            var medication = (MedicationViewModel)value;
            if (medication.Fields.ContainsKey(param))
                return medication.Fields[param];
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new KeyValuePair<string, object>(parameter as string, value);
        }
    }
}
