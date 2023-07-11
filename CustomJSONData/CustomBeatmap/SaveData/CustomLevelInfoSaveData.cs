using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomLevelInfoSaveData : StandardLevelInfoSaveData
    {
        internal CustomLevelInfoSaveData(
            string version,
            string songName,
            string songSubName,
            string songAuthorName,
            string levelAuthorName,
            float beatsPerMinute,
            float songTimeOffset,
            float shuffle,
            float shufflePeriod,
            float previewStartTime,
            float previewDuration,
            string songFilename,
            string coverImageFilename,
            string environmentName,
            string allDirectionsEnvironmentName,
            string[] environmentNames,
            BeatmapLevelColorSchemeSaveData[] colorSchemes,
            DifficultyBeatmapSet[] difficultyBeatmapSets,
            CustomData customData,
            Dictionary<string, CustomData> beatmapCustomDatasByFilename)
            : base(
                  songName,
                  songSubName,
                  songAuthorName,
                  levelAuthorName,
                  beatsPerMinute,
                  songTimeOffset,
                  shuffle,
                  shufflePeriod,
                  previewStartTime,
                  previewDuration,
                  songFilename,
                  coverImageFilename,
                  environmentName,
                  allDirectionsEnvironmentName,
                  environmentNames,
                  colorSchemes,
                  difficultyBeatmapSets)
        {
            _version = version;
            this.customData = customData;
            this.beatmapCustomDatasByFilename = beatmapCustomDatasByFilename;
        }

        public CustomData customData { get; }

        public Dictionary<string, CustomData> beatmapCustomDatasByFilename { get; }

        // SongCore 3.9.0 skips past the CustomLevelLoader, so we just use the string data instead
        // Unfortunately no reading from a stream, but info.dat's should never be big enough to matter
        internal static CustomLevelInfoSaveData Deserialize(string stringData)
        {
            string version = string.Empty;
            string songName = string.Empty;
            string songSubName = string.Empty;
            string songAuthorName = string.Empty;
            string levelAuthorName = string.Empty;
            float beatsPerMinute = default;
            float songTimeOffset = default;
            float shuffle = default;
            float shufflePeriod = default;
            float previewStartTime = default;
            float previewDuration = default;
            string songFilename = string.Empty;
            string coverImageFilename = string.Empty;
            string environmentName = string.Empty;
            string allDirectionsEnvrionmentName = string.Empty;
            string[] environmentNames = Array.Empty<string>();
            List<BeatmapLevelColorSchemeSaveData> colorSchemes = new();
            List<DifficultyBeatmapSet> difficultyBeatmapSets = new();
            CustomData customData = new();
            Dictionary<string, CustomData> beatmapCustomDatasByFilename = new();

            using JsonTextReader reader = new(new StringReader(stringData));
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch (reader.Value)
                    {
                        default:
                            reader.Skip();
                            break;

                        case "_version":
                            version = reader.ReadAsString() ?? version;
                            break;

                        case "_songName":
                            songName = reader.ReadAsString() ?? songName;
                            break;

                        case "_songSubName":
                            songSubName = reader.ReadAsString() ?? songSubName;
                            break;

                        case "_songAuthorName":
                            songAuthorName = reader.ReadAsString() ?? songAuthorName;
                            break;

                        case "_levelAuthorName":
                            levelAuthorName = reader.ReadAsString() ?? levelAuthorName;
                            break;

                        case "_beatsPerMinute":
                            beatsPerMinute = (float?)reader.ReadAsDouble() ?? beatsPerMinute;
                            break;

                        case "_songTimeOffset":
                            songTimeOffset = (float?)reader.ReadAsDouble() ?? songTimeOffset;
                            break;

                        case "_shuffle":
                            shuffle = (float?)reader.ReadAsDouble() ?? shuffle;
                            break;

                        case "_shufflePeriod":
                            shufflePeriod = (float?)reader.ReadAsDouble() ?? shufflePeriod;
                            break;

                        case "_previewStartTime":
                            previewStartTime = (float?)reader.ReadAsDouble() ?? previewStartTime;
                            break;

                        case "_previewDuration":
                            previewDuration = (float?)reader.ReadAsDouble() ?? previewDuration;
                            break;

                        case "_songFilename":
                            songFilename = reader.ReadAsString() ?? songFilename;
                            break;

                        case "_coverImageFilename":
                            coverImageFilename = reader.ReadAsString() ?? coverImageFilename;
                            break;

                        case "_environmentName":
                            environmentName = reader.ReadAsString() ?? environmentName;
                            break;

                        case "_allDirectionsEnvironmentName":
                            allDirectionsEnvrionmentName = reader.ReadAsString() ?? allDirectionsEnvrionmentName;
                            break;

                        case "_environmentNames":
                            environmentNames = reader.ReadStringArray() ?? environmentNames;
                            break;

                        case "_colorSchemes":
                            reader.ReadObjectArray(() =>
                            {
                                bool useOverride = false;
                                PlayerSaveData.ColorScheme? colorScheme = null;
                                reader.ReadObject(objectName =>
                                {
                                    switch (objectName)
                                    {
                                        case "useOverride":
                                            useOverride = reader.ReadAsBoolean() ?? useOverride;
                                            break;

                                        case "colorScheme":
                                            string colorSchemeId = string.Empty;
                                            Color saberAColor = default;
                                            Color saberBColor = default;
                                            Color environmentColor0 = default;
                                            Color environmentColor1 = default;
                                            Color obstaclesColor = default;
                                            Color environmentColor0Boost = default;
                                            Color environmentColor1Boost = default;
                                            reader.ReadObject(colorSchemeName =>
                                            {
                                                switch (colorSchemeName)
                                                {
                                                    case "colorSchemeId":
                                                        colorSchemeId = reader.ReadAsString() ?? colorSchemeId;
                                                        break;

                                                    case "saberAColor":
                                                        saberAColor = reader.ReadAsColor();
                                                        break;

                                                    case "saberBColor":
                                                        saberBColor = reader.ReadAsColor();
                                                        break;

                                                    case "environmentColor0":
                                                        environmentColor0 = reader.ReadAsColor();
                                                        break;

                                                    case "environmentColor1":
                                                        environmentColor1 = reader.ReadAsColor();
                                                        break;

                                                    case "obstaclesColor":
                                                        obstaclesColor = reader.ReadAsColor();
                                                        break;

                                                    case "environmentColor0Boost":
                                                        environmentColor0Boost = reader.ReadAsColor();
                                                        break;

                                                    case "environmentColor1Boost":
                                                        environmentColor1Boost = reader.ReadAsColor();
                                                        break;
                                                }
                                            });

                                            colorScheme = new PlayerSaveData.ColorScheme(
                                                colorSchemeId,
                                                saberAColor,
                                                saberBColor,
                                                environmentColor0,
                                                environmentColor1,
                                                obstaclesColor,
                                                environmentColor0Boost,
                                                environmentColor1Boost);
                                            break;
                                    }
                                });

                                colorSchemes.Add(new BeatmapLevelColorSchemeSaveData
                                {
                                    useOverride = useOverride,
                                    colorScheme = colorScheme
                                });
                            });
                            break;

                        case "_difficultyBeatmapSets":
                            reader.ReadObjectArray(() =>
                            {
                                string beatmapCharacteristicName = string.Empty;
                                List<DifficultyBeatmap> difficultyBeatmaps = new();
                                reader.ReadObject(objectName =>
                                {
                                    switch (objectName)
                                    {
                                        case "_beatmapCharacteristicName":
                                            beatmapCharacteristicName = reader.ReadAsString() ?? beatmapCharacteristicName;
                                            break;

                                        case "_difficultyBeatmaps":
                                            reader.ReadObjectArray(() =>
                                            {
                                                string difficulty = string.Empty;
                                                int difficultyRank = default;
                                                string beatmapFilename = string.Empty;
                                                float noteJumpMovementSpeed = default;
                                                float noteJumpStartBeatOffset = default;
                                                int beatmapColorSchemeIdx = default;
                                                int environmentNameIdx = default;
                                                CustomData data = new();
                                                reader.ReadObject(difficultyBeatmapObjectName =>
                                                {
                                                    switch (difficultyBeatmapObjectName)
                                                    {
                                                        case "_difficulty":
                                                            difficulty = reader.ReadAsString() ?? difficulty;
                                                            break;

                                                        case "_difficultyRank":
                                                            difficultyRank = reader.ReadAsInt32Safe() ?? difficultyRank;
                                                            break;

                                                        case "_beatmapFilename":
                                                            beatmapFilename = reader.ReadAsString() ?? beatmapFilename;
                                                            break;

                                                        case "_noteJumpMovementSpeed":
                                                            noteJumpMovementSpeed = (float?)reader.ReadAsDouble() ?? noteJumpMovementSpeed;
                                                            break;

                                                        case "_noteJumpStartBeatOffset":
                                                            noteJumpStartBeatOffset = (float?)reader.ReadAsDouble() ?? noteJumpStartBeatOffset;
                                                            break;

                                                        case "_beatmapColorSchemeIdx":
                                                            beatmapColorSchemeIdx = reader.ReadAsInt32Safe() ?? beatmapColorSchemeIdx;
                                                            break;

                                                        case "_environmentNameIdx":
                                                            environmentNameIdx = reader.ReadAsInt32Safe() ?? environmentNameIdx;
                                                            break;

                                                        case "_customData":
                                                            reader.ReadToDictionary(data);
                                                            break;

                                                        default:
                                                            reader.Skip();
                                                            break;
                                                    }
                                                });

                                                beatmapCustomDatasByFilename[beatmapFilename] = data;
                                                difficultyBeatmaps.Add(new DifficultyBeatmap(
                                                    difficulty,
                                                    difficultyRank,
                                                    beatmapFilename,
                                                    noteJumpMovementSpeed,
                                                    noteJumpStartBeatOffset,
                                                    beatmapColorSchemeIdx,
                                                    environmentNameIdx,
                                                    data));
                                            });

                                            break;

                                        default:
                                            reader.Skip();
                                            break;
                                    }
                                });

                                difficultyBeatmapSets.Add(new DifficultyBeatmapSet(
                                    beatmapCharacteristicName,
                                    difficultyBeatmaps.ToArray<StandardLevelInfoSaveData.DifficultyBeatmap>()));
                            });

                            break;

                        case "_customData":
                            reader.ReadToDictionary(customData);
                            break;
                    }
                }
            }

            return new CustomLevelInfoSaveData(
                version,
                songName,
                songSubName,
                songAuthorName,
                levelAuthorName,
                beatsPerMinute,
                songTimeOffset,
                shuffle,
                shufflePeriod,
                previewStartTime,
                previewDuration,
                songFilename,
                coverImageFilename,
                environmentName,
                allDirectionsEnvrionmentName,
                environmentNames,
                colorSchemes.ToArray(),
                difficultyBeatmapSets.ToArray(),
                customData,
                beatmapCustomDatasByFilename);
        }

        public new class DifficultyBeatmap : StandardLevelInfoSaveData.DifficultyBeatmap
        {
            internal DifficultyBeatmap(
                string difficultyName,
                int difficultyRank,
                string beatmapFilename,
                float noteJumpMovementSpeed,
                float noteJumpStartBeatOffset,
                int beatmapColorSchemeIdx,
                int environmentNameIdx,
                CustomData customData)
            : base(
                difficultyName,
                difficultyRank,
                beatmapFilename,
                noteJumpMovementSpeed,
                noteJumpStartBeatOffset,
                beatmapColorSchemeIdx,
                environmentNameIdx)
            {
                this.customData = customData;
            }

            [PublicAPI]
            public CustomData customData { get; }
        }
    }
}
