using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelInfoSaveData), nameof(StandardLevelInfoSaveData.DeserializeFromJSONString))]
    internal static class StandardLevelInfoSaveDataDeserializeFromJSONString
    {
        [UsedImplicitly]
        private static bool Prefix(ref StandardLevelInfoSaveData __result, string stringData)
        {
            __result = CustomLevelInfoSaveData.Deserialize(stringData);

            return false;
        }
    }
}
