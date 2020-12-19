using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace launchpad
{

    public class PadConfig
    {
        public Grid grid { get; set; }
        public Mission[] commands { get; set; }
    }

    public class Grid
    {
        public Point dimensions { get; set; }
    }

    public class Mission
    {
        public Point position { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        [JsonIgnore]
        public Process currentProcess { get; set; }

        public override string ToString() => label;
    }

    public class CmdMission : Mission
    {
        public string command { get; set; }
        public CmdMission()
        {

        }
    }

    public class PowershellMission : Mission
    {
        public string command { get; set; }
        public PowershellMission()
        {

        }
    }


    public class MissionConverter : JsonCreationConverter<Mission>
    {
        public static Type GetMissionByName(string typeName)
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(myType => myType.Name.ToLower() == $"{typeName.ToLower()}mission")
                .Where(myType => myType.IsClass)
                .Where(myType => !myType.IsAbstract)
                .FirstOrDefault(myType => myType.IsSubclassOf(typeof(Mission)));
        }

        protected override Mission Create(Type objectType, JObject jObject)
        {
            if (jObject["type"] == null)
            {
                throw new InvalidDataException("No type field!");
            }

            var typeValue = jObject["type"].Value<string>();
            var missionType = GetMissionByName(typeValue);
            if (missionType == null)
            {
                throw new NotSupportedException("Type " + typeValue + " is not supported");
            }
            return (Mission)jObject.ToObject(missionType);
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }
    }
}
