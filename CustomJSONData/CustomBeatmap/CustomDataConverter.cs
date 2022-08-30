using System;
using Newtonsoft.Json;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomDataConverter : JsonConverter<CustomData>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, CustomData? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override CustomData ReadJson(JsonReader reader, Type objectType, CustomData? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return CustomData.FromJSON(reader);
        }
    }
}
