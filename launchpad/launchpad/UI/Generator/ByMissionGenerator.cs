using System.Windows.Controls;
using launchpad.Models;

namespace launchpad.UI.Generator
{
    public abstract class ByMissionGenerator<TMission> where TMission : Mission
    {
        public Button GenerateButton(TMission mission)
        {
            var button = new Button
            {
                Content = mission
            };
            System.Windows.Controls.Grid.SetColumn(button, (int)mission.position.X);
            System.Windows.Controls.Grid.SetRow(button, (int)mission.position.Y);
            return button;
        }
    }
}