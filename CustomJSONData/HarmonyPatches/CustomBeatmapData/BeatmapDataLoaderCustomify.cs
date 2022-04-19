using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BeatmapSaveDataVersion3;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    internal static class BeatmapDataLoaderCustomify
    {
        private static readonly ConstructorInfo _beatmapDataCtor = AccessTools.FirstConstructor(typeof(BeatmapData), _ => true);
        private static readonly ConstructorInfo _bpmTimeProcessorCtor = AccessTools.FirstConstructor(typeof(BeatmapDataLoader.BpmTimeProcessor), _ => true);
        private static readonly MethodInfo _createCustomBeatmapData = AccessTools.Method(typeof(BeatmapDataLoaderCustomify), nameof(CreateCustomBeatmapData));
        private static readonly MethodInfo _addCustomEvent = AccessTools.Method(typeof(BeatmapDataLoaderCustomify), nameof(AddCustomEvents));

        [HarmonyTranspiler]
        [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _beatmapDataCtor))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Set(OpCodes.Call, _createCustomBeatmapData)

                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _bpmTimeProcessorCtor))
                .Advance(2)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 4),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, _addCustomEvent))

                // "convertor" is NOT the correct spelling
                .ReplaceConverter<DataConvertor<BeatmapObjectData>, Converters.CustomDataConverter<BeatmapObjectData>>()

                .ReplaceConverter<BeatmapDataLoader.ColorNoteConvertor, Converters.CustomColorNoteConverter>()
                .ReplaceConverter<BeatmapDataLoader.BombNoteConvertor, Converters.CustomBombNoteConverter>()
                .ReplaceConverter<BeatmapDataLoader.ObstacleConvertor, Converters.CustomObstacleConverter>()
                .ReplaceConverter<BeatmapDataLoader.SliderConvertor, Converters.CustomSliderConverter>()
                .ReplaceConverter<BeatmapDataLoader.BurstSliderConvertor, Converters.CustomBurstSliderConverter>()
                .ReplaceConverter<BeatmapDataLoader.WaypointConvertor, Converters.CustomWaypointConverter>()

                .ReplaceConverter<DataConvertor<BeatmapEventData>, Converters.CustomDataConverter<BeatmapEventData>>()

                .ReplaceConverter<BeatmapDataLoader.BpmEventConvertor, Converters.CustomBpmEventConverter>()
                .ReplaceConverter<BeatmapDataLoader.RotationEventConvertor, Converters.CustomRotationEventConverter>()
                .ReplaceConverter<BeatmapDataLoader.BasicEventConvertor, Converters.CustomBasicEventConverter>()
                .ReplaceConverter<BeatmapDataLoader.ColorBoostEventConvertor, Converters.CustomColorBoostEventConverter>()

                .InstructionEnumeration();
        }

        private static CodeMatcher ReplaceConverter<TOriginal, TCustom>(this CodeMatcher matcher)
        {
            ConstructorInfo original = AccessTools.FirstConstructor(typeof(TOriginal), _ => true);
            ConstructorInfo custom = AccessTools.FirstConstructor(typeof(TCustom), _ => true);

            return matcher.MatchForward(false, new CodeMatch(OpCodes.Newobj, original))
                .SetOperandAndAdvance(custom);
        }

        private static BeatmapData CreateCustomBeatmapData(int numberOfLines, BeatmapSaveData beatmapSaveData)
        {
            if (beatmapSaveData is CustomBeatmapSaveData customBeatmapSaveData)
            {
                return new CustomBeatmapData(
                    numberOfLines,
                    customBeatmapSaveData.version2_6_0AndEarlier,
                    customBeatmapSaveData.customData,
                    customBeatmapSaveData.beatmapCustomData,
                    customBeatmapSaveData.levelCustomData);
            }

            return new CustomBeatmapData(
                4,
                false,
                new Dictionary<string, object?>(),
                new Dictionary<string, object?>(),
                new Dictionary<string, object?>());
        }

        private static void AddCustomEvents(
            BeatmapDataLoader.BpmTimeProcessor timeProcessor,
            CustomBeatmapData beatmapData,
            BeatmapSaveData saveData)
        {
            if (saveData is not CustomBeatmapSaveData customSaveData)
            {
                return;
            }

            foreach (CustomBeatmapSaveData.CustomEventData customEventSaveData in customSaveData.customEvents)
            {
                beatmapData.InsertCustomEventData(new CustomEventData(
                    timeProcessor.ConvertBeatToTime(customEventSaveData.beat),
                    customEventSaveData.type,
                    customEventSaveData.customData));
            }
        }
    }
}
