using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using launchpad.Models;
using Grid = System.Windows.Controls.Grid;

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

    public abstract class MissionBasedUIGenerator<TMission> where TMission : Mission
    {
        public ModifierKeysButton GenerateButton(TMission mission)
        {
            var button = new ModifierKeysButton
            {
                Content = mission
            };
            Grid.SetColumn(button, (int)mission.position.X);
            Grid.SetRow(button, (int)mission.position.Y);
            button.CtrlClickEvent += (sender, args) =>
            {
                var windows = new Windows.EditMissionWindow();
                var missionGrid = GenerateUIElement<UserControl>(mission);
                Grid.SetColumn(missionGrid, 0);
                Grid.SetRow(missionGrid, 3);
                windows.Content.Children.Add(missionGrid);
                windows.ShowDialog();
            };
            return button;
        }

        private TElement GenerateUIElement<TElement>(TMission mission)
        {
            var elementTypeForMission = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract)
                .Where(myType => myType.BaseType != null)
                .FirstOrDefault(myType => myType.BaseType.GenericTypeArguments.Contains(mission.GetType()));
            if (elementTypeForMission == null)
            {
                throw new NotImplementedException($"No UI element factory exists for missions of type '{typeof(TMission).Name}'; make sure the class is a non-abstract class deriving of 'MissionBasedUIGenerator<TMission>'");
            }

            var objectForMission = Activator.CreateInstance(elementTypeForMission);
            if (objectForMission == null)
            {
                throw new NotImplementedException($"'{elementTypeForMission.Name}' has no parameterless constructor");
            }

            var generatorMethod = elementTypeForMission.GetMethods()
                .Where(method => method.ReturnType == typeof(TElement))
                .FirstOrDefault(method =>
                {
                    var parameters = method.GetParameters();
                    return parameters.Length == 1 && parameters.First().ParameterType == typeof(TMission);
                });
            if (generatorMethod == null)
            {
                throw new NotImplementedException($"Unable to find a generator method for UI element of type '{typeof(TElement).Name}' on '{elementTypeForMission.Name}'");
            }

            var uiElement = (TElement)generatorMethod.Invoke(objectForMission, new object[] { mission });
            return uiElement;
        }
    }
}