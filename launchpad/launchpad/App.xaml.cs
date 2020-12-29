using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using launchpad.Models;
using launchpad.ModelWrapper;
using launchpad.UI.Generator;

namespace launchpad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IEnumerable<Type> AvailableMissionTypes { get; }
        public App()
        {
            AvailableMissionTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract)
                .Where(myType => myType.BaseType == typeof(MissionWrapper));
        }

        public static MissionWrapper WrapMission(Mission mission)
        {
            var elementTypeForMission = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract)
                .Where(myType => myType.BaseType == typeof(MissionWrapper))
                .First(type => type.Name.ToLower().StartsWith(mission.type.ToLower()));

            return (MissionWrapper)Activator.CreateInstance(elementTypeForMission, mission);
        }

        public static TElement GenerateUIElement<TElement>(MissionWrapper missionWrapper)
        {
            var elementTypeForMission = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract)
                .Where(myType => myType.BaseType != null)
                .Where(myType => myType.BaseType.GUID == typeof(MissionBasedUIGenerator<>).GUID)
                .FirstOrDefault(myType => myType.BaseType.GenericTypeArguments.Contains(missionWrapper.GetType()));

            if (elementTypeForMission == null)
            {
                throw new NotImplementedException($"No UI element factory exists for missions of type '{missionWrapper.GetType().Name}'; make sure the class is a non-abstract class deriving of 'MissionBasedUIGenerator<TMission>'");
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
                    return parameters.Length == 1 && parameters.First().ParameterType == missionWrapper.GetType();
                });
            if (generatorMethod == null)
            {
                throw new NotImplementedException($"Unable to find a generator method for UI element of type '{typeof(TElement).Name}' on '{elementTypeForMission.Name}'");
            }

            var uiElement = (TElement)generatorMethod.Invoke(objectForMission, new object[] { missionWrapper });
            return uiElement;
        }
    }
}
