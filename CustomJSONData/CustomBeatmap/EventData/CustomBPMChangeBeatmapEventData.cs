namespace CustomJSONData.CustomBeatmap
{
    public class CustomBPMChangeBeatmapEventData : BPMChangeBeatmapEventData, ICustomData
    {
        public CustomBPMChangeBeatmapEventData(
            float time,
            float bpm,
            CustomData customData)
            : base(time, bpm)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBPMChangeBeatmapEventData(time, bpm, customData.Copy());
        }
    }
}
