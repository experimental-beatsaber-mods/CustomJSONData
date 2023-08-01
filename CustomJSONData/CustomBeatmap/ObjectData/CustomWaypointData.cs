namespace CustomJSONData.CustomBeatmap
{
    public class CustomWaypointData : WaypointData, ICustomData, IVersionable
    {
        public CustomWaypointData(float time, int lineIndex, NoteLineLayer noteLineLayer, OffsetDirection offsetDirection, CustomData customData, bool version260AndEarlier)
            : base(time, lineIndex, noteLineLayer, offsetDirection)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomWaypointData(time, lineIndex, lineLayer, offsetDirection, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
