using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UniRmmz
{
    public static class ConvertEx
    {
        public static Vector4 ToVector4(object obj, Vector4 defaultValue = default)
        {
            if (obj == null || obj is not JArray array || array.Count < 4)
            {
                return defaultValue;
            }

            try
            {
                float x = array[0].Type == JTokenType.Integer || array[0].Type == JTokenType.Float
                    ? (float)array[0]
                    : 0f;

                float y = array[1].Type == JTokenType.Integer || array[1].Type == JTokenType.Float
                    ? (float)array[1]
                    : 0f;

                float z = array[2].Type == JTokenType.Integer || array[2].Type == JTokenType.Float
                    ? (float)array[2]
                    : 0f;

                float w = array[3].Type == JTokenType.Integer || array[3].Type == JTokenType.Float
                    ? (float)array[3]
                    : 0f;

                return new Vector4(x, y, z, w);
            }
            catch
            {
                return defaultValue;
            }
        }
        
        public static T[] ToArray<T>(object obj, T[] defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = Array.Empty<T>();
            }

            if (obj == null || obj is not JArray array)
            {
                return defaultValue;
            }

            try
            {
                T[] result = new T[array.Count];

                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i] != null)
                    {
                        result[i] = array[i].Value<T>();
                    }
                }

                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string[] ToStringArray(object obj, string[] defaultValue = null)
        {
            if (defaultValue == null)
            {
                defaultValue = Array.Empty<string>();
            }

            if (obj == null || obj is not JArray array)
            {
                return defaultValue;
            }

            try
            {
                string[] result = new string[array.Count];

                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i] != null)
                    {
                        result[i] = array[i].ToString();
                    }
                    else
                    {
                        result[i] = string.Empty;
                    }
                }

                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static MoveRoute ToMoveRoute(object obj)
        {
            var jObject = obj as JObject;
            if (jObject == null)
                return null;

            var moveRoute = new MoveRoute();

            // 基本プロパティの解析
            moveRoute.repeat = jObject.Value<bool>("repeat");
            moveRoute.skippable = jObject.Value<bool>("skippable");
            moveRoute.wait = jObject.Value<bool>("wait");
            moveRoute.list = new List<MoveCommand>();

            // listプロパティの解析
            var listToken = jObject["list"];
            if (listToken != null && listToken.Type == JTokenType.Array)
            {
                var listArray = (JArray)listToken;
                foreach (var item in listArray)
                {
                    var moveCommand = ToMoveCommand(item as JObject);
                    if (moveCommand != null)
                    {
                        moveRoute.List.Add(moveCommand);
                    }
                }
            }

            return moveRoute;
        }

        public static MoveCommand ToMoveCommand(object obj)
        {
            var jObject = obj as JObject;
            if (jObject == null)
                return null;

            var moveCommand = new MoveCommand();
            moveCommand.code = jObject.Value<int>("code");

            // parametersの解析
            var parametersToken = jObject["parameters"];
            if (parametersToken != null && parametersToken.Type == JTokenType.Array)
            {
                var parametersArray = (JArray)parametersToken;
                var parameters = new List<object>(parametersArray.Count);
            
                for (int i = 0; i < parametersArray.Count; i++)
                {
                    var token = parametersArray[i];
                    switch (token.Type)
                    {
                        case JTokenType.Integer:
                            parameters.Add((int)token);
                            break;
                        
                        case JTokenType.Float:
                            parameters.Add((float)token);
                            break;
                        
                        case JTokenType.String:
                            parameters.Add((string)token);
                            break;
                        
                        case JTokenType.Object:
                            parameters.Add((object)token);
                            break;
                    }
                    
                }
            
                moveCommand.parameters = parameters;
            }

            return moveCommand;
        }
        
        public static DataSystem.DataSound ToSoundData(object obj)
        {
            var jObject = obj as JObject;
            if (jObject == null)
                return null;

            var soundData = new DataSystem.DataSound();

            soundData.name = jObject.Value<string>("name");
            soundData.pan = jObject.Value<int>("pan");
            soundData.pitch = jObject.Value<int>("pitch");
            soundData.volume = jObject.Value<int>("volume");

            return soundData;
        }
    }
}