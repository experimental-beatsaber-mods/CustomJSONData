namespace CustomJSONData.CustomBeatmap
{
    public class CustomSpawnRotationBeatmapEventdata : SpawnRotationBeatmapEventData, ICustomData, IVersionable
    {
        public CustomSpawnRotationBeatmapEventdata(
            float time,
            SpawnRotationEventType spawnRotationEventType,
            float deltaRotation,
            CustomData customData,
            bool version260AndEarlier)
            : base(time, spawnRotationEventType, deltaRotation)
        {
            this.customData = customData;
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomSpawnRotationBeatmapEventdata(
                time,
                spawnRotationEventType,
                _deltaRotation,
                customData.Copy(),
                version2_6_0AndEarlier);
        }
    }
}
