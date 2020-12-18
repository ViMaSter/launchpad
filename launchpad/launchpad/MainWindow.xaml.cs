using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace launchpad
{
    public abstract class ButtonGenerator<M> where M : Mission
    {
        public Button Generate(M mission)
        {
            var button = new Button
            {
                Content = mission.label
            };
            System.Windows.Controls.Grid.SetColumn(button, (int)mission.position.X);
            System.Windows.Controls.Grid.SetRow(button, (int)mission.position.Y);
            return button;
        }
    }

    public class CmdButtonGenerator : ButtonGenerator<CmdMission>
    {
        public new Button Generate(CmdMission mission)
        {
            var button = base.Generate(mission);
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
    }

    public class PowershellButtonGenerator : ButtonGenerator<PowershellMission>
    {
        public new Button Generate(PowershellMission mission)
        {
            var button = base.Generate(mission);
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
    }

    public partial class MainWindow
    {
        public void Reset()
        {
            MissionGrid.RowDefinitions.Clear();
            MissionGrid.ColumnDefinitions.Clear();
            MissionGrid.Children.Clear();
        }
        public void LoadPadConfig(PadConfig config)
        {
            Reset();
            for (var x = 0; x < config.grid.dimensions.X; ++x)
            {
                MissionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (var y = 0; y < config.grid.dimensions.Y; ++y)
            {
                MissionGrid.RowDefinitions.Add(new RowDefinition());
            }

            foreach (var mission in config.commands)
            {
                var button = GenerateButton(mission);
                button.Height = 50;
                MissionGrid.Children.Add(button);
            }
        }

        public Button GenerateButton(Mission mission)
        {
            var c = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract);
            var type = c
                .First(myType => myType.BaseType.GenericTypeArguments.Contains(mission.GetType()));

            var gen = Activator.CreateInstance(type);
            
            return (Button)gen.GetType().GetMethod("Generate").Invoke(gen, new object[]{mission});
        }

        public MainWindow()
        {
            InitializeComponent();

            var pad = File.ReadAllText("C:\\prj\\10_launchpad\\0_git\\pad.json");
            var seri = JsonConvert.DeserializeObject<PadConfig>(pad, new MissionConverter());
            LoadPadConfig(seri);
        }
    }
}
