using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using launchpad.Models;

namespace launchpad.UIGenerator
{
    public class PowershellGenerator : ByMissionGenerator<PowershellMission>
    {
        public new Button Generate(PowershellMission mission)
        {
            var button = base.Generate(mission);
            button.Background = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(19, 45, 96), 0),
                    new GradientStop(Color.FromRgb(15, 35, 75), 1),
                }
            };
            return button;
        }
    }
}