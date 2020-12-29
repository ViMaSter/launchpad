using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace launchpad.Models
{
    public class CmdMission : Mission
    {
        private string _workingDirectory;
        private string _command;

        public string workingDirectory
        {
            get => _workingDirectory;
            set { 
                _workingDirectory = value;
                OnPropertyChanged();
            }
        }

        public string command
        {
            get => _command;
            set
            {
                _command = value; 
                OnPropertyChanged();
            }
        }
    }
}