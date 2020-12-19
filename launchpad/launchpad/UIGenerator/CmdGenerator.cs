using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using launchpad.Models;

namespace launchpad.UIGenerator
{
    public class CmdGenerator : ByMissionGenerator<CmdMission>
    {
        public new Button Generate(CmdMission mission)
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

            var button = base.Generate(mission);
            button.Click += OnClick;

            button.ContextMenu = new ContextMenu()
            {
                Items =
                {
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
                    new GradientStop(Color.FromRgb(46, 46, 46), 0),
                    new GradientStop(Color.FromRgb(23, 23, 23), 1),
                }
            };
            return button;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var getTempBat = Path.GetTempFileName();
            File.Move(getTempBat, getTempBat + ".bat");
            getTempBat += ".bat";
            var button = (Button) sender;
            CmdMission mission = (CmdMission) button.Content;
            File.WriteAllText(getTempBat, mission.command.Replace("\\n", "\n"));

            var process = Process.Start(new ProcessStartInfo(getTempBat)
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