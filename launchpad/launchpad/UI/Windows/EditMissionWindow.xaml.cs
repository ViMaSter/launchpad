using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using launchpad.Models;
using launchpad.ModelWrapper;
using launchpad.UI.Generator;
using launchpad.UI.UserControls;
using Microsoft.VisualBasic.FileIO;
using Grid = System.Windows.Controls.Grid;

namespace launchpad.UI.Windows
{
    public partial class EditMissionWindow
    {
        private EditMissionWindow()
        {
            InitializeComponent();
            foreach (var result in ((App) Application.Current).AvailableMissionTypes)
            {
                Type.Items.Add(result.Name.Replace("MissionWrapper", "").ToLower());
            }
        }

        public static TMissionWrapper OpenEdit<TMissionWrapper>(TMissionWrapper input) where TMissionWrapper : MissionWrapper
        {
            var window = new EditMissionWindow
            {
                DataContext = input.mission,
                TypeSpecificUserControl = {Content = App.GenerateUIElement<MissionUserControl>(input)}
            };
            window.ShowDialog();
            return input;
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            var type = ((App) Application.Current).AvailableMissionTypes.First(type =>
                type.Name.ToLower().StartsWith((string) e.AddedItems[0]));


            var missionWrapper = (MissionWrapper)Activator.CreateInstance(type, (Mission)DataContext);

            TypeSpecificUserControl.Content = App.GenerateUIElement<MissionUserControl>(missionWrapper);
        }
    }
}
