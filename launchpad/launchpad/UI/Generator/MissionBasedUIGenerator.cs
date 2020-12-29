using System.Windows.Controls;
using System.Windows.Data;
using launchpad.Models;
using launchpad.ModelWrapper;
using Grid = System.Windows.Controls.Grid;

namespace launchpad.UI.Generator
{
    public abstract class MissionBasedUIGenerator<TMissionWrapper> where TMissionWrapper : MissionWrapper
    {
        public ModifierKeysButton GenerateButton(TMissionWrapper missionWrapper)
        {
            var button = new ModifierKeysButton
            {
                DataContext = missionWrapper,
            };
            button.SetBinding(ContentControl.ContentProperty, new Binding("mission.label"));
            Grid.SetColumn(button, missionWrapper.mission.X);
            Grid.SetRow(button, missionWrapper.mission.Y);
            button.CtrlClickEvent += (sender, args) =>
            {
                if (button.DataContext == null)
                {
                    return;
                }
                var newMission = Windows.EditMissionWindow.OpenEdit((MissionWrapper)button.DataContext);
            };
            return button;
        }
    }
}