using System.Windows.Controls;
using launchpad.Models;

namespace launchpad.UIGenerator
{
    public abstract class ByMissionGenerator<M> where M : Mission
    {
        public Button Generate(M mission)
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