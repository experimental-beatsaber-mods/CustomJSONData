namespace CustomJSONData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    public static class DictionaryExtensions
    {
        public static Dictionary<string, object?> FromJSON(string jsonString)
        {
            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            using JsonTextReader reader = new JsonTextReader(new StreamReader(jsonString));
            return FromJSON(reader);
        }

        public static Dictionary<string, object?> FromJSON(JsonReader reader, Func<string, bool>? specialCase = null)
        {
            Dictionary<string, object?> dictioanry = new Dictionary<string, object?>();
            reader.ReadToDictionary(dictioanry, specialCase);
            return dictioanry;
        }

        public static Dictionary<string, object?> Copy(this Dictionary<string, object?> dictionary)
        {
            return dictionary != null ? new Dictionary<string, object?>(dictionary) : new Dictionary<string, object?>();
        }

        public static T? Get<T>(this Dictionary<string, object?> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object? value))
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null)
                {
                    return (T)Convert.ChangeType(value, underlyingType);
                }
                else if (value is IConvertible)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    return (T?)value;
                }
            }

            return default;
        }

        public static T? GetStringToEnum<T>(this Dictionary<string, object?> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object? value) && value != null)
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null)
                {
                    return (T)Enum.Parse(underlyingType, (string)value);
                }
                else
                {
                    return (T)Enum.Parse(typeof(T), (string)value);
                }
            }

            return default;
        }
    }
}
