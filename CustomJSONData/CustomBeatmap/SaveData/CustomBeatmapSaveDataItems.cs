using System.Collections.Generic;
using BeatmapSaveDataVersion3;

namespace CustomJSONData.CustomBeatmap
{
    public partial class CustomBeatmapSaveData
    {
        public class CustomEventData : BeatmapSaveDataItem, ICustomData
        {
            internal CustomEventData(float beat, string type, Dictionary<string, object?> data)
                : base(beat)
            {
                this.type = type;
                customData = data;
            }

            public string type { get; }

            public Dictionary<string, object?> customData { get; }
        }

        public new class BasicEventData : BeatmapSaveData.BasicEventData, ICustomData
        {
            public BasicEventData(
                float beat,
                BeatmapSaveDataVersion2_6_0AndEarlier.BeatmapSaveData.BeatmapEventType eventType,
                int value,
                float floatValue,
                Dictionary<string, object?> customData)
                : base(beat, eventType, value, floatValue)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class ColorBoostEventData : BeatmapSaveData.ColorBoostEventData, ICustomData
        {
            public ColorBoostEventData(
                float beat,
                bool boost,
                Dictionary<string, object?> customData)
                : base(beat, boost)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class BpmChangeEventData : BeatmapSaveData.BpmChangeEventData, ICustomData
        {
            public BpmChangeEventData(
                float beat,
                float bpm,
                Dictionary<string, object?> customData)
                : base(beat, bpm)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class RotationEventData : BeatmapSaveData.RotationEventData, ICustomData
        {
            public RotationEventData(
                float beat,
                ExecutionTime executionTime,
                float rotation,
                Dictionary<string, object?> customData)
                : base(beat, executionTime, rotation)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class LightColorEventBoxGroup : BeatmapSaveData.LightColorEventBoxGroup, ICustomData
        {
            public LightColorEventBoxGroup(
                float beat,
                int groupId,
                List<LightColorEventBox> eventBoxes,
                Dictionary<string, object?> customData)
                : base(beat, groupId, eventBoxes)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class LightRotationEventBoxGroup : BeatmapSaveData.LightRotationEventBoxGroup, ICustomData
        {
            public LightRotationEventBoxGroup(
                float beat,
                int groupId,
                List<LightRotationEventBox> eventBoxes,
                Dictionary<string, object?> customData)
                : base(beat, groupId, eventBoxes)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class ColorNoteData : BeatmapSaveData.ColorNoteData, ICustomData
        {
            public ColorNoteData(
                float beat,
                int line,
                int layer,
                NoteColorType color,
                NoteCutDirection cutDirection,
                int angleOffset,
                Dictionary<string, object?> customData)
                : base(beat, line, layer, color, cutDirection, angleOffset)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class BombNoteData : BeatmapSaveData.BombNoteData, ICustomData
        {
            public BombNoteData(
                float beat,
                int line,
                int layer,
                Dictionary<string, object?> customData)
                : base(beat, line, layer)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class WaypointData : BeatmapSaveData.WaypointData, ICustomData
        {
            public WaypointData(
                float beat,
                int line,
                int layer,
                OffsetDirection offsetDirection,
                Dictionary<string, object?> customData)
                : base(beat, line, layer, offsetDirection)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class SliderData : BeatmapSaveData.SliderData, ICustomData
        {
            public SliderData(
                NoteColorType colorType,
                float headBeat,
                int headLine,
                int headLayer,
                float headControlPointLengthMultiplier,
                NoteCutDirection headCutDirection,
                float tailBeat,
                int tailLine,
                int tailLayer,
                float tailControlPointLengthMultiplier,
                NoteCutDirection tailCutDirection,
                SliderMidAnchorMode sliderMidAnchorMode,
                Dictionary<string, object?> customData)
                : base(
                    colorType,
                    headBeat,
                    headLine,
                    headLayer,
                    headControlPointLengthMultiplier,
                    headCutDirection,
                    tailBeat,
                    tailLine,
                    tailLayer,
                    tailControlPointLengthMultiplier,
                    tailCutDirection,
                    sliderMidAnchorMode)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class BurstSliderData : BeatmapSaveData.BurstSliderData, ICustomData
        {
            public BurstSliderData(
                NoteColorType colorType,
                float headBeat,
                int headLine,
                int headLayer,
                NoteCutDirection headCutDirection,
                float tailBeat,
                int tailLine,
                int tailLayer,
                int sliceCount,
                float squishAmount,
                Dictionary<string, object?> customData)
                : base(
                    colorType,
                    headBeat,
                    headLine,
                    headLayer,
                    headCutDirection,
                    tailBeat,
                    tailLine,
                    tailLayer,
                    sliceCount,
                    squishAmount)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class ObstacleData : BeatmapSaveData.ObstacleData, ICustomData
        {
            public ObstacleData(
                float beat,
                int line,
                int layer,
                float duration,
                int width,
                int height,
                Dictionary<string, object?> customData)
                : base(beat, line, layer, duration, width, height)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }
    }
}
