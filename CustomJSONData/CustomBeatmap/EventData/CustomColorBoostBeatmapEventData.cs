namespace CustomJSONData.CustomBeatmap
{
    public class CustomColorBoostBeatmapEventData : ColorBoostBeatmapEventData, ICustomData, IVersionable
    {
        public CustomColorBoostBeatmapEventData(
            float time,
            bool boostColorsAreOn,
            CustomData customData,
            bool version260AndEarlier)
            : base(time, boostColorsAreOn)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomColorBoostBeatmapEventData(time, boostColorsAreOn, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
