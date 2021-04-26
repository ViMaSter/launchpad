using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace launchpad.UI.Buttons
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ModifierKeysButton : Button
    {
        public EventHandler CtrlClickEvent { get; set; }
        public EventHandler UnmodifiedClickEvent { get; set; }

        public ModifierKeysButton()
        {
            this.Click += (sender, args) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    CtrlClickEvent(sender, args);
                    return;
                }

                UnmodifiedClickEvent(sender, args);
            };
        }
    }
}
