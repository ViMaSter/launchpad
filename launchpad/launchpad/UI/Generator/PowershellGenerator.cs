using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using launchpad.Models;

namespace launchpad.UI.Generator
{
    public class PowershellGenerator : ByMissionGenerator<PowershellMission>
    {
        public Button Generate(PowershellMission mission)
        {
            var button = base.GenerateButton(mission);
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