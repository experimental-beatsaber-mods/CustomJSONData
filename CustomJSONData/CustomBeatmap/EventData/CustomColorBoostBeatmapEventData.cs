using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomColorBoostBeatmapEventData : ColorBoostBeatmapEventData, ICustomData
    {
        public CustomColorBoostBeatmapEventData(
            float time,
            bool boostColorsAreOn,
            Dictionary<string, object?> customData)
            : base(time, boostColorsAreOn)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomColorBoostBeatmapEventData(time, boostColorsAreOn, customData.Copy());
        }
    }
}
