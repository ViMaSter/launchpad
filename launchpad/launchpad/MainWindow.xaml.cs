using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Input;
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
                Content = mission
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
        private const int ROW_WIDTH = 150;
        private const int ROW_HEIGHT = 50;
        public void Reset()
        {
            MissionGrid.RowDefinitions.Clear();
            MissionGrid.ColumnDefinitions.Clear();
            MissionGrid.Children.Clear();
        }
        public void LoadPadConfig(PadConfig config)
        {
            for (var x = 0; x < config.grid.dimensions.X; ++x)
            {
                MissionGrid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(ROW_WIDTH) });
            }   
            for (var y = 0; y < config.grid.dimensions.Y; ++y)
            {
                MissionGrid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(ROW_HEIGHT) });
            }

            Button[,] buttonByPos = new Button[(int)config.grid.dimensions.X, (int)config.grid.dimensions.Y];

            for (var x = 0; x < config.grid.dimensions.X; ++x)
            {
                for (var y = 0; y < config.grid.dimensions.Y; ++y)
                {
                    buttonByPos[x,y] = new Button()
                    {
                        Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                        Width = ROW_WIDTH,
                        Height = ROW_HEIGHT
                    };
                    System.Windows.Controls.Grid.SetColumn(buttonByPos[x, y], x);
                    System.Windows.Controls.Grid.SetRow(buttonByPos[x, y], y);
                    MissionGrid.Children.Add(buttonByPos[x, y]);
                }
            }

            foreach (var mission in config.commands)
            {
                buttonByPos[(int)mission.position.X, (int)mission.position.Y] = GenerateButton(mission);
                MissionGrid.Children.Add(buttonByPos[(int)mission.position.X, (int)mission.position.Y]);
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
            
            var button = (Button)gen.GetType().GetMethod("Generate").Invoke(gen, new object[]{mission});
            button.Height = ROW_HEIGHT;
            button.Width = ROW_WIDTH;
            return button;
        }

        public MainWindow()
        {
            InitializeComponent();

            PadSelector.Items.Add("");
            PadSelector.Items.Add("C:\\prj\\10_launchpad\\0_git\\pad.json");
            PadSelector.SelectionChanged += PadSelector_SelectionChanged;

            Reset();
        }

        private void PadSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reset();
            if (e.AddedItems.Count <= 0)
            {
                return;
            }

            string item = (string) e.AddedItems[0];
            if (string.IsNullOrEmpty(item))
            {
                return;
            }

            var pad = File.ReadAllText(item);
            var serializedPad = JsonConvert.DeserializeObject<PadConfig>(pad, new MissionConverter());
            LoadPadConfig(serializedPad);
        }
    }
}
