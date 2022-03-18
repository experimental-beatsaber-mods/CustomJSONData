namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomWaypointData : WaypointData, ICustomData
    {
        public CustomWaypointData(float time, int lineIndex, NoteLineLayer noteLineLayer, OffsetDirection offsetDirection, Dictionary<string, object?> customData)
            : base(time, lineIndex, noteLineLayer, offsetDirection)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomWaypointData(time, lineIndex, lineLayer, offsetDirection, customData.Copy());
        }
    }
}
