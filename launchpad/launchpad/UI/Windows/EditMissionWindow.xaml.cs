using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Biaui.Controls;

namespace launchpad.UI.Windows
{
    /// <summary>
    /// Interaction logic for EditMissionWindow.xaml
    /// </summary>
    public partial class EditMissionWindow : BiaWindow
    {
        public EditMissionWindow()
        {
            InitializeComponent();
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
