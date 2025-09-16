using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace UniRmmz
{
    /// <summary>
    /// The static class that handles JSON with object information.
    /// </summary>
    public static partial class JsonEx
    {
        private class UniRmmzContractResolver : DefaultContractResolver
        {
            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                var contract = base.CreateObjectContract(objectType);

                contract.DefaultCreatorNonPublic = true;
                contract.DefaultCreator = () =>
                {
                    return FormatterServices.GetUninitializedObject(objectType);
                };

                return contract;
            }
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                // フィールドのみを対象とする
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => !f.IsInitOnly && !f.IsLiteral) // readonly/constを除外
                    .Select(f => CreateProperty(f, memberSerialization))
                    .ToList();
        
                fields.ForEach(p => { 
                    p.Writable = true; 
                    p.Readable = true; 
                });
        
                return fields;
            }
        }
        
        private class ObjectArrayConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(object[]);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JArray array = JArray.Load(reader);
                object[] parameters = new object[array.Count];

                for (int i = 0; i < array.Count; i++)
                {
                    JToken token = array[i];
                    if (token.Type == JTokenType.Object)
                    {
                        parameters[i] = token.ToObject<object>(serializer);
                    }
                    else
                    {
                        parameters[i] = token.ToObject<object>();
                    }
                }

                return parameters;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        
        private static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new UniRmmzContractResolver(),
            Converters = new List<JsonConverter> { new ObjectArrayConverter() },
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        };

        /// <summary>
        /// Converts an object to a JSON string with object information.
        /// </summary>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>The JSON string.</returns>
        public static string Stringify<T>(T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj, s_settings);
        }

        /// <summary>
        /// Parses a JSON string and reconstructs the corresponding object.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The reconstructed object.</returns>
        public static T Parse<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json, s_settings);
        }
    
        /// <summary>
        /// Makes a deep copy of the specified object.
        /// </summary>
        /// <param name="obj">The object to be copied.</param>
        /// <returns>The copied object.</returns>
        public static T MakeDeepCopy<T>(T obj) where T : class
        {
            var json = Stringify(obj);
            return Parse<T>(json);
        }
    }
}