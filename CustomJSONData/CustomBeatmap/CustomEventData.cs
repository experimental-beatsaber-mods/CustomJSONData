namespace CustomJSONData.CustomBeatmap
{
    public class CustomEventData : BeatmapDataItem, ICustomData
    {
        public CustomEventData(float time, string type, CustomData data)
            : base(time, 0, 0, (BeatmapDataItemType)2)
        {
            eventType = type;
            customData = data;
        }

        public string eventType { get; }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomEventData(time, eventType, customData.Copy());
        }
    }
}
