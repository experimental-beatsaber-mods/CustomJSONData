namespace CustomJSONData.CustomBeatmap
{
    public class CustomObstacleData : ObstacleData, ICustomData
    {
        public CustomObstacleData(float time, int lineIndex, NoteLineLayer lineLayer, float duration, int width, int height, CustomData customData)
            : base(time, lineIndex, lineLayer, duration, width, height)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomObstacleData(time, lineIndex, lineLayer, duration, width, height, customData.Copy());
        }
    }
}
