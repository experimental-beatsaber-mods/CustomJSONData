using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CustomJSONData
{
    public static class JsonExtensions
    {
        public static bool? ReadIntAsBoolean(this JsonReader reader)
        {
            int? result = reader.ReadAsInt32();
            return result.HasValue ? result.Value > 0 : null;
        }

        public static void ReadToDictionary(
            this JsonReader reader,
            Dictionary<string, object?> dictionary,
            [InstantHandle] Func<string, bool>? specialCase = null)
        {
            ObjectReadObject(reader, dictionary, specialCase);
        }

        public static void ReadObject(this JsonReader reader, [InstantHandle] Action<string> action)
        {
            reader.Read();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                action((string?)reader.Value ?? string.Empty);

                reader.Read();
            }
        }

        public static void ReadObjectArray(this JsonReader reader, [InstantHandle] Action action)
        {
            reader.Read(); // StartArray
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException("Was not array.");
            }

            reader.Read(); // StartObject (hopefully)

            while (reader.TokenType == JsonToken.StartObject)
            {
                action();
                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new JsonSerializationException("Unexpected end when reading array.");
            }
        }

        private static object? ObjectReadValue(JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonToken.StartObject => ObjectReadObject(reader),
                JsonToken.StartArray => ObjectReadList(reader),
                _ => reader.Value
            };
        }

        private static IList<object?> ObjectReadList(JsonReader reader)
        {
            IList<object?> list = new List<object?>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;

                    default:
                        list.Add(ObjectReadValue(reader));
                        break;

                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
        }

        private static object ObjectReadObject(JsonReader reader, Dictionary<string, object?>? dictionary = null, Func<string, bool>? specialCase = null)
        {
            dictionary ??= new Dictionary<string, object?>();

            while (reader.Read())
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        string propertyName = (string?)reader.Value ?? string.Empty;

                        if (specialCase != null && !specialCase(propertyName))
                        {
                            break;
                        }

                        if (!reader.Read())
                        {
                            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
                        }

                        dictionary[propertyName] = ObjectReadValue(reader);

                        break;

                    case JsonToken.Comment:
                        break;

                    case JsonToken.EndObject:
                        return dictionary;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading Dictionary.");
        }
    }
}
