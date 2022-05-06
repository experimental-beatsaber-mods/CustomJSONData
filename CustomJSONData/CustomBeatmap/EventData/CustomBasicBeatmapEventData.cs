namespace CustomJSONData.CustomBeatmap
{
    public class CustomBasicBeatmapEventData : BasicBeatmapEventData, ICustomData
    {
        public CustomBasicBeatmapEventData(
            float time,
            BasicBeatmapEventType basicBeatmapEventType,
            int value,
            float floatValue,
            CustomData customData)
            : base(time, basicBeatmapEventType, value, floatValue)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBasicBeatmapEventData(time, basicBeatmapEventType, value, floatValue, customData.Copy());
        }
    }
}
