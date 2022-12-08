using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Utils
{
    public static class JsonUtils
    {
        public static T GetOrCreate<T>(WrongWarpMod mod, string modPath) where T : new()
        {
            var path = mod.ModHelper.Manifest.ModFolderPath + modPath;
            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new SafeStringEnumConverter()
                }
            };
            try
            {
                var json = File.ReadAllText(path);
                var result = JsonConvert.DeserializeObject<T>(json, jsonSettings);
                return result;
            } catch
            {
                var result = new T();
                var json = JsonConvert.SerializeObject(result, jsonSettings);
                File.WriteAllText(path, json);
                return result;
            }
        }

        public class SafeStringEnumConverter : StringEnumConverter
        {
            public SafeStringEnumConverter() : base(typeof(CamelCaseNamingStrategy)) { }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                try
                {
                    return base.ReadJson(reader, objectType, existingValue, serializer);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
