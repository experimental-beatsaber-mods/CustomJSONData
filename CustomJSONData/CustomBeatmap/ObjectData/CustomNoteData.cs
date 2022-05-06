namespace CustomJSONData.CustomBeatmap
{
    public class CustomNoteData : NoteData, ICustomData
    {
        public CustomNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            NoteLineLayer beforeJumpNoteLineLayer,
            GameplayType gameplayType,
            ScoringType scoringType,
            ColorType colorType,
            NoteCutDirection cutDirection,
            float timeToNextColorNote,
            float timeToPrevColorNote,
            int flipLineIndex,
            float flipYSide,
            float cutDirectionAngleOffset,
            float cutSfxVolumeMultiplier,
            CustomData customData)
                       : base(
                             time,
                             lineIndex,
                             noteLineLayer,
                             beforeJumpNoteLineLayer,
                             gameplayType,
                             scoringType,
                             colorType,
                             cutDirection,
                             timeToNextColorNote,
                             timeToPrevColorNote,
                             flipLineIndex,
                             flipYSide,
                             cutDirectionAngleOffset,
                             cutSfxVolumeMultiplier)
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public static CustomNoteData CreateCustomBombNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            CustomData customData)
        {
            return new CustomNoteData(
                time,
                lineIndex,
                noteLineLayer,
                noteLineLayer,
                GameplayType.Bomb,
                ScoringType.NoScore,
                ColorType.None,
                NoteCutDirection.None,
                0f,
                0f,
                lineIndex,
                0f,
                0f,
                1f,
                customData);
        }

        public static CustomNoteData CreateCustomBasicNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            ColorType colorType,
            NoteCutDirection cutDirection,
            CustomData customData)
        {
            return new CustomNoteData(
                time,
                lineIndex,
                noteLineLayer,
                noteLineLayer,
                GameplayType.Normal,
                ScoringType.Normal,
                colorType,
                cutDirection,
                0f,
                0f,
                lineIndex,
                0f,
                0f,
                1f,
                customData);
        }

        public static CustomNoteData CreateCustomBurstSliderNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            NoteLineLayer beforeJumpNoteLineLayer,
            ColorType colorType,
            NoteCutDirection cutDirection,
            float cutSfxVolumeMultiplier,
            CustomData customData)
        {
            return new CustomNoteData(
                time,
                lineIndex,
                noteLineLayer,
                beforeJumpNoteLineLayer,
                GameplayType.BurstSliderElement,
                ScoringType.BurstSliderElement,
                colorType,
                cutDirection,
                0f,
                0f,
                lineIndex,
                0f,
                0f,
                cutSfxVolumeMultiplier,
                customData);
        }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomNoteData(
                time,
                lineIndex,
                noteLineLayer,
                beforeJumpNoteLineLayer,
                gameplayType,
                scoringType,
                colorType,
                cutDirection,
                timeToNextColorNote,
                timeToPrevColorNote,
                flipLineIndex,
                flipYSide,
                cutDirectionAngleOffset,
                cutSfxVolumeMultiplier,
                customData.Copy());
        }
    }
}
