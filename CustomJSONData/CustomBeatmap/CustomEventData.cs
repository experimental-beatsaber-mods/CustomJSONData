namespace CustomJSONData.CustomBeatmap
{
    public class CustomEventData : BeatmapDataItem, ICustomData
    {
        // TODO: delete this at some point
        public CustomEventData(float time, string type, CustomData data)
            : this(time, type, data, false)
        {
        }

        public CustomEventData(float time, string type, CustomData data, bool version260AndEarlier)
            : base(time, 0, 0, (BeatmapDataItemType)2)
        {
            eventType = type;
            customData = data;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public string eventType { get; }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomEventData(time, eventType, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
