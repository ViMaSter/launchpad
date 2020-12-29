using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using launchpad.Models;

namespace launchpad.ModelWrapper
{
    public abstract class MissionWrapper
    {
        protected MissionWrapper(Mission mission)
        {
            this.mission = mission;
        }
        public abstract Task Execute();

        public Mission mission { get; init; }
    }
}
