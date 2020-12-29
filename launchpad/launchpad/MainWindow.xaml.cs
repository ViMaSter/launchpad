using System;
using System.Collections.Generic;
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
using launchpad.JsonConverter;
using launchpad.Models;
using launchpad.ModelWrapper;
using launchpad.UI.Generator;
using Newtonsoft.Json;

namespace launchpad
{
    public partial class MainWindow
    {
        private const int ROW_WIDTH = 150;
        private const int ROW_HEIGHT = 50;
        private ContentPresenter[,] buttonByPos;

        public void Reset()
        {
            MissionGrid.RowDefinitions.Clear();
            MissionGrid.ColumnDefinitions.Clear();
            MissionGrid.Children.Clear();
            buttonByPos = new ContentPresenter[0,0];
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

            buttonByPos = new ContentPresenter[(int)config.grid.dimensions.X, (int)config.grid.dimensions.Y];

            for (var x = 0; x < config.grid.dimensions.X; ++x)
            {
                for (var y = 0; y < config.grid.dimensions.Y; ++y)
                {
                    buttonByPos[x, y] = new ContentPresenter
                    {
                        Content = new Button()
                        {
                            Background = new SolidColorBrush(Color.FromRgb(255, 0, 0))
                        }
                    };
                    System.Windows.Controls.Grid.SetColumn(buttonByPos[x, y], x);
                    System.Windows.Controls.Grid.SetRow(buttonByPos[x, y], y);
                    MissionGrid.Children.Add(buttonByPos[x, y]);
                }
            }

            foreach (var mission in config.commands)
            {
                RebuildButton(mission);
            }
        }

        private void RebuildButton(Mission mission)
        {
            var button = App.GenerateUIElement<ModifierKeysButton>(App.WrapMission(mission));
            button.CtrlClickEvent += (sender, args) =>
            {
                RebuildButton(((MissionWrapper)button.DataContext).mission);
            };

            buttonByPos[mission.X, mission.Y].Content = button;
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
