namespace CustomJSONData.CustomBeatmap
{
    public class CustomBasicBeatmapEventData : BasicBeatmapEventData, ICustomData, IVersionable
    {
        // TODO: delete this at some point
        public CustomBasicBeatmapEventData(
            float time,
            BasicBeatmapEventType basicBeatmapEventType,
            int value,
            float floatValue,
            CustomData customData)
            : this(time, basicBeatmapEventType, value, floatValue, customData, false)
        {
        }

        public CustomBasicBeatmapEventData(
            float time,
            BasicBeatmapEventType basicBeatmapEventType,
            int value,
            float floatValue,
            CustomData customData,
            bool version260AndEarlier)
            : base(time, basicBeatmapEventType, value, floatValue)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomBasicBeatmapEventData(time, basicBeatmapEventType, value, floatValue, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
