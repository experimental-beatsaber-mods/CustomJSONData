namespace CustomJSONData.CustomBeatmap
{
    public class CustomColorBoostBeatmapEventData : ColorBoostBeatmapEventData, ICustomData
    {
        public CustomColorBoostBeatmapEventData(
            float time,
            bool boostColorsAreOn,
            CustomData customData)
            : base(time, boostColorsAreOn)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomColorBoostBeatmapEventData(time, boostColorsAreOn, customData.Copy());
        }
    }
}
