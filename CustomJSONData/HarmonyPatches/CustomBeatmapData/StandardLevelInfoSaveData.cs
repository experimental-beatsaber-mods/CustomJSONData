using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(StandardLevelInfoSaveData))]
    [HarmonyPatch("DeserializeFromJSONString")]
    internal class StandardLevelInfoSaveDataDeserializeFromJSONString
    {
        [UsedImplicitly]
        private static bool Prefix(ref StandardLevelInfoSaveData __result, string stringData)
        {
            __result = CustomLevelInfoSaveData.Deserialize(stringData);

            return false;
        }
    }
}
