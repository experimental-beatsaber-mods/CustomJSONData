using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomEventData
    {
        public CustomEventData(float time, string type, Dictionary<string, object?> data)
        {
            this.time = time;
            this.type = type;
            this.data = data;
        }

        public string type { get; }

        public float time { get; }

        public Dictionary<string, object?> data { get; }

        public CustomEventData GetCopy()
        {
            return new CustomEventData(time, type, data.Copy());
        }
    }
}
