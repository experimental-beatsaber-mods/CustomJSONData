namespace CustomJSONData.CustomBeatmap
{
    using System.Collections.Generic;
    using System.Reflection;

    public class CustomBeatmapData : BeatmapData
    {
        private static MethodInfo? _copyBeatmapObjects;
        private static MethodInfo? _copyBeatmapEvents;
        private static MethodInfo? _copyAvailableSpecialEventsPerKeywordDictionary;

        public CustomBeatmapData(
            int numberOfLines,
            List<CustomEventData> customEventsData,
            Dictionary<string, object?> customData,
            Dictionary<string, object?> beatmapCustomData,
            Dictionary<string, object?> levelCustomData)
            : base(numberOfLines)
        {
            this.customEventsData = customEventsData;
            this.customData = customData;
            this.beatmapCustomData = beatmapCustomData;
            this.levelCustomData = levelCustomData;
        }

        public List<CustomEventData> customEventsData { get; }

        public Dictionary<string, object?> customData { get; }

        public Dictionary<string, object?> beatmapCustomData { get; }

        public Dictionary<string, object?> levelCustomData { get; }

        private static MethodInfo CopyBeatmapObjectsMethod
        {
            get
            {
                if (_copyBeatmapObjects == null)
                {
                    _copyBeatmapObjects = typeof(BeatmapData).GetMethod("CopyBeatmapObjects", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyBeatmapObjects;
            }
        }

        private static MethodInfo CopyBeatmapEventsMethod
        {
            get
            {
                if (_copyBeatmapEvents == null)
                {
                    _copyBeatmapEvents = typeof(BeatmapData).GetMethod("CopyBeatmapEvents", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyBeatmapEvents;
            }
        }

        private static MethodInfo CopyAvailableSpecialEventsPerKeywordDictionaryMethod
        {
            get
            {
                if (_copyAvailableSpecialEventsPerKeywordDictionary == null)
                {
                    _copyAvailableSpecialEventsPerKeywordDictionary = typeof(BeatmapData).GetMethod("CopyAvailableSpecialEventsPerKeywordDictionary", BindingFlags.Static | BindingFlags.NonPublic);
                }

                return _copyAvailableSpecialEventsPerKeywordDictionary;
            }
        }

        public static void CopyBeatmapObjects(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyBeatmapObjectsMethod.Invoke(null, new object[] { src, dst });
        }

        public static void CopyBeatmapEvents(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyBeatmapEventsMethod.Invoke(null, new object[] { src, dst });
        }

        public static void CopyAvailableSpecialEventsPerKeywordDictionary(IReadonlyBeatmapData src, BeatmapData dst)
        {
            CopyAvailableSpecialEventsPerKeywordDictionaryMethod.Invoke(null, new object[] { src, dst });
        }

        public override BeatmapData GetCopy()
        {
            CustomBeatmapData customBeatmapData = BaseCopy();
            CopyBeatmapObjects(this, customBeatmapData);
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            return customBeatmapData;
        }

        public override BeatmapData GetCopyWithoutEvents()
        {
            CustomBeatmapData customBeatmapData = BaseCopy();
            CopyBeatmapObjects(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            return customBeatmapData;
        }

        public override BeatmapData GetCopyWithoutBeatmapObjects()
        {
            CustomBeatmapData customBeatmapData = BaseCopy();
            CopyBeatmapEvents(this, customBeatmapData);
            CopyAvailableSpecialEventsPerKeywordDictionary(this, customBeatmapData);
            return customBeatmapData;
        }

        internal CustomBeatmapData BaseCopy()
        {
            List<CustomEventData> customEventsDataCopy = customEventsData.ConvertAll(n => n.GetCopy());
            CustomBeatmapData customBeatmapData = new CustomBeatmapData(_beatmapLinesData.Length, customEventsDataCopy, customData.Copy(), beatmapCustomData.Copy(), levelCustomData.Copy());
            return customBeatmapData;
        }
    }
}
