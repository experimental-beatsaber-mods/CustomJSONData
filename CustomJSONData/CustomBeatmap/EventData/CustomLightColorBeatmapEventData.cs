using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomLightColorBeatmapEventData : LightColorBeatmapEventData, ICustomData
    {
        public CustomLightColorBeatmapEventData(
            float time,
            int groupId,
            int elementId,
            BeatmapEventTransitionType transitionType,
            EnvironmentColorType colorType,
            float brightness,
            int strobeBeatFrequency,
            Dictionary<string, object?> customData)
            : base(time, groupId, elementId, transitionType, colorType, brightness, strobeBeatFrequency)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomLightColorBeatmapEventData(time, groupId, elementId, transitionType, colorType, brightness, strobeBeatFrequency, customData.Copy());
        }
    }
}
