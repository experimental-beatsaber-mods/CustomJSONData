using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectCallbackController))]
    [HarmonyPatch("Start")]
    internal class BeatmapObjectCallbackControllerStart
    {
        [UsedImplicitly]
        private static void Postfix(BeatmapObjectCallbackController __instance, IReadonlyBeatmapData ____beatmapData)
        {
            if (____beatmapData is CustomBeatmap.CustomBeatmapData)
            {
                __instance.gameObject.AddComponent<CustomEventCallbackController>().Init(__instance, ____beatmapData);
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapObjectCallbackController))]
    [HarmonyPatch("SetNewBeatmapData")]
    internal class BeatmapObjectCallbackControllerSetNewBeatmapData
    {
        [UsedImplicitly]
        private static void Postfix(BeatmapObjectCallbackController __instance, IReadonlyBeatmapData beatmapData)
        {
            __instance.GetComponent<CustomEventCallbackController>()?.SetNewBeatmapData(beatmapData);
        }
    }
}
