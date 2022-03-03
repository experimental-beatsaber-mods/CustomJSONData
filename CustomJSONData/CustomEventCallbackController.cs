using System.Collections.Generic;
using CustomJSONData.CustomBeatmap;
using JetBrains.Annotations;
using Zenject;

namespace CustomJSONData
{
    public class CustomEventCallbackController : ILateTickable
    {
        private readonly List<CustomEventCallbackData> _customEventCallbackData = new();

        private readonly BeatmapObjectCallbackController _beatmapObjectCallbackController;

        private readonly float _spawningStartTime;

        private readonly IAudioTimeSource _audioTimeSource;

        private CustomBeatmapData _beatmapData;

        private CustomEventCallbackController(
            BeatmapObjectCallbackController beatmapObjectCallbackController,
            BeatmapObjectCallbackController.InitData initData,
            IAudioTimeSource audioTimeSource)
        {
            _beatmapObjectCallbackController = beatmapObjectCallbackController;
            _spawningStartTime = initData.spawningStartTime;
            _beatmapData = (CustomBeatmapData)initData.beatmapData;
            _audioTimeSource = audioTimeSource;
        }

        public delegate void CustomEventCallback(CustomEventData eventData);

        [PublicAPI]
        public CustomEventCallbackData AddCustomEventCallback(CustomEventCallback callback, float aheadTime = 0, bool callIfBeforeStartTime = true)
        {
            CustomEventCallbackData customEventCallbackData = new(callback, aheadTime, callIfBeforeStartTime);
            _customEventCallbackData.Add(customEventCallbackData);
            return customEventCallbackData;
        }

        [PublicAPI]
        public void RemoveBeatmapEventCallback(CustomEventCallbackData callbackData)
        {
            _customEventCallbackData.Remove(callbackData);
        }

        [PublicAPI]
        public void InvokeCustomEvent(CustomEventData customEventData)
        {
            foreach (CustomEventCallbackData t in _customEventCallbackData)
            {
                t.callback(customEventData);
            }
        }

        public void LateTick()
        {
            if (!_beatmapObjectCallbackController.enabled)
            {
                return;
            }

            foreach (CustomEventCallbackData customEventCallbackData in _customEventCallbackData)
            {
                while (customEventCallbackData.nextEventIndex < _beatmapData.customEventsData.Count)
                {
                    CustomEventData customEventData = _beatmapData.customEventsData[customEventCallbackData.nextEventIndex];
                    if (customEventData.time - customEventCallbackData.aheadTime >= _audioTimeSource?.songTime)
                    {
                        break;
                    }

                    // skip events before song start
                    if (customEventData.time >= _spawningStartTime || customEventCallbackData.callIfBeforeStartTime)
                    {
                        customEventCallbackData.callback(customEventData);
                    }

                    customEventCallbackData.nextEventIndex++;
                }
            }
        }

        internal void SetNewBeatmapData(IReadonlyBeatmapData beatmapData)
        {
            _beatmapData = (CustomBeatmapData)beatmapData;

            foreach (CustomEventCallbackData customEventCallbackData in _customEventCallbackData)
            {
                customEventCallbackData.nextEventIndex = 0;
            }
        }

        public class CustomEventCallbackData
        {
            public CustomEventCallbackData(CustomEventCallback callback, float aheadTime, bool callIfBeforeStartTime)
            {
                this.callback = callback;
                this.aheadTime = aheadTime;
                this.callIfBeforeStartTime = callIfBeforeStartTime;
                nextEventIndex = 0;
            }

            public CustomEventCallback callback { get; }

            public float aheadTime { get; }

            public int nextEventIndex { get; set; }

            public bool callIfBeforeStartTime { get; }
        }
    }
}
