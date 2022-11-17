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

        private static readonly MethodInfo _insertCustomEvent = AccessTools.Method(typeof(StrobeFilterCustomify), nameof(InsertCustomEvent));

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
                    new CodeMatch(OpCodes.Isinst))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call, _insertCustomEvent))

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
                beatmapData.version2_6_0AndEarlier,
                beatmapData.customData.Copy(),
                beatmapData.beatmapCustomData.Copy(),
                beatmapData.levelCustomData.Copy());

            foreach (CustomEventData customEventData in beatmapData.customEventDatas)
            {
                newBeatmapData.InsertCustomEventData(customEventData);
            }

            return newBeatmapData;
        }

        private static BeatmapDataItem InsertCustomEvent(BeatmapDataItem beatmapDataItem, CustomBeatmapData beatmapData)
        {
            if (beatmapDataItem is CustomEventData customEventData)
            {
                beatmapData.InsertCustomEventDataInOrder(customEventData);
            }

            return beatmapDataItem; // return to stack
        }

        private static CustomData GetEventCustomData(BeatmapEventData beatmapEventData)
        {
            if (beatmapEventData is ICustomData customBeatmapEventData)
            {
                return customBeatmapEventData.customData;
            }

            return new CustomData();
        }
    }
}
