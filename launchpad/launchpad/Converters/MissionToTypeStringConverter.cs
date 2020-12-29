using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using launchpad.Models;

namespace launchpad.Converters
{
    public class MissionToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
            {
                throw new NotSupportedException("Only 'string' can be converted to Missions");
            }

            var missionType = (string)value;
            return Activator.CreateInstance(((App)Application.Current).AvailableMissionTypes.First(type => type.Name.ToLower().StartsWith(missionType.ToLower())));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Mission))
            {
                throw new NotSupportedException("Only 'Mission' models can be converted");
            }

            Mission mission = (Mission)value;
            return mission.GetType().Name.Replace("Mission", "").ToLower();
        }
    }
}
