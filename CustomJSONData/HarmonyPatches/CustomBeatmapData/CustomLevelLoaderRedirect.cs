using System;
using System.Collections.Generic;
using System.IO;
using BeatmapSaveDataVersion3;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    // Create a CustomBeatmapSaveData instead of a BeatmapSaveData
    [HarmonyPatch(typeof(CustomLevelLoader))]
    internal static class CustomLevelLoaderRedirect
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomLevelLoader.LoadBeatmapDataBasicInfo))]
        private static bool Prefix(
            ref Tuple<BeatmapSaveData, BeatmapDataBasicInfo> __result,
            string customLevelPath,
            string difficultyFileName,
            StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            string path = Path.Combine(customLevelPath, difficultyFileName);
            if (!File.Exists(path))
            {
                return false;
            }

            Dictionary<string, object?> beatmapData;
            Dictionary<string, object?> levelData;
            if (standardLevelInfoSaveData is CustomLevelInfoSaveData customLevelInfoSaveData)
            {
                beatmapData = customLevelInfoSaveData.beatmapCustomDatasByFilename[difficultyFileName];
                levelData = customLevelInfoSaveData.customData;
            }
            else
            {
                beatmapData = new Dictionary<string, object?>();
                levelData = new Dictionary<string, object?>();
            }

            CustomBeatmapSaveData saveData = CustomBeatmapSaveData.Deserialize(path, beatmapData, levelData);

            __result = new Tuple<BeatmapSaveData, BeatmapDataBasicInfo>(
                saveData,
                BeatmapDataLoader.GetBeatmapDataBasicInfoFromSaveData(saveData));

            return false;
        }
    }
}
