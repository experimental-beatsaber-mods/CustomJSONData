using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataStrobeFilterTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal static class BeatmapDataStrobeFilterTransformCreateTransformedData
    {
        private static readonly ConstructorInfo _eventDataCtor = AccessTools.FirstConstructor(typeof(BeatmapEventData), _ => true);
        private static readonly ConstructorInfo _customEventDataCtor = AccessTools.FirstConstructor(typeof(CustomBeatmapEventData), _ => true);
        private static readonly MethodInfo _getEventCustomData = AccessTools.Method(typeof(BeatmapDataStrobeFilterTransformCreateTransformedData), nameof(GetEventCustomData));

        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Newobj, _eventDataCtor))
                .Repeat(n => n
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldloc_S, 5),
                        new CodeInstruction(OpCodes.Call, _getEventCustomData))
                    .SetOperandAndAdvance(_customEventDataCtor))
                .InstructionEnumeration();
        }

        private static Dictionary<string, object?> GetEventCustomData(BeatmapEventData beatmapEventData)
        {
            if (beatmapEventData is CustomBeatmapEventData customBeatmapEventData)
            {
                return new Dictionary<string, object?>(customBeatmapEventData.customData);
            }

            return new Dictionary<string, object?>();
        }
    }
}
