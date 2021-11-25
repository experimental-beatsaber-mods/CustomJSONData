using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataLoader))]
    [HarmonyPatch("GetBeatmapDataFromBeatmapSaveData")]
    internal static class BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData
    {
        private static readonly MethodInfo _createBombNoteData = AccessTools.Method(typeof(NoteData), nameof(NoteData.CreateBombNoteData));
        private static readonly MethodInfo _createBombCustomNoteData = AccessTools.Method(typeof(CustomNoteData), nameof(CustomNoteData.CreateBombNoteData));
        private static readonly MethodInfo _createBasicNoteData = AccessTools.Method(typeof(NoteData), nameof(NoteData.CreateBasicNoteData));
        private static readonly MethodInfo _createBasicCustomNoteData = AccessTools.Method(typeof(CustomNoteData), nameof(CustomNoteData.CreateBasicNoteData));
        private static readonly MethodInfo _getNoteCustomData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(GetNoteCustomData));

        private static readonly ConstructorInfo _waypointDataCtor = AccessTools.FirstConstructor(typeof(WaypointData), _ => true);
        private static readonly ConstructorInfo _customWaypointDataCtor = AccessTools.FirstConstructor(typeof(CustomWaypointData), _ => true);
        private static readonly MethodInfo _getWaypointCustomData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(GetWaypointCustomData));

        private static readonly ConstructorInfo _obstacleDataCtor = AccessTools.FirstConstructor(typeof(ObstacleData), _ => true);
        private static readonly ConstructorInfo _customObstacleDataCtor = AccessTools.FirstConstructor(typeof(CustomObstacleData), _ => true);
        private static readonly MethodInfo _getObstacleCustomData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(GetObstacleCustomData));

        private static readonly ConstructorInfo _eventDataCtor = AccessTools.FirstConstructor(typeof(BeatmapEventData), _ => true);
        private static readonly ConstructorInfo _customEventDataCtor = AccessTools.FirstConstructor(typeof(CustomBeatmapEventData), _ => true);
        private static readonly MethodInfo _getEventCustomData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(GetEventCustomData));

        private static readonly ConstructorInfo _dictionaryCtor = AccessTools.FirstConstructor(typeof(Dictionary<string, object?>), _ => true);

        private static readonly MethodInfo _createCustomBeatmapData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(CreateCustomBeatmapData));

        internal static CustomBeatmapSaveData? BeatmapSaveData { get; set; }

        internal static Dictionary<string, object?>? BeatmapCustomData { get; set; }

        internal static Dictionary<string, object?>? LevelCustomData { get; set; }

        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)

                // CreateCustomBeatmapData
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Stloc_S))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldarg_S, 7),
                    new CodeInstruction(OpCodes.Ldarg_S, 8),
                    new CodeInstruction(OpCodes.Call, _createCustomBeatmapData),
                    new CodeInstruction(OpCodes.Stloc_0))

                // bomb note
                .MatchForward(false, new CodeMatch(OpCodes.Call, _createBombNoteData))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 6),
                    new CodeInstruction(OpCodes.Call, _getNoteCustomData))
                .SetOperandAndAdvance(_createBombCustomNoteData)

                // basic note
                .MatchForward(false, new CodeMatch(OpCodes.Call, _createBasicNoteData))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 6),
                    new CodeInstruction(OpCodes.Call, _getNoteCustomData))
                .SetOperandAndAdvance(_createBasicCustomNoteData)

                // waypoint
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _waypointDataCtor))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 7),
                    new CodeInstruction(OpCodes.Call, _getWaypointCustomData))
                .SetOperandAndAdvance(_customWaypointDataCtor)

                // obstacle
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _obstacleDataCtor))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 8),
                    new CodeInstruction(OpCodes.Call, _getObstacleCustomData))
                .SetOperandAndAdvance(_customObstacleDataCtor)

                // event with custom data
                .MatchForward(
                    false,
                    new CodeMatch(OpCodes.Newobj, _eventDataCtor),
                    new CodeMatch(OpCodes.Stloc_S))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 21),
                    new CodeInstruction(OpCodes.Call, _getEventCustomData))
                .SetOperandAndAdvance(_customEventDataCtor)

                // event w/o custom data
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _eventDataCtor))
                .Repeat(n => n
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Newobj, _dictionaryCtor))
                    .SetOperandAndAdvance(_customEventDataCtor))

                .InstructionEnumeration();
        }

        private static Dictionary<string, object?> GetNoteCustomData(BeatmapSaveData.NoteData noteSaveData)
        {
            if (noteSaveData is CustomBeatmapSaveData.NoteData customNoteSaveData)
            {
                return new Dictionary<string, object?>(customNoteSaveData.customData);
            }

            return new Dictionary<string, object?>();
        }

        private static Dictionary<string, object?> GetWaypointCustomData(BeatmapSaveData.WaypointData waypointData)
        {
            if (waypointData is CustomBeatmapSaveData.WaypointData customWaypointData)
            {
                return new Dictionary<string, object?>(customWaypointData.customData);
            }

            return new Dictionary<string, object?>();
        }

        private static Dictionary<string, object?> GetObstacleCustomData(BeatmapSaveData.ObstacleData obstacleSaveData)
        {
            if (obstacleSaveData is CustomBeatmapSaveData.ObstacleData customObstacleSaveData)
            {
                return new Dictionary<string, object?>(customObstacleSaveData.customData);
            }

            return new Dictionary<string, object?>();
        }

        private static Dictionary<string, object?> GetEventCustomData(BeatmapSaveData.EventData eventSaveData)
        {
            if (eventSaveData is CustomBeatmapSaveData.EventData customEventSaveData)
            {
                return new Dictionary<string, object?>(customEventSaveData.customData);
            }

            return new Dictionary<string, object?>();
        }

        private static CustomBeatmapData CreateCustomBeatmapData(BeatmapDataLoader beatmapDataLoader, List<BeatmapDataLoader.BpmChangeData> bpmChanges, float shuffle, float shufflePeriod)
        {
            CustomBeatmapData customBeatmapData;

            if (BeatmapSaveData != null)
            {
                List<CustomBeatmapSaveData.CustomEventData> customEventsSaveData = BeatmapSaveData.customEvents;
                customEventsSaveData = customEventsSaveData.OrderBy(x => x.time).ToList();

                List<CustomEventData> customEventDatas = new(customEventsSaveData.Count);
                customEventDatas.AddRange(from customEventData in customEventsSaveData
                    let time = beatmapDataLoader.ProcessTime(customEventData.time, bpmChanges, shuffle, shufflePeriod)
                    select new CustomEventData(time, customEventData.type, customEventData.data));

                customBeatmapData = new CustomBeatmapData(4, customEventDatas, BeatmapSaveData.customData, BeatmapCustomData ?? new Dictionary<string, object?>(), LevelCustomData ?? new Dictionary<string, object?>());
            }
            else
            {
                customBeatmapData = new CustomBeatmapData(4, new List<CustomEventData>(), new Dictionary<string, object?>(), new Dictionary<string, object?>(), new Dictionary<string, object?>());
            }

            return customBeatmapData;
        }
    }
}
