namespace CustomJSONData.CustomBeatmap
{
    public class CustomWaypointData : WaypointData, ICustomData
    {
        public CustomWaypointData(float time, int lineIndex, NoteLineLayer noteLineLayer, OffsetDirection offsetDirection, CustomData customData)
            : base(time, lineIndex, noteLineLayer, offsetDirection)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomWaypointData(time, lineIndex, lineLayer, offsetDirection, customData.Copy());
        }
    }
}
