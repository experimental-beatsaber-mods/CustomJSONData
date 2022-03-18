namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;

    public class CustomObstacleData : ObstacleData, ICustomData
    {
        public CustomObstacleData(float time, int lineIndex, NoteLineLayer lineLayer, float duration, int width, int height, Dictionary<string, object?> customData)
            : base(time, lineIndex, lineLayer, duration, width, height)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomObstacleData(time, lineIndex, lineLayer, duration, width, height, customData.Copy());
        }
    }
}
