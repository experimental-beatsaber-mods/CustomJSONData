using JetBrains.Annotations;
using SiraUtil.Affinity;

namespace CustomJSONData.HarmonyPatches
{
    internal class BeatmapObjectCallbackControllerSetNewBeatmapData : IAffinity
    {
        private readonly CustomEventCallbackController _customEventCallbackController;

        private BeatmapObjectCallbackControllerSetNewBeatmapData(
            CustomEventCallbackController customEventCallbackController)
        {
            _customEventCallbackController = customEventCallbackController;
        }

        [UsedImplicitly]
        [AffinityPostfix]
        [AffinityPatch(typeof(BeatmapObjectCallbackController), nameof(BeatmapObjectCallbackController.SetNewBeatmapData))]
        private void Postfix(BeatmapObjectCallbackController __instance, IReadonlyBeatmapData beatmapData)
        {
            _customEventCallbackController.SetNewBeatmapData(beatmapData);
        }
    }
}
