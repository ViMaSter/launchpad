using launchpad.Models;

namespace launchpad.UI.UserControls
{
    public partial class PowershellUserControl
    {
        public PowershellUserControl()
        {
            InitializeComponent();
        }

        public override void RenderMission(PowershellMission mission)
        {
            this.WorkingDirectory.Text = mission.workingDirectory;
            this.Script.Text = mission.command;
        }

        public override void UpdateMission(ref PowershellMission mission)
        {
            mission.workingDirectory = this.WorkingDirectory.Text;
            mission.command = this.Script.Text;
        }
    }
}