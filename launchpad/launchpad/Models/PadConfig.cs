using System.Windows;

namespace launchpad.Models
{
    public class PadConfig
    {
        public string displayName { get; set; }
        public Point dimensions { get; set; }
        public Mission[] commands { get; set; }
    }
}