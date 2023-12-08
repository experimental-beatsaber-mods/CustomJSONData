namespace CustomJSONData.CustomBeatmap
{
    public class CustomLightColorBeatmapEventData : LightColorBeatmapEventData, ICustomData
    {
        public CustomLightColorBeatmapEventData(
            float time,
            int groupId,
            int elementId,
            BeatmapEventTransitionType transitionType,
            EnvironmentColorType colorType,
            float brightness,
            int strobeBeatFrequency,
#if LATEST
            float strobeBrightness,
            bool strobeFade,
#endif
            CustomData customData)
            : base(
                time,
                groupId,
                elementId,
                transitionType,
                colorType,
                brightness,
#if LATEST
                strobeBeatFrequency,
                strobeBrightness,
                strobeFade)
#else
                strobeBeatFrequency)
#endif
        {
            this.customData = customData;
        }

        public CustomData customData { get; }

        public override BeatmapDataItem GetCopy()
        {
            return new CustomLightColorBeatmapEventData(
                time,
                groupId,
                elementId,
                transitionType,
                colorType,
                brightness,
                strobeBeatFrequency,
#if LATEST
                strobeBrightness,
                strobeFade,
#endif
                customData.Copy());
        }
    }
}
