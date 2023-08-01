namespace CustomJSONData.CustomBeatmap
{
    public class CustomObstacleData : ObstacleData, ICustomData, IVersionable
    {
        public CustomObstacleData(float time, int lineIndex, NoteLineLayer lineLayer, float duration, int width, int height, CustomData customData, bool version260AndEarlier)
            : base(time, lineIndex, lineLayer, duration, width, height)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomObstacleData(time, lineIndex, lineLayer, duration, width, height, customData.Copy(), version2_6_0AndEarlier);
        }
    }
}
