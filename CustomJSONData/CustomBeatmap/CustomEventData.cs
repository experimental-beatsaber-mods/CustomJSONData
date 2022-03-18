using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomEventData : BeatmapDataItem, ICustomData
    {
        public CustomEventData(float time, string type, Dictionary<string, object?> data)
            : base(time, 0, 0, (BeatmapDataItemType)2)
        {
            eventType = type;
            customData = data;
        }

        public string eventType { get; }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomEventData(time, eventType, customData.Copy());
        }
    }
}
