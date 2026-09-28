#if !PRE_V1_37_1
using CustomJSONData.CustomBeatmap;
using HarmonyLib;
#if !PRE_V1_39_1
using System.Threading.Tasks;
#endif

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch]
    internal static class BeatmapDataLoaderLevelInfoInjector
    {
#if !PRE_V1_39_1
        // BeatmapDataLoader.LoadBeatmapData was renamed to LoadBeatmapDataAsync as of 1.39.1 and became
        // async (returning Task<IReadonlyBeatmapData?> instead of IReadonlyBeatmapData directly). The
        // sibling BeatmapDataLoaderLevelInfoInjectorAsync patch previously handled this by transpiling
        // the compiler-generated state machine's MoveNext directly (matching hardcoded local-variable
        // slots and a specific field-access pattern) -- that broke on 1.45.1 because the method body's
        // actual IL shape changed (the "enableBeatmapDataCaching" field the transpiler searched for is no
        // longer part of this state machine at all), causing an IL-compile-time exception that aborted
        // ALL of CustomJSONData's Harmony patches, not just this one. A plain postfix can't read the real
        // result via __result directly on an async method (it would just see the still-incomplete Task),
        // so instead this postfixes the async method itself (getting the Task back before the caller
        // does, since Harmony's postfix runs before the method returns to its caller) and chains a
        // continuation to run the same injection once it actually completes -- this only depends on the
        // method's public signature, not its internal IL layout, so it isn't exposed to the same class of
        // breakage. BeatmapDataLoaderLevelInfoInjectorAsync.cs was removed as redundant/broken.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BeatmapDataLoader), nameof(BeatmapDataLoader.LoadBeatmapDataAsync))]
        private static void InjectCustomDataAsync(
            Task<IReadonlyBeatmapData?> __result,
            IBeatmapLevelData beatmapLevelData,
            BeatmapKey beatmapKey)
        {
            __result.ContinueWith(
                t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        InjectCustomData(t.Result, beatmapLevelData, beatmapKey);
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
#else
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BeatmapDataLoader), "LoadBeatmapData")]
        private static void InjectCustomDataSync(IReadonlyBeatmapData __result, IBeatmapLevelData beatmapLevelData, BeatmapKey beatmapKey)
        {
            InjectCustomData(__result, beatmapLevelData, beatmapKey);
        }
#endif

        private static void InjectCustomData(IReadonlyBeatmapData? result, IBeatmapLevelData beatmapLevelData, BeatmapKey beatmapKey)
        {
            if (beatmapLevelData is not CustomFileBeatmapLevelData fileBeatmapLevelData ||
                result is not CustomBeatmapData beatmapData)
            {
                return;
            }

            beatmapData.levelCustomData = fileBeatmapLevelData.customData;
            FileDifficultyBeatmap? fileDifficultyBeatmap = fileBeatmapLevelData.GetDifficultyBeatmap(beatmapKey);
            if (fileDifficultyBeatmap is CustomFileDifficultyBeatmap customFileDifficultyBeatmap)
            {
                beatmapData.beatmapCustomData = customFileDifficultyBeatmap.customData;
            }
        }
    }
}
#endif
