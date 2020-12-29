using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using launchpad.Annotations;
using Newtonsoft.Json;

namespace launchpad.Models
{
    public abstract class Mission : INotifyPropertyChanged, ICloneable
    {
        private string _type;
        private string _label;

        [JsonProperty]
        private Point position { get; set; }

        [JsonIgnore]
        public int X
        {
            get => (int) position.X;
            set
            {
                position = new Point(value, position.Y);
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public int Y
        {
            get => (int)position.Y;
            set
            {
                position = new Point(position.X, value);
                OnPropertyChanged();
            }
        }

        public string type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string label
        {
            get => _label;
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }

        public override string ToString() => label;
        public abstract object Clone();

        protected void Clone(ref Mission output)
        {
            output._type = _type;
            output._label = _label;
            output.position = position;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}