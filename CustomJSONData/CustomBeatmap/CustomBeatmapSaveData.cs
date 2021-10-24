namespace CustomJSONData.CustomBeatmap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class CustomBeatmapSaveData : BeatmapSaveData
    {
        private CustomBeatmapSaveData(
            string version,
            List<BeatmapSaveData.EventData> events,
            List<BeatmapSaveData.NoteData> notes,
            List<BeatmapSaveData.WaypointData> waypoints,
            List<BeatmapSaveData.ObstacleData> obstacles,
            SpecialEventKeywordFiltersData specialEventsKeywordFilters,
            Dictionary<string, object?> customData,
            List<CustomEventData> customEvents)
            : base(events, notes, waypoints, obstacles, specialEventsKeywordFilters)
        {
            _version = version;
            this.customData = customData;
            this.customEvents = customEvents;
        }

        public static event Action<DeserializeEventArgs>? deserializeCustomDataEvent;

        public List<CustomEventData> customEvents { get; }

        public Dictionary<string, object?> customData { get; }

        public static CustomBeatmapSaveData Deserialize(string path)
        {
            string version = string.Empty;
            Dictionary<string, object?> customData = new Dictionary<string, object?>();
            List<CustomEventData> customEvents = new List<CustomEventData>();
            List<EventData> events = new List<EventData>();
            List<NoteData> notes = new List<NoteData>();
            List<WaypointData> waypoints = new List<WaypointData>();
            List<ObstacleData> obstacles = new List<ObstacleData>();
            List<SpecialEventsForKeyword> keywords = new List<SpecialEventsForKeyword>();

            using JsonTextReader reader = new JsonTextReader(new StreamReader(path));
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

                        case "_events":
                            reader.ReadObjectArray(() => events.Add(DeserializeEvent(reader)));

                            break;

                        case "_notes":
                            reader.ReadObjectArray(() => notes.Add(DeserializeNote(reader)));

                            break;

                        case "_waypoints":
                            reader.ReadObjectArray(() => waypoints.Add(DeserializeWaypoint(reader)));

                            break;

                        case "_obstacles":
                            reader.ReadObjectArray(() => obstacles.Add(DeserializeObstacle(reader)));

                            break;

                        case "_specialEventsKeywordFilters":
                            reader.Read();
                            reader.ReadObject(propertyName =>
                            {
                                if (propertyName.Equals("_keywords"))
                                {
                                    reader.ReadObjectArray(() => keywords.Add(DeserializeKeyword(reader)));
                                }
                                else
                                {
                                    reader.Skip();
                                }
                            });

                            break;

                        case "_customData":
                            DeserializeEventArgs eventArgs = new DeserializeEventArgs(reader, customData, customEvents, events, notes, waypoints, obstacles, keywords);

                            reader.ReadToDictionary(customData, propertyName =>
                            {
                                eventArgs.PropertyName = propertyName;
                                DeserializeCustomEventsArray(eventArgs);
                                deserializeCustomDataEvent?.Invoke(eventArgs);
                                if (eventArgs.DontAddToDictionary)
                                {
                                    eventArgs.DontAddToDictionary = false;
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            });

                            break;
                    }
                }
            }

            CustomBeatmapSaveData beatmapSaveData = new CustomBeatmapSaveData(
                version,
                events.Cast<BeatmapSaveData.EventData>().ToList(),
                notes.Cast<BeatmapSaveData.NoteData>().ToList(),
                waypoints.Cast<BeatmapSaveData.WaypointData>().ToList(),
                obstacles.Cast<BeatmapSaveData.ObstacleData>().ToList(),
                new SpecialEventKeywordFiltersData(keywords),
                customData,
                customEvents);

            // Below taken straight from BeatmapSaveData.DeserializeFromJSONString
            if (!string.IsNullOrEmpty(beatmapSaveData.version))
            {
                Version versionVersion = new Version(beatmapSaveData.version);
                Version value = new Version("2.5.0");
                if (versionVersion.CompareTo(value) < 0)
                {
                    ConvertBeatmapSaveDataPreV2_5_0(beatmapSaveData);
                }
            }
            else
            {
                ConvertBeatmapSaveDataPreV2_5_0(beatmapSaveData);
            }

            return beatmapSaveData;
        }

        public static EventData DeserializeEvent(JsonTextReader reader)
        {
            float time = default;
            BeatmapEventType type = default;
            int value = default;
            float floatValue = default;
            Dictionary<string, object?> data = new Dictionary<string, object?>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_time":
                        time = (float?)reader.ReadAsDouble() ?? time;
                        break;

                    case "_type":
                        type = (BeatmapEventType?)reader.ReadAsInt32() ?? type;
                        break;

                    case "_value":
                        value = reader.ReadAsInt32() ?? value;
                        break;

                    case "_floatValue":
                        floatValue = (float?)reader.ReadAsDouble() ?? floatValue;
                        break;

                    case "_customData":
                        reader.ReadToDictionary(data);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new EventData(time, type, value, floatValue, data);
        }

        public static NoteData DeserializeNote(JsonTextReader reader)
        {
            float time = default;
            int lineIndex = default;
            NoteLineLayer lineLayer = default;
            NoteType type = default;
            NoteCutDirection cutDirection = default;
            Dictionary<string, object?> data = new Dictionary<string, object?>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_time":
                        time = (float?)reader.ReadAsDouble() ?? time;
                        break;

                    case "_lineIndex":
                        lineIndex = reader.ReadAsInt32() ?? lineIndex;
                        break;

                    case "_lineLayer":
                        lineLayer = (NoteLineLayer?)reader.ReadAsInt32() ?? lineLayer;
                        break;

                    case "_type":
                        type = (NoteType?)reader.ReadAsInt32() ?? type;
                        break;

                    case "_cutDirection":
                        cutDirection = (NoteCutDirection?)reader.ReadAsInt32() ?? cutDirection;
                        break;

                    case "_customData":
                        reader.ReadToDictionary(data);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new NoteData(time, lineIndex, lineLayer, type, cutDirection, data);
        }

        public static WaypointData DeserializeWaypoint(JsonTextReader reader)
        {
            float time = default;
            int lineIndex = default;
            NoteLineLayer lineLayer = default;
            OffsetDirection offsetDirection = default;
            Dictionary<string, object?> data = new Dictionary<string, object?>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_time":
                        time = (float?)reader.ReadAsDouble() ?? time;
                        break;

                    case "_lineIndex":
                        lineIndex = reader.ReadAsInt32() ?? lineIndex;
                        break;

                    case "_lineLayer":
                        lineLayer = (NoteLineLayer?)reader.ReadAsInt32() ?? lineLayer;
                        break;

                    case "_offsetDirection":
                        offsetDirection = (OffsetDirection?)reader.ReadAsInt32() ?? offsetDirection;
                        break;

                    case "_customData":
                        reader.ReadToDictionary(data);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new WaypointData(time, lineIndex, lineLayer, offsetDirection, data);
        }

        public static ObstacleData DeserializeObstacle(JsonTextReader reader)
        {
            float time = default;
            int lineIndex = default;
            ObstacleType type = default;
            float duration = default;
            int width = default;
            Dictionary<string, object?> data = new Dictionary<string, object?>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_time":
                        time = (float?)reader.ReadAsDouble() ?? time;
                        break;

                    case "_lineIndex":
                        lineIndex = reader.ReadAsInt32() ?? lineIndex;
                        break;

                    case "_type":
                        type = (ObstacleType?)reader.ReadAsInt32() ?? type;
                        break;

                    case "_duration":
                        duration = (float?)reader.ReadAsDouble() ?? duration;
                        break;

                    case "_width":
                        width = reader.ReadAsInt32() ?? width;
                        break;

                    case "_customData":
                        reader.ReadToDictionary(data);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new ObstacleData(time, lineIndex, type, duration, width, data);
        }

        public static SpecialEventsForKeyword DeserializeKeyword(JsonTextReader reader)
        {
            string keyword = string.Empty;
            List<BeatmapEventType> specialEvents = new List<BeatmapEventType>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_keyword":
                        keyword = reader.ReadAsString() ?? keyword;
                        break;

                    case "_specialEvents":
                        reader.Read();
                        if (reader.TokenType != JsonToken.StartArray)
                        {
                            throw new JsonSerializationException("_specialEvents was not array.");
                        }

                        while (true)
                        {
                            int? specialEvent = reader.ReadAsInt32();
                            if (specialEvent.HasValue)
                            {
                                specialEvents.Add((BeatmapEventType)specialEvent);
                            }
                            else
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }
                                else
                                {
                                    throw new JsonSerializationException($"Value in _specialEvents was not int.");
                                }
                            }
                        }

                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new SpecialEventsForKeyword(keyword, specialEvents);
        }

        public static CustomEventData DeserializeCustomEvent(JsonTextReader reader)
        {
            float time = default;
            string type = string.Empty;
            Dictionary<string, object?> data = new Dictionary<string, object?>();
            reader.ReadObject(objectName =>
            {
                switch (objectName)
                {
                    case "_time":
                        time = (float?)reader.ReadAsDouble() ?? time;
                        break;

                    case "_type":
                        type = reader.ReadAsString() ?? type;
                        break;

                    case "_data":
                        reader.ReadToDictionary(data);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            });

            return new CustomEventData(time, type, data);
        }

        private static void DeserializeCustomEventsArray(DeserializeEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "_customEvents")
            {
                JsonTextReader reader = eventArgs.Reader;
                reader.ReadObjectArray(() => eventArgs.CustomEvents.Add(DeserializeCustomEvent(reader)));

                eventArgs.DontAddToDictionary = true;
            }
        }

        private static void ConvertBeatmapSaveDataPreV2_5_0(CustomBeatmapSaveData beatmapSaveData)
        {
            List<BeatmapSaveData.EventData> list = new List<BeatmapSaveData.EventData>(beatmapSaveData.events.Count);
            foreach (EventData eventData in beatmapSaveData.events)
            {
                EventData? newData = null;
                if (eventData.type == BeatmapEventType.Event10)
                {
                    newData = new EventData(eventData.time, BeatmapEventType.BpmChange, eventData.value, eventData.floatValue, eventData.customData);
                }

                if (((global::BeatmapEventType)eventData.type).IsBPMChangeEvent())
                {
                    if (eventData.value != 0)
                    {
                        newData = new EventData(eventData.time, eventData.type, eventData.value, eventData.value, eventData.customData);
                    }
                }
                else
                {
                    newData = new EventData(eventData.time, eventData.type, eventData.value, 1f, eventData.customData);
                }

                list.Add(newData ?? eventData);
            }

            beatmapSaveData._events = list;
        }

        public class DeserializeEventArgs : EventArgs
        {
            public DeserializeEventArgs(
                JsonTextReader reader,
                Dictionary<string, object?> customData,
                List<CustomEventData> customEvents,
                List<EventData> events,
                List<NoteData> notes,
                List<WaypointData> waypoints,
                List<ObstacleData> obstacles,
                List<SpecialEventsForKeyword> keywords)
            {
                Reader = reader;
                CustomData = customData;
                CustomEvents = customEvents;
                Events = events;
                Notes = notes;
                Waypoints = waypoints;
                Obstacles = obstacles;
                Keywords = keywords;
            }

            public bool DontAddToDictionary { get; set; } = false;

            public JsonTextReader Reader { get; }

            public string PropertyName { get; internal set; } = string.Empty;

            public Dictionary<string, object?> CustomData { get; }

            public List<CustomEventData> CustomEvents { get; }

            public List<EventData> Events { get; }

            public List<NoteData> Notes { get; }

            public List<WaypointData> Waypoints { get; }

            public List<ObstacleData> Obstacles { get; }

            public List<SpecialEventsForKeyword> Keywords { get; }
        }

        public new class EventData : BeatmapSaveData.EventData
        {
            internal EventData(float time, BeatmapEventType type, int value, float floatValue, Dictionary<string, object?> customData)
                : base(time, type, value, floatValue)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public class CustomEventData
        {
            internal CustomEventData(float time, string type, Dictionary<string, object?> data)
            {
                this.time = time;
                this.type = type;
                this.data = data;
            }

            public float time { get; }

            public string type { get; }

            public Dictionary<string, object?> data { get; }
        }

        public new class NoteData : BeatmapSaveData.NoteData
        {
            internal NoteData(float time, int lineIndex, NoteLineLayer lineLayer, NoteType type, NoteCutDirection cutDirection, Dictionary<string, object?> customData)
                : base(time, lineIndex, lineLayer, type, cutDirection)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class WaypointData : BeatmapSaveData.WaypointData
        {
            public WaypointData(float time, int lineIndex, NoteLineLayer lineLayer, OffsetDirection offsetDirection, Dictionary<string, object?> customData)
                : base(time, lineIndex, lineLayer, offsetDirection)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }

        public new class ObstacleData : BeatmapSaveData.ObstacleData
        {
            public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width, Dictionary<string, object?> customData)
                : base(time, lineIndex, type, duration, width)
            {
                this.customData = customData;
            }

            public Dictionary<string, object?> customData { get; }
        }
    }
}
