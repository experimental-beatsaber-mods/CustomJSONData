namespace CustomJSONData.CustomBeatmap
{
    public class CustomBPMChangeBeatmapEventData : BPMChangeBeatmapEventData, ICustomData, IVersionable
    {
        public CustomBPMChangeBeatmapEventData(
            float time,
            float bpm,
            CustomData customData,
            bool version260AndEarlier)
            : base(time, bpm)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBPMChangeBeatmapEventData(time, bpm, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
