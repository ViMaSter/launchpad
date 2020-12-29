using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            var newMission = (Mission)input.mission.Clone();
            var window = new EditMissionWindow
            {
                DataContext = newMission,
                TypeSpecificUserControl = {Content = App.GenerateUIElement<MissionUserControl>(input)}
            };
            window.ShowDialog();
            if (window.DialogResult.HasValue && window.DialogResult.Value)
            {
                return (TMissionWrapper)App.WrapMission(newMission);
            }

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

        private void EditMissionWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnOKClick(sender, null);
                return;
            }

            if (e.Key == Key.Escape)
            {
                OnCancelClick(sender, null);
                return;
            }
        }
    }
}
