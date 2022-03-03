using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IPA.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomJSONData
{
    public static class DictionaryExtensions
    {
        [PublicAPI]
        public static Dictionary<string, object?> FromJSON(string jsonString)
        {
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(jsonString));
            using JsonTextReader reader = new(new StreamReader(jsonString));
            return FromJSON(reader);
        }

        public static Dictionary<string, object?> FromJSON(JsonReader reader, Func<string, bool>? specialCase = null)
        {
            Dictionary<string, object?> dictioanry = new();
            reader.ReadToDictionary(dictioanry, specialCase);
            return dictioanry;
        }

        public static Dictionary<string, object?> Copy(this Dictionary<string, object?>? dictionary)
        {
            return dictionary != null ? new Dictionary<string, object?>(dictionary) : new Dictionary<string, object?>();
        }

        public static T? Get<T>(this Dictionary<string, object?> dictionary, string key)
        {
            static bool IsNumericType(object o)
            {
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                    default:
                        return false;
                }
            }

            // trygetvalue missing [notnullwhen] attribute :(
            if (!dictionary.TryGetValue(key, out object? value))
            {
                return default;
            }

            if (value == null)
            {
                return default;
            }

            Type resultType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (IsNumericType(value!))
            {
                return (T)Convert.ChangeType(value, resultType);
            }

            return (T?)value;
        }

        [PublicAPI]
        public static Vector3? GetVector3(this Dictionary<string, object?> dictionary, string key)
        {
            List<float>? data = dictionary.Get<List<object>>(key)?.Select(Convert.ToSingle).ToList();
            Vector3? final = null;
            if (data != null)
            {
                final = new Vector3(data[0], data[1], data[2]);
            }

            return final;
        }

        [PublicAPI]
        public static T? GetStringToEnum<T>(this Dictionary<string, object?> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out object? value) || value == null)
            {
                return default;
            }

            Type? underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType != null)
            {
                return (T)Enum.Parse(underlyingType, (string)value);
            }

            return (T)Enum.Parse(typeof(T), (string)value);
        }

        [PublicAPI]
        public static string Stringify(this Dictionary<string, object?> dictionary)
        {
            return FormatDictionary(dictionary);
        }

        public static string FormatDictionary(Dictionary<string, object?> dictionary, int indent = 0)
        {
            string prefix = new('\t', indent);
            StringBuilder builder = new();
            builder.AppendLine(prefix + "{");
            foreach ((string key, object? value) in dictionary)
            {
                builder.AppendLine($"{prefix}\t\"{key}\": {FormatObject(value, indent + 1)}");
            }

            builder.Append(prefix + "}");
            return builder.ToString();
        }

        public static string FormatList(List<object?> list, int indent = 0)
        {
            return "[" + string.Join(", ", list.Select(n => FormatObject(n, indent))) + "]";
        }

        public static string FormatObject(object? obj, int indent = 0)
        {
            return obj switch
            {
                List<object?> recursiveList => FormatList(recursiveList, indent),
                Dictionary<string, object?> dictionary => FormatDictionary(dictionary, indent),
                _ => obj?.ToString() ?? "NULL"
            };
        }
    }
}
