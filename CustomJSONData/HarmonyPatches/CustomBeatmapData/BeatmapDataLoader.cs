namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using CustomJSONData;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

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

        private static readonly MethodInfo _createCustomBeatmapData = AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData), nameof(CreateCustomBeatmapData));

        internal static CustomBeatmapSaveData? BeatmapSaveData { get; set; }

        internal static Dictionary<string, object?>? BeatmapCustomData { get; set; }

        internal static Dictionary<string, object?>? LevelCustomData { get; set; }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            FieldInfo? bpmChangesDataField = null;

            bool foundBombNoteData = false;
            bool foundBasicNoteData = false;
            bool foundWaypointData = false;
            bool foundObstacleData = false;
            bool foundEventData = false;
            bool foundBeatmapData = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundBombNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createBombNoteData)
                {
                    foundBombNoteData = true;
                    instructionList[i].operand = _createBombCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 6));
                }

                if (!foundBasicNoteData &&
                    instructionList[i].opcode == OpCodes.Call &&
                    instructionList[i].operand == _createBasicNoteData)
                {
                    foundBasicNoteData = true;
                    instructionList[i].operand = _createBasicCustomNoteData;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getNoteCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 6));
                }

                if (!foundWaypointData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _waypointDataCtor)
                {
                    foundWaypointData = true;
                    instructionList[i].operand = _customWaypointDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getWaypointCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 7));
                }

                if (!foundObstacleData &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _obstacleDataCtor)
                {
                    foundObstacleData = true;
                    instructionList[i].operand = _customObstacleDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getObstacleCustomData));
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 8));
                }

                if (instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _eventDataCtor)
                {
                    instructionList[i].operand = _customEventDataCtor;
                    instructionList.Insert(i, new CodeInstruction(OpCodes.Call, _getEventCustomData));
                    if (!foundEventData)
                    {
                        instructionList.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, 21));
                    }
                    else
                    {
                        instructionList.Insert(i, new CodeInstruction(OpCodes.Ldnull));
                    }

                    foundEventData = true;
                }

                if (bpmChangesDataField == null &&
                    instructionList[i].opcode == OpCodes.Stfld &&
                    ((FieldInfo)instructionList[i].operand).Name == "bpmChangesData")
                {
                    bpmChangesDataField = (FieldInfo)instructionList[i].operand;
                }

                // we look for this specifically because it happens after the bpm changes have been loaded
                if (!foundBeatmapData &&
                    bpmChangesDataField != null &&
                    instructionList[i].opcode == OpCodes.Stfld &&
                    ((FieldInfo)instructionList[i].operand).Name == "bpmChangesDataIdx")
                {
                    foundBeatmapData = true;
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_0));
                    instructionList.Insert(i + 3, new CodeInstruction(OpCodes.Ldfld, bpmChangesDataField));
                    instructionList.Insert(i + 4, new CodeInstruction(OpCodes.Ldarg_S, 7));
                    instructionList.Insert(i + 5, new CodeInstruction(OpCodes.Ldarg_S, 8));
                    instructionList.Insert(i + 6, new CodeInstruction(OpCodes.Call, _createCustomBeatmapData));
                    instructionList.Insert(i + 7, new CodeInstruction(OpCodes.Stloc_1));
                }
            }
#pragma warning restore CS0252
            if (!foundBombNoteData || !foundBasicNoteData || /*!foundLongNoteData ||*/ !foundWaypointData || !foundObstacleData || !foundEventData || !foundBeatmapData)
            {
                Logger.Log("Failed to patch GetBeatmapDataFromBeatmapSaveData in BeatmapDataLoader!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
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

                List<CustomEventData> customEventDatas = new List<CustomEventData>(customEventsSaveData.Count);

                foreach (CustomBeatmapSaveData.CustomEventData customEventData in customEventsSaveData)
                {
                    // Same math from BeatmapDataLoader
                    int bpmChangesDataIdx = 0;
                    float time = customEventData.time;
                    while (bpmChangesDataIdx < bpmChanges.Count - 1 && bpmChanges[bpmChangesDataIdx + 1].bpmChangeStartBpmTime < time)
                    {
                        bpmChangesDataIdx++;
                    }

                    BeatmapDataLoader.BpmChangeData bpmchangeData = bpmChanges[bpmChangesDataIdx];
                    float realTime = bpmchangeData.bpmChangeStartTime + beatmapDataLoader.GetRealTimeFromBPMTime(time - bpmchangeData.bpmChangeStartBpmTime, bpmchangeData.bpm, shuffle, shufflePeriod);

                    customEventDatas.Add(new CustomEventData(realTime, customEventData.type, customEventData.data));
                }

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
