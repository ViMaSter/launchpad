using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;

namespace launchpad.Models
{
    public class Mission
    {
        public Point position { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        [JsonIgnore]
        public Process currentProcess { get; set; }

        public override string ToString() => label;
    }
}