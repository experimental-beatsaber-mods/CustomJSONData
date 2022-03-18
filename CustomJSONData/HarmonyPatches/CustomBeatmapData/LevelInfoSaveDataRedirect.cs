using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelInfoSaveData))]
    internal static class LevelInfoSaveDataRedirect
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StandardLevelInfoSaveData.DeserializeFromJSONString))]
        private static bool Prefix(ref StandardLevelInfoSaveData __result, string stringData)
        {
            __result = CustomLevelInfoSaveData.Deserialize(stringData);

            return false;
        }
    }
}
