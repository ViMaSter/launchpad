using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace launchpad.UI.Generator
{
    public class ModifierKeysButton : Button
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