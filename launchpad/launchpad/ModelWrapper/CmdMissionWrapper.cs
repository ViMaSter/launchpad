﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using launchpad.Models;

namespace launchpad.ModelWrapper
{
    public class CmdMissionWrapper : MissionWrapper
    {
        public CmdMission CmdMission => (CmdMission) mission;
        public CmdMissionWrapper(Mission mission) : base(mission)
        {
            
        }

        public string outputLog = "";
        public string errorLog = "";

        public override Task StartExecution()
        {
            CurrentMissionExecutionState = MissionExecutionState.RUNNING;
            return Task.Run(() =>
            {
                var tempFilePath = Path.GetTempFileName();
                File.Move(tempFilePath, tempFilePath + ".bat");
                tempFilePath += ".bat";
                File.WriteAllText(tempFilePath, CmdMission.command.Replace("\\n", "\n"));

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

                CurrentMissionExecutionState = process.ExitCode == 0
                    ? MissionExecutionState.SUCCESSFUL
                    : MissionExecutionState.FAILED;

                File.Delete(tempFilePath);
            });
        }
    }
}
