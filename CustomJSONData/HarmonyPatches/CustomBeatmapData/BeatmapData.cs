using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapData))]
    [HarmonyPatch("CopyBeatmapEvents")]
    internal class BeatmapDataCopyBeatmapEvents
    {
        [UsedImplicitly]
        private static bool Prefix(IReadonlyBeatmapData src, BeatmapData dst)
        {
            foreach (BeatmapEventData beatmapEventData in src.beatmapEventsData)
            {
                if (beatmapEventData is CustomBeatmapEventData customBeatmapEventData)
                {
                    dst.AddBeatmapEventData(customBeatmapEventData.GetCopy());
                }
                else
                {
                    dst.AddBeatmapEventData(beatmapEventData);
                }
            }

            return false;
        }
    }
}
