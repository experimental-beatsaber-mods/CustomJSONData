using System.Collections.Generic;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomSliderData : SliderData, ICustomData
    {
        public CustomSliderData(
            Type sliderType,
            ColorType colorType,
            bool hasHeadNote,
            float headTime,
            int headLineIndex,
            NoteLineLayer headLineLayer,
            NoteLineLayer headBeforeJumpLineLayer,
            float headControlPointLengthMultiplier,
            NoteCutDirection headCutDirection,
            float headCutDirectionAngleOffset,
            bool hasTailNote,
            float tailTime,
            int tailLineIndex,
            NoteLineLayer tailLineLayer,
            NoteLineLayer tailBeforeJumpLineLayer,
            float tailControlPointLengthMultiplier,
            NoteCutDirection tailCutDirection,
            float tailCutDirectionAngleOffset,
            SliderMidAnchorMode midAnchorMode,
            int sliceCount,
            float squishAmount,
            Dictionary<string, object?> customData)
            : base(
                sliderType,
                colorType,
                hasHeadNote,
                headTime,
                headLineIndex,
                headLineLayer,
                headBeforeJumpLineLayer,
                headControlPointLengthMultiplier,
                headCutDirection,
                headCutDirectionAngleOffset,
                hasTailNote,
                tailTime,
                tailLineIndex,
                tailLineLayer,
                tailBeforeJumpLineLayer,
                tailControlPointLengthMultiplier,
                tailCutDirection,
                tailCutDirectionAngleOffset,
                midAnchorMode,
                sliceCount,
                squishAmount)
        {
            this.customData = customData;
        }

        public Dictionary<string, object?> customData { get; }

        public static SliderData CreateCustomSliderData(
            ColorType colorType,
            float headTime,
            int headLineIndex,
            NoteLineLayer headLineLayer,
            NoteLineLayer headBeforeJumpLineLayer,
            float headControlPointLengthMultiplier,
            NoteCutDirection headCutDirection,
            float tailTime,
            int tailLineIndex,
            NoteLineLayer tailLineLayer,
            NoteLineLayer tailBeforeJumpLineLayer,
            float tailControlPointLengthMultiplier,
            NoteCutDirection tailCutDirection,
            SliderMidAnchorMode midAnchorMode,
            Dictionary<string, object?> customData)
        {
            return new CustomSliderData(
                Type.Normal,
                colorType,
                false,
                headTime,
                headLineIndex,
                headLineLayer,
                headBeforeJumpLineLayer,
                headControlPointLengthMultiplier,
                headCutDirection,
                0,
                false,
                tailTime,
                tailLineIndex,
                tailLineLayer,
                tailBeforeJumpLineLayer,
                tailControlPointLengthMultiplier,
                tailCutDirection,
                0,
                midAnchorMode,
                0,
                1,
                customData);
        }

        public static SliderData CreateCustomBurstSliderData(
            ColorType colorType,
            float headTime,
            int headLineIndex,
            NoteLineLayer headLineLayer,
            NoteLineLayer headBeforeJumpLineLayer,
            NoteCutDirection headCutDirection,
            float tailTime,
            int tailLineIndex,
            NoteLineLayer tailLineLayer,
            NoteLineLayer tailBeforeJumpLineLayer,
            NoteCutDirection tailCutDirection,
            int sliceCount,
            float squishAmount,
            Dictionary<string, object?> customData)
        {
            return new CustomSliderData(
                Type.Burst,
                colorType,
                false,
                headTime,
                headLineIndex,
                headLineLayer,
                headBeforeJumpLineLayer,
                0,
                headCutDirection,
                0,
                false,
                tailTime,
                tailLineIndex,
                tailLineLayer,
                tailBeforeJumpLineLayer,
                0,
                tailCutDirection,
                0,
                SliderMidAnchorMode.Straight,
                sliceCount,
                squishAmount,
                customData);
        }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomSliderData(
                sliderType,
                colorType,
                hasHeadNote,
                time,
                headLineIndex,
                headLineLayer,
                headBeforeJumpLineLayer,
                headControlPointLengthMultiplier,
                headCutDirection,
                headCutDirectionAngleOffset,
                hasTailNote,
                tailTime,
                tailLineIndex,
                tailLineLayer,
                tailBeforeJumpLineLayer,
                tailControlPointLengthMultiplier,
                tailCutDirection,
                tailCutDirectionAngleOffset,
                midAnchorMode,
                sliceCount,
                squishAmount,
                customData.Copy());
        }
    }
}
