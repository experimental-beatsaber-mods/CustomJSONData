using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataStrobeFilterTransform))]
    internal static class StrobeFilterCustomify
    {
        private static readonly ConstructorInfo _beatmapDataCtor = AccessTools.FirstConstructor(typeof(BeatmapData), _ => true);
        private static readonly MethodInfo _newCustomBeatmapData = AccessTools.Method(typeof(StrobeFilterCustomify), nameof(NewCustomBeatmapData));

        private static readonly ConstructorInfo _eventDataCtor = AccessTools.FirstConstructor(typeof(BasicBeatmapEventData), _ => true);
        private static readonly ConstructorInfo _customEventDataCtor = AccessTools.FirstConstructor(typeof(CustomBasicBeatmapEventData), _ => true);
        private static readonly MethodInfo _getEventCustomData = AccessTools.Method(typeof(StrobeFilterCustomify), nameof(GetEventCustomData));

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(BeatmapDataStrobeFilterTransform.CreateTransformedData))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Newobj, _beatmapDataCtor))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, _newCustomBeatmapData))
                .RemoveInstruction()

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

        private static CustomBeatmapData NewCustomBeatmapData(int numberOfLines, CustomBeatmapData beatmapData)
        {
            CustomBeatmapData newBeatmapData = new(
                numberOfLines,
                beatmapData.customData,
                beatmapData.beatmapCustomData,
                beatmapData.levelCustomData);

            foreach (CustomEventData customEventData in beatmapData.GetBeatmapDataItems<CustomEventData>())
            {
                newBeatmapData.InsertCustomEventData(customEventData);
            }

            return newBeatmapData;
        }

        private static Dictionary<string, object?> GetEventCustomData(BeatmapEventData beatmapEventData)
        {
            if (beatmapEventData is ICustomData customBeatmapEventData)
            {
                return customBeatmapEventData.customData;
            }

            return new Dictionary<string, object?>();
        }
    }
}
