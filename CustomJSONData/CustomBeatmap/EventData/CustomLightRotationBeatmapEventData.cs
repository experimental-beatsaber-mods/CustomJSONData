using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomLightRotationBeatmapEventData : LightRotationBeatmapEventData, ICustomData
    {
        public CustomLightRotationBeatmapEventData(
            float time,
            int groupId,
            int elementId,
            bool usePreviousEventValue,
            EaseType easeType,
            Axis axis,
            float rotation,
            int loopCount,
            LightRotationDirection rotationDirection,
            Dictionary<string, object?> customData)
            : base(time, groupId, elementId, usePreviousEventValue, easeType, axis, rotation, loopCount, rotationDirection)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomLightRotationBeatmapEventData(
                time,
                groupId,
                elementId,
                usePreviousEventValue,
                easeType,
                axis,
                rotation,
                loopCount,
                rotationDirection,
                customData.Copy());
        }
    }
}
