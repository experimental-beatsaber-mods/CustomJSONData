using System.Collections.Generic;
using BeatmapSaveDataVersion3;

namespace CustomJSONData.CustomBeatmap
{
    public partial class CustomBeatmapSaveData
    {
        public class CustomEventData : BeatmapSaveDataItem, ICustomData, IVersionable
        {
            internal CustomEventData(float beat, string type, CustomData data, bool version260AndEarlier = false)
                : base(beat)
            {
                this.type = type;
                customData = data;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public string type { get; }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class BasicEventData : BeatmapSaveData.BasicEventData, ICustomData, IVersionable
        {
            public BasicEventData(
                float beat,
                BeatmapSaveDataVersion2_6_0AndEarlier.BeatmapSaveData.BeatmapEventType eventType,
                int value,
                float floatValue,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, eventType, value, floatValue)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class ColorBoostEventData : BeatmapSaveData.ColorBoostEventData, ICustomData, IVersionable
        {
            public ColorBoostEventData(
                float beat,
                bool boost,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, boost)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class BpmChangeEventData : BeatmapSaveData.BpmChangeEventData, ICustomData, IVersionable
        {
            public BpmChangeEventData(
                float beat,
                float bpm,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, bpm)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class RotationEventData : BeatmapSaveData.RotationEventData, ICustomData, IVersionable
        {
            public RotationEventData(
                float beat,
                ExecutionTime executionTime,
                float rotation,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, executionTime, rotation)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class LightColorEventBoxGroup : BeatmapSaveData.LightColorEventBoxGroup, ICustomData
        {
            public LightColorEventBoxGroup(
                float beat,
                int groupId,
                List<LightColorEventBox> eventBoxes,
                CustomData customData)
                : base(beat, groupId, eventBoxes)
            {
                this.customData = customData;
            }

            public CustomData customData { get; }
        }

        public new class LightRotationEventBoxGroup : BeatmapSaveData.LightRotationEventBoxGroup, ICustomData
        {
            public LightRotationEventBoxGroup(
                float beat,
                int groupId,
                List<LightRotationEventBox> eventBoxes,
                CustomData customData)
                : base(beat, groupId, eventBoxes)
            {
                this.customData = customData;
            }

            public CustomData customData { get; }
        }

        public new class ColorNoteData : BeatmapSaveData.ColorNoteData, ICustomData, IVersionable
        {
            public ColorNoteData(
                float beat,
                int line,
                int layer,
                NoteColorType color,
                NoteCutDirection cutDirection,
                int angleOffset,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, line, layer, color, cutDirection, angleOffset)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class BombNoteData : BeatmapSaveData.BombNoteData, ICustomData, IVersionable
        {
            public BombNoteData(
                float beat,
                int line,
                int layer,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, line, layer)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class WaypointData : BeatmapSaveData.WaypointData, ICustomData, IVersionable
        {
            public WaypointData(
                float beat,
                int line,
                int layer,
                OffsetDirection offsetDirection,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, line, layer, offsetDirection)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }

        public new class SliderData : BeatmapSaveData.SliderData, ICustomData, IVersionable
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
                CustomData customData,
                bool version260AndEarlier = false)
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
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
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
                CustomData customData)
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

            public CustomData customData { get; }
        }

        public new class ObstacleData : BeatmapSaveData.ObstacleData, ICustomData, IVersionable
        {
            public ObstacleData(
                float beat,
                int line,
                int layer,
                float duration,
                int width,
                int height,
                CustomData customData,
                bool version260AndEarlier = false)
                : base(beat, line, layer, duration, width, height)
            {
                this.customData = customData;
                version2_6_0AndEarlier = version260AndEarlier;
            }

            public CustomData customData { get; }

            public bool version2_6_0AndEarlier { get; }
        }
    }
}
