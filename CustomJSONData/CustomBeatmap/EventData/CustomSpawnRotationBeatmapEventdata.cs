using System.Collections.Generic;
using IPA.Utilities;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomSpawnRotationBeatmapEventdata : SpawnRotationBeatmapEventData, ICustomData
    {
        private static readonly FieldAccessor<SpawnRotationBeatmapEventData, SpawnRotationEventType>.Accessor _spawnRotationEventTypeAccessor =
            FieldAccessor<SpawnRotationBeatmapEventData, SpawnRotationEventType>.GetAccessor("spawnRotationEventType");

        private static readonly FieldAccessor<SpawnRotationBeatmapEventData, float>.Accessor _deltaRotationAccessor =
            FieldAccessor<SpawnRotationBeatmapEventData, float>.GetAccessor("_deltaRotation");

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
            SpawnRotationBeatmapEventData @this = this;
            return new CustomSpawnRotationBeatmapEventdata(
                time,
                _spawnRotationEventTypeAccessor(ref @this),
                _deltaRotationAccessor(ref @this),
                customData.Copy());
        }
    }
}
