using System;
using System.IO;
using System.Linq;
using System.Reflection;
using launchpad.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace launchpad.JsonConverter
{
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
}