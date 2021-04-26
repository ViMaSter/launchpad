using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using launchpad.Annotations;
using launchpad.Models;

namespace launchpad.ModelWrapper
{
    public abstract class MissionWrapper : INotifyPropertyChanged
    {
        public enum MissionExecutionState
        {
            INITIALIZED,
            RUNNING,
            FAILED,
            SUCCESSFUL
        };

        private MissionExecutionState _currentMissionExecutionState = MissionExecutionState.INITIALIZED;
        public MissionExecutionState CurrentMissionExecutionState
        {
            get => _currentMissionExecutionState;
            set
            {
                _currentMissionExecutionState = value;
                OnPropertyChanged();
            }
        }

        protected MissionWrapper(Mission mission)
        {
            this.mission = mission;
        }

        public abstract Task StartExecution();

        public Mission mission { get; init; }
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
