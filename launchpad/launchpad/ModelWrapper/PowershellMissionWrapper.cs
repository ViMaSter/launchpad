using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using launchpad.Models;

namespace launchpad.ModelWrapper
{
    public class PowershellMissionWrapper : MissionWrapper
    {
        public PowershellMission PowershellMission => (PowershellMission)mission;
        public PowershellMissionWrapper(Mission mission) : base(mission)
        {

        }

        public string outputLog = "";
        public string errorLog = "";

        public override Task Execute()
        {
            return Task.Run(() =>
            {
                var tempFilePath = Path.GetTempFileName();
                File.Move(tempFilePath, tempFilePath + ".bat");
                tempFilePath += ".bat";
                File.WriteAllText(tempFilePath, PowershellMission.command.Replace("\\n", "\n"));

                var process = Process.Start(new ProcessStartInfo(tempFilePath)
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
                process.ErrorDataReceived += (sender, e) => errorLog += $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}]{e.Data}{Environment.NewLine}";
                process.OutputDataReceived += (sender, e) => outputLog += $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}]{e.Data}{Environment.NewLine}";
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();

                File.Delete(tempFilePath);
            });
        }
    }
}