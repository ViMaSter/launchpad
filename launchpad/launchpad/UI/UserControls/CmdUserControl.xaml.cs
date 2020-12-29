using launchpad.Models;

namespace launchpad.UI.UserControls
{
    public partial class CmdUserControl
    {
        public CmdUserControl()
        {
            InitializeComponent();
        }

        public override void RenderMission(CmdMission mission)
        {
            this.WorkingDirectory.Text = mission.workingDirectory;
            this.Script.Text = mission.command;
        }

        public override void UpdateMission(ref CmdMission mission)
        {
            mission.workingDirectory = this.WorkingDirectory.Text;
            mission.command = this.Script.Text;
        }
    }
}