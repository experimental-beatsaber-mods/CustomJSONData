namespace CustomJSONData.CustomBeatmap
{
    public class CustomNoteData : NoteData, ICustomData, IVersionable
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
            CustomData customData,
            bool version260AndEarlier)
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
            version2_6_0AndEarlier = version260AndEarlier;
        }

        public CustomData customData { get; }

        public bool version2_6_0AndEarlier { get; }

        public static CustomNoteData CreateCustomBombNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            CustomData customData,
            bool version260AndEarlier)
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
                customData,
                version260AndEarlier);
        }

        public static CustomNoteData CreateCustomBasicNoteData(
            float time,
            int lineIndex,
            NoteLineLayer noteLineLayer,
            ColorType colorType,
            NoteCutDirection cutDirection,
            CustomData customData,
            bool version260AndEarlier)
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
                customData,
                version260AndEarlier);
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
                customData,
                false);
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
                customData.Copy(),
                version2_6_0AndEarlier);
        }
    }
}
