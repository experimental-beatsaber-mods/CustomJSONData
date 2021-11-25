using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

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
            DifficultyBeatmapSet[] difficultyBeatmapSets,
            Dictionary<string, object?> customData,
            Dictionary<string, Dictionary<string, object?>> beatmapCustomDatasByFilename)
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
                  difficultyBeatmapSets)
        {
            _version = version;
            this.customData = customData;
            this.beatmapCustomDatasByFilename = beatmapCustomDatasByFilename;
        }

        public Dictionary<string, object?> customData { get; }

        public Dictionary<string, Dictionary<string, object?>> beatmapCustomDatasByFilename { get; }

        internal static CustomLevelInfoSaveData Deserialize(string path)
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
            List<DifficultyBeatmapSet> difficultyBeatmapSets = new();
            Dictionary<string, object?> customData = new();
            Dictionary<string, Dictionary<string, object?>> beatmapCustomDatasByFilename = new();

            using JsonTextReader reader = new(new StreamReader(path));
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
                                                Dictionary<string, object?> data = new();
                                                reader.ReadObject(difficultyBeatmapObjectName =>
                                                {
                                                    switch (difficultyBeatmapObjectName)
                                                    {
                                                        case "_difficulty":
                                                            difficulty = reader.ReadAsString() ?? difficulty;
                                                            break;

                                                        case "_difficultyRank":
                                                            difficultyRank = reader.ReadAsInt32() ?? difficultyRank;
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

                                                        case "_customData":
                                                            reader.ReadToDictionary(data);
                                                            break;

                                                        default:
                                                            reader.Skip();
                                                            break;
                                                    }
                                                });

                                                beatmapCustomDatasByFilename[beatmapFilename] = data;
                                                difficultyBeatmaps.Add(new DifficultyBeatmap(difficulty, difficultyRank, beatmapFilename, noteJumpMovementSpeed, noteJumpStartBeatOffset, data));
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
                difficultyBeatmapSets.ToArray(),
                customData,
                beatmapCustomDatasByFilename);
        }

        public new class DifficultyBeatmap : StandardLevelInfoSaveData.DifficultyBeatmap
        {
            internal DifficultyBeatmap(string difficultyName, int difficultyRank, string beatmapFilename, float noteJumpMovementSpeed, float noteJumpStartBeatOffset, Dictionary<string, object?> customData)
            : base(difficultyName, difficultyRank, beatmapFilename, noteJumpMovementSpeed, noteJumpStartBeatOffset)
            {
                this.customData = customData;
            }

            [PublicAPI]
            public Dictionary<string, object?> customData { get; }
        }
    }
}
