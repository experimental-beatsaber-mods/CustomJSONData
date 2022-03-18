using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBasicBeatmapEventData : BasicBeatmapEventData, ICustomData
    {
        public CustomBasicBeatmapEventData(
            float time,
            BasicBeatmapEventType basicBeatmapEventType,
            int value,
            float floatValue,
            Dictionary<string, object?> customData)
            : base(time, basicBeatmapEventType, value, floatValue)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBasicBeatmapEventData(time, basicBeatmapEventType, value, floatValue, customData.Copy());
        }
    }
}
