using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomSpawnRotationBeatmapEventdata : SpawnRotationBeatmapEventData, ICustomData
    {
        public CustomSpawnRotationBeatmapEventdata(
            float time,
            SpawnRotationEventType spawnRotationEventType,
            float deltaRotation,
            Dictionary<string, object?> customData)
            : base(time, spawnRotationEventType, deltaRotation)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomSpawnRotationBeatmapEventdata(time, spawnRotationEventType, _deltaRotation, customData.Copy());
        }
    }
}
