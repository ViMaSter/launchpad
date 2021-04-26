using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using launchpad.Models;
using launchpad.ModelWrapper;
using launchpad.UI.Buttons;
using launchpad.UI.UserControls;
using launchpad.UI.Windows;
using Grid = System.Windows.Controls.Grid;

namespace launchpad.UI.Generator
{
    public class CmdBasedUIGenerator : MissionBasedUIGenerator<CmdMissionWrapper>
    {
        public new ModifierKeysButton GenerateButton(CmdMissionWrapper missionWrapper)
        {
            var lastLog = new MenuItem() { Header = "Open last log" };
            lastLog.Click += (sender, args) =>
            {
                new LogOutputWindow(missionWrapper.outputLog, LogOutputWindow.LogType.LOG).ShowDialog();
            };
            var lastErrorLog = new MenuItem() { Header = "Open last error log" };
            lastErrorLog.Click += (sender, args) =>
            {
                new LogOutputWindow(missionWrapper.errorLog, LogOutputWindow.LogType.ERROR).ShowDialog();
            };

            var button = base.GenerateButton(missionWrapper);
            button.UnmodifiedClickEvent += (sender, args) => { missionWrapper.StartExecution(); };

            var edit = new MenuItem() { Header = "Edit" };
            edit.Click += (sender, args) =>
            {
                button.CtrlClickEvent(sender, args);
            };

            button.ContextMenu = new ContextMenu()
            {
                Items =
                {
                    edit,
                    new Separator(),
                    lastLog,
                    lastErrorLog
                }
            };

            /*button.Background = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(46, 46, 46), 0),
                    new GradientStop(Color.FromRgb(23, 23, 23), 1),
                }
            };*/
            
            return button;
        }

        public MissionUserControl GeneratePage(CmdMissionWrapper missionWrapper)
        {
            return new CmdUserControl
            {
                WorkingDirectory = {Text = missionWrapper.CmdMission.workingDirectory}, 
                Script = {Text = missionWrapper.CmdMission.command}
            };
        }
    }
}