using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace launchpad.UI.UserControls
{
    public class MissionUserControl : UserControl { }
    public abstract class MissionUserControl<TMission> : MissionUserControl
    {
        public abstract void RenderMission(TMission input);
        public abstract void UpdateMission(ref TMission mission);
    }
}
