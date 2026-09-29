#if !PRE_V1_45_1
using System.Collections.Generic;
using System.Linq;
using BeatmapDataLoaderVersion4;
using BeatmapSaveDataVersion4;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    // V4's per-item save data (ColorNote, BombNote, Obstacle, Chain, Arc, Waypoint, BasicEvent,
    // ColorBoostEvent) are plain structs -- see the comment on Version4CustomDataStore for why that
    // rules out the V2/V3 "subclass the save-data type" approach entirely. Each XxxItemConverter's
    // Convert method still ends up constructing the same runtime types V2/V3 already produce
    // (NoteData, ObstacleData, SliderData, WaypointData, BasicBeatmapEventData,
    // ColorBoostBeatmapEventData), so rather than fight V4's differing constructor/factory argument
    // order (rotation lands in different relative positions than the Custom* factories expect),
    // these postfixes just read every public property back off the already-constructed vanilla
    // result and use it to build the equivalent Custom*Data subclass, which already exists and is
    // shared with V2/V3.
    [HarmonyPatch]
    internal static class BeatmapDataLoaderV4Customify
    {
        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(BeatmapDataLoaderVersion4.BeatmapDataLoader),
            nameof(BeatmapDataLoaderVersion4.BeatmapDataLoader.GetBeatmapDataFromSaveDataJson))]
        private static void LoadCustomDataStore(string? beatmapJson, string? lightshowJson)
        {
            Version4CustomDataStore.Load(beatmapJson, lightshowJson);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ColorNoteItemConverter), nameof(ColorNoteItemConverter.Convert))]
        private static void ColorNoteConvertV4(BeatmapBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("colorNotesData", index.i);
            if (customData != null && __result is NoteData noteData)
            {
                __result = BuildCustomNoteData(noteData, customData);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BombNoteItemConverter), nameof(BombNoteItemConverter.Convert))]
        private static void BombNoteConvertV4(BeatmapBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("bombNotesData", index.i);
            if (customData != null && __result is NoteData noteData)
            {
                __result = BuildCustomNoteData(noteData, customData);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ObstacleItemConverter), nameof(ObstacleItemConverter.Convert))]
        private static void ObstacleConvertV4(BeatmapBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("obstaclesData", index.i);
            if (customData == null || __result is not ObstacleData obstacleData)
            {
                return;
            }

            __result = new CustomObstacleData(
                obstacleData.time,
                obstacleData.beat,
                obstacleData.endBeat,
                obstacleData.rotation,
                obstacleData.lineIndex,
                obstacleData.lineLayer,
                obstacleData.duration,
                obstacleData.width,
                obstacleData.height,
                customData,
                VersionExtensions.version4);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChainItemConverter), nameof(ChainItemConverter.Convert))]
        private static void ChainConvertV4(ChainBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("chainsData", index.ci);
            if (customData != null && __result is SliderData sliderData)
            {
                __result = BuildCustomSliderData(sliderData, customData);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArcItemConverter), nameof(ArcItemConverter.Convert))]
        private static void ArcConvertV4(ArcBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("arcsData", index.ai);
            if (customData != null && __result is SliderData sliderData)
            {
                __result = BuildCustomSliderData(sliderData, customData);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WaypointItemConverter), nameof(WaypointItemConverter.Convert))]
        private static void WaypointConvertV4(BeatmapBeatIndex index, ref BeatmapObjectData __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("waypointsData", index.i);
            if (customData == null || __result is not WaypointData waypointData)
            {
                return;
            }

            __result = new CustomWaypointData(
                waypointData.time,
                waypointData.beat,
                waypointData.rotation,
                waypointData.lineIndex,
                waypointData.lineLayer,
                waypointData.offsetDirection,
                customData,
                VersionExtensions.version4);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BasicEventItemConverter), nameof(BasicEventItemConverter.Convert))]
        private static void BasicEventConvertV4(BeatIndex index, ref IEnumerable<BeatmapEventData> __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("basicEventsData", index.i);
            if (customData == null)
            {
                return;
            }

            __result = __result.Select(eventData =>
            {
                if (eventData.GetType() != typeof(BasicBeatmapEventData))
                {
                    return eventData;
                }

                BasicBeatmapEventData basic = (BasicBeatmapEventData)eventData;
                return (BeatmapEventData)new CustomBasicBeatmapEventData(
                    basic.time,
                    basic.basicBeatmapEventType,
                    basic.value,
                    basic.floatValue,
                    customData,
                    VersionExtensions.version4);
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ColorBoostEventItemConverter), nameof(ColorBoostEventItemConverter.Convert))]
        private static void ColorBoostEventConvertV4(BeatIndex index, ref IEnumerable<BeatmapEventData> __result)
        {
            CustomData? customData = Version4CustomDataStore.Get("colorBoostEventsData", index.i);
            if (customData == null)
            {
                return;
            }

            __result = __result.Select(eventData =>
            {
                if (eventData.GetType() != typeof(ColorBoostBeatmapEventData))
                {
                    return eventData;
                }

                ColorBoostBeatmapEventData colorBoost = (ColorBoostBeatmapEventData)eventData;
                return (BeatmapEventData)new CustomColorBoostBeatmapEventData(
                    colorBoost.time,
                    colorBoost.boostColorsAreOn,
                    customData,
                    VersionExtensions.version4);
            });
        }

        private static CustomNoteData BuildCustomNoteData(NoteData noteData, CustomData customData)
        {
            return new CustomNoteData(
                noteData.time,
                noteData.beat,
                noteData.rotation,
                noteData.lineIndex,
                noteData.noteLineLayer,
                noteData.beforeJumpNoteLineLayer,
                noteData.gameplayType,
                noteData.scoringType,
                noteData.colorType,
                noteData.cutDirection,
                noteData.timeToNextColorNote,
                noteData.timeToPrevColorNote,
                noteData.flipLineIndex,
                noteData.flipYSide,
                noteData.cutDirectionAngleOffset,
                noteData.cutSfxVolumeMultiplier,
                customData,
                VersionExtensions.version4);
        }

        private static CustomSliderData BuildCustomSliderData(SliderData sliderData, CustomData customData)
        {
            return new CustomSliderData(
                sliderData.sliderType,
                sliderData.colorType,
                sliderData.hasHeadNote,
                sliderData.time,
                sliderData.beat,
                sliderData.rotation,
                sliderData.headLineIndex,
                sliderData.headLineLayer,
                sliderData.headBeforeJumpLineLayer,
                sliderData.headControlPointLengthMultiplier,
                sliderData.headCutDirection,
                sliderData.headCutDirectionAngleOffset,
                sliderData.hasTailNote,
                sliderData.tailTime,
                sliderData.tailRotation,
                sliderData.tailLineIndex,
                sliderData.tailLineLayer,
                sliderData.tailBeforeJumpLineLayer,
                sliderData.tailControlPointLengthMultiplier,
                sliderData.tailCutDirection,
                sliderData.tailCutDirectionAngleOffset,
                sliderData.midAnchorMode,
                sliderData.sliceCount,
                sliderData.squishAmount,
                customData,
                VersionExtensions.version4);
        }
    }
}
#endif
