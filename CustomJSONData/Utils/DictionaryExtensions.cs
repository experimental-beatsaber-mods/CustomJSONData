using CustomJSONData.CustomBeatmap;
using JetBrains.Annotations;

namespace CustomJSONData
{
    public static class DictionaryExtensions
    {
        [PublicAPI]
        public static CustomBeatmapSaveData? GetBeatmapSaveData(this IDifficultyBeatmap difficultyBeatmap)
        {
            return difficultyBeatmap is CustomDifficultyBeatmap { beatmapSaveData: CustomBeatmapSaveData customBeatmapSaveData }
                ? customBeatmapSaveData : null;
        }

        [PublicAPI]
        public static CustomData GetBeatmapCustomData(this IDifficultyBeatmap difficultyBeatmap)
        {
            return difficultyBeatmap is CustomDifficultyBeatmap { beatmapSaveData: CustomBeatmapSaveData customBeatmapSaveData }
                ? customBeatmapSaveData.beatmapCustomData : new CustomData();
        }

        [PublicAPI]
        public static CustomData GetLevelCustomData(this IDifficultyBeatmap difficultyBeatmap)
        {
            return difficultyBeatmap is CustomDifficultyBeatmap { beatmapSaveData: CustomBeatmapSaveData customBeatmapSaveData }
                ? customBeatmapSaveData.levelCustomData : new CustomData();
        }
    }
}
