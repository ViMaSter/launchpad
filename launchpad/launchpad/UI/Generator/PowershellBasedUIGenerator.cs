using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using launchpad.Models;
using launchpad.UI.Windows;
using Grid = System.Windows.Controls.Grid;

namespace launchpad.UI.Generator
{
    public class PowershellBasedUIGenerator : MissionBasedUIGenerator<PowershellMission>
    {
        public new Button GenerateButton(PowershellMission mission)
        {
            var lastLog = new MenuItem() { Header = "Open last log" };
            lastLog.Click += (sender, args) =>
            {
                new LogOutputWindow(outputLog, LogOutputWindow.LogType.LOG).ShowDialog();
            };
            var lastErrorLog = new MenuItem() { Header = "Open last error log" };
            lastErrorLog.Click += (sender, args) =>
            {
                new LogOutputWindow(errorLog, LogOutputWindow.LogType.ERROR).ShowDialog();
            };

            var button = base.GenerateButton(mission);
            button.UnmodifiedClickEvent += OnClick;

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

        public UserControl GeneratePage(PowershellMission mission)
        {
            return new PowershellUserControl();
        }

        private void OnClick(object sender, EventArgs e)
        {
            var tempFilePath = Path.GetTempFileName();
            File.Move(tempFilePath, tempFilePath + ".ps1");
            tempFilePath += ".ps1";
            var button = (Button)sender;
            PowershellMission mission = (PowershellMission)button.Content;
            File.WriteAllText(tempFilePath, mission.command.Replace("\\n", "\n"));

            var process = Process.Start(new ProcessStartInfo(tempFilePath)
            {
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            File.Delete(tempFilePath);
        }

        private string errorLog = "";
        private string outputLog = "";
        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            outputLog += $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}]{e.Data}{Environment.NewLine}";
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            errorLog += e.Data + Environment.NewLine;
        }
    }
}