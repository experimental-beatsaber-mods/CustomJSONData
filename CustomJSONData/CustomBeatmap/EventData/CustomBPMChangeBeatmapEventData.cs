using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBPMChangeBeatmapEventData : BPMChangeBeatmapEventData, ICustomData
    {
        public CustomBPMChangeBeatmapEventData(
            float time,
            float bpm,
            Dictionary<string, object?> customData)
            : base(time, bpm)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBPMChangeBeatmapEventData(time, bpm, customData.Copy());
        }
    }
}
