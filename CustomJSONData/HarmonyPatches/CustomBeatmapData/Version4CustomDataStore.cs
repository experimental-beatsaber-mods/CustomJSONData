#if !PRE_V1_45_1
using System.Collections.Generic;
using CustomJSONData.CustomBeatmap;
using Newtonsoft.Json.Linq;

namespace CustomJSONData.HarmonyPatches
{
    // BeatmapSaveDataVersion4's item types (ColorNote, BombNote, Obstacle, Chain, Arc, Waypoint,
    // BasicEvent, ColorBoostEvent) are all plain structs deserialized directly by Unity's
    // JsonUtility, which -- unlike Newtonsoft.Json -- has no extension-data/custom-converter
    // mechanism to capture unrecognized JSON fields. There is no way to subclass a struct or hook
    // JsonUtility to smuggle a "customData" field through it the way the V2/V3 pipeline does.
    //
    // Instead, this does a second, independent parse of the same raw JSON (via Newtonsoft, which
    // CustomJSONData already depends on) purely to pull out each item's "customData" object by its
    // position in its save-data array -- the same index BeatmapBeatIndex/ChainBeatIndex/etc. use
    // to look the item up in that array. BeatmapDataLoaderV4Customify's postfixes then look up that
    // index here when reconstructing the runtime object.
    internal static class Version4CustomDataStore
    {
        private static Dictionary<string, Dictionary<int, CustomData>> _current = new();

        internal static void Load(string? beatmapJson, string? lightshowJson)
        {
            Dictionary<string, Dictionary<int, CustomData>> map = new();
            if (!string.IsNullOrEmpty(beatmapJson))
            {
                ExtractInto(map, beatmapJson, "colorNotesData");
                ExtractInto(map, beatmapJson, "bombNotesData");
                ExtractInto(map, beatmapJson, "obstaclesData");
                ExtractInto(map, beatmapJson, "chainsData");
                ExtractInto(map, beatmapJson, "arcsData");
            }

            if (!string.IsNullOrEmpty(lightshowJson))
            {
                ExtractInto(map, lightshowJson, "basicEventsData");
                ExtractInto(map, lightshowJson, "colorBoostEventsData");
                ExtractInto(map, lightshowJson, "waypointsData");
            }

            _current = map;
        }

        internal static CustomData? Get(string arrayName, int index)
        {
            return _current.TryGetValue(arrayName, out Dictionary<int, CustomData>? byIndex) &&
                   byIndex.TryGetValue(index, out CustomData? data)
                ? data
                : null;
        }

        private static void ExtractInto(
            Dictionary<string, Dictionary<int, CustomData>> map,
            string json,
            string arrayName)
        {
            JObject root;
            try
            {
                root = JObject.Parse(json);
            }
            catch
            {
                // Malformed JSON is the base game's problem to report, not ours -- just skip
                // custom data extraction for this file.
                return;
            }

            if (root[arrayName] is not JArray array)
            {
                return;
            }

            Dictionary<int, CustomData>? byIndex = null;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] is JObject item && item["customData"] is JObject customDataToken)
                {
                    byIndex ??= new Dictionary<int, CustomData>();
                    byIndex[i] = CustomData.FromJSON(customDataToken.CreateReader());
                }
            }

            if (byIndex != null)
            {
                map[arrayName] = byIndex;
            }
        }
    }
}
#endif
