using System;
using System.Collections.Generic;
using IPA.Utilities;

namespace CustomJSONData.CustomBeatmap
{
    public sealed class CustomBeatmapData : BeatmapData
    {
        private static readonly FieldAccessor<BeatmapData, BeatmapDataSortedListForTypes<BeatmapDataItem>>.Accessor _beatmapDataItemsPerTypeAccessor =
            FieldAccessor<BeatmapData, BeatmapDataSortedListForTypes<BeatmapDataItem>>.GetAccessor(nameof(_beatmapDataItemsPerType));

        public CustomBeatmapData(
            int numberOfLines,
            Dictionary<string, object?> customData,
            Dictionary<string, object?> beatmapCustomData,
            Dictionary<string, object?> levelCustomData)
            : base(numberOfLines)
        {
            BeatmapData @this = this;
            _beatmapDataItemsPerTypeAccessor(ref @this) =
                new CustomBeatmapDataSortedListForTypes<BeatmapDataItem>(_beatmapDataItemsPerTypeAccessor(ref @this));
            _beatmapDataItemsPerType.AddList(new SortedList<CustomEventData, BeatmapDataItem>(null));
            this.customData = customData;
            this.beatmapCustomData = beatmapCustomData;
            this.levelCustomData = levelCustomData;
        }

        public Dictionary<string, object?> customData { get; }

        public Dictionary<string, object?> beatmapCustomData { get; }

        public Dictionary<string, object?> levelCustomData { get; }

        public static Type GetCustomType(object item)
        {
            Type type = item.GetType();
            if (item is not CustomEventData && item is ICustomData)
            {
                type = type.BaseType
                       ?? throw new InvalidOperationException($"[{item.GetType().FullName}] does not have a base type.");
            }

            return type;
        }

        public void InsertCustomEventData(CustomEventData customEventData)
        {
            _beatmapDataItemsPerType.InsertItem(customEventData);
            _allBeatmapData.Insert(customEventData);
        }

        public override BeatmapData GetCopy()
        {
            CustomBeatmapData beatmapData = new(
                _numberOfLines,
                customData.Copy(),
                beatmapCustomData.Copy(),
                levelCustomData.Copy());
            foreach (BeatmapDataItem beatmapDataItem in allBeatmapDataItems)
            {
                switch (beatmapDataItem)
                {
                    case BeatmapEventData beatmapEventData:
                        beatmapData.InsertBeatmapEventData(beatmapEventData);
                        break;
                    case BeatmapObjectData beatmapObjectData:
                        beatmapData.AddBeatmapObjectData(beatmapObjectData);
                        break;
                    case CustomEventData customEventData:
                        beatmapData.InsertCustomEventData(customEventData);
                        break;
                }
            }

            return beatmapData;
        }

        public override BeatmapData GetFilteredCopy(Func<BeatmapDataItem, BeatmapDataItem> processDataItem)
        {
            _isCreatingFilteredCopy = true;
            CustomBeatmapData beatmapData = new(
                _numberOfLines,
                customData.Copy(),
                beatmapCustomData.Copy(),
                levelCustomData.Copy());
            foreach (BeatmapDataItem beatmapDataItem in allBeatmapDataItems)
            {
                BeatmapDataItem beatmapDataItem2 = processDataItem(beatmapDataItem.GetCopy());
                if (beatmapDataItem2 != null)
                {
                    switch (beatmapDataItem2)
                    {
                        case BeatmapEventData beatmapEventData:
                            beatmapData.InsertBeatmapEventData(beatmapEventData);
                            break;
                        case BeatmapObjectData beatmapObjectData:
                            beatmapData.AddBeatmapObjectData(beatmapObjectData);
                            break;
                        case CustomEventData customEventData:
                            beatmapData.InsertCustomEventData(customEventData);
                            break;
                    }
                }
            }

            _isCreatingFilteredCopy = false;
            return beatmapData;
        }
    }
}
