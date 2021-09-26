namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomBeatmapEventData : BeatmapEventData
    {
        private CustomBeatmapEventData(float time, BeatmapEventType type, int value, float floatValue, Dictionary<string, object?> customData)
            : base(time, type, value, floatValue)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public CustomBeatmapEventData GetCopy()
        {
            return new CustomBeatmapEventData(time, type, value, floatValue, customData.Copy());
        }
    }
}
