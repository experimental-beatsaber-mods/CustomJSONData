using System;
using System.Collections.Generic;
using HarmonyLib;
using IPA.Utilities;
using JetBrains.Annotations;

namespace CustomJSONData.CustomBeatmap
{
    [HarmonyPatch(typeof(BeatmapData))]
    public sealed class CustomBeatmapData : BeatmapData
    {
        private static readonly FieldAccessor<BeatmapData, BeatmapDataSortedListForTypeAndIds<BeatmapDataItem>>.Accessor _beatmapDataItemsPerTypeAccessor =
            FieldAccessor<BeatmapData, BeatmapDataSortedListForTypeAndIds<BeatmapDataItem>>.GetAccessor(nameof(_beatmapDataItemsPerTypeAndId));

        private readonly List<BeatmapObjectData> _beatmapObjectDatas = new();
        private readonly List<BeatmapEventData> _beatmapEventDatas = new();
        private readonly List<CustomEventData> _customEventDatas = new();

        public CustomBeatmapData(
            int numberOfLines,
            bool version2_6_0AndEarlier,
            CustomData customData,
            CustomData beatmapCustomData,
            CustomData levelCustomData)
            : base(numberOfLines)
        {
            BeatmapData @this = this;
            _beatmapDataItemsPerTypeAccessor(ref @this) =
                new CustomBeatmapDataSortedListForTypeAndIds<BeatmapDataItem>(_beatmapDataItemsPerTypeAccessor(ref @this));
            this.version2_6_0AndEarlier = version2_6_0AndEarlier;
            this.customData = customData;
            this.beatmapCustomData = beatmapCustomData;
            this.levelCustomData = levelCustomData;
        }

        public bool version2_6_0AndEarlier { get; }

        public CustomData customData { get; }

        public CustomData beatmapCustomData { get; }

        public CustomData levelCustomData { get; }

        [PublicAPI]
        public IReadOnlyList<BeatmapObjectData> beatmapObjectDatas => _beatmapObjectDatas;

        [PublicAPI]
        public IReadOnlyList<BeatmapEventData> beatmapEventDatas => _beatmapEventDatas;

        [PublicAPI]
        public IReadOnlyList<CustomEventData> customEventDatas => _customEventDatas;

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

        public override void AddBeatmapObjectData(BeatmapObjectData beatmapObjectData)
        {
            _beatmapObjectDatas.Add(beatmapObjectData);
            base.AddBeatmapObjectData(beatmapObjectData);
        }

        // InOrder variants do not use a virtual call to AddBeatmapObjectData so they will not call the above method
        public override void AddBeatmapObjectDataInOrder(BeatmapObjectData beatmapObjectData)
        {
            _beatmapObjectDatas.Add(beatmapObjectData);
            base.AddBeatmapObjectDataInOrder(beatmapObjectData);
        }

        public override void InsertBeatmapEventData(BeatmapEventData beatmapEventData)
        {
            _beatmapEventDatas.Add(beatmapEventData);
            base.InsertBeatmapEventData(beatmapEventData);
        }

        public override void InsertBeatmapEventDataInOrder(BeatmapEventData beatmapEventData)
        {
            _beatmapEventDatas.Add(beatmapEventData);
            base.InsertBeatmapEventDataInOrder(beatmapEventData);
        }

        public void InsertCustomEventData(CustomEventData customEventData)
        {
            _customEventDatas.Add(customEventData);
            LinkedListNode<BeatmapDataItem> node = _beatmapDataItemsPerTypeAndId.InsertItem(customEventData);
            if (updateAllBeatmapDataOnInsert)
            {
                InsertToAllBeatmapData(customEventData, node);
            }
        }

        // what in gods name is the point of this
        public void InsertCustomEventDataInOrder(CustomEventData customEventData)
        {
            InsertCustomEventData(customEventData);
            InsertToAllBeatmapData(customEventData);
        }

        public override BeatmapData GetCopy()
        {
            CustomBeatmapData beatmapData = new(
                _numberOfLines,
                version2_6_0AndEarlier,
                customData.Copy(),
                beatmapCustomData.Copy(),
                levelCustomData.Copy());
            foreach (BeatmapDataItem beatmapDataItem in allBeatmapDataItems)
            {
                BeatmapDataItem copy = beatmapDataItem.GetCopy();
                switch (copy)
                {
                    case BeatmapEventData beatmapEventData:
                        beatmapData.InsertBeatmapEventDataInOrder(beatmapEventData);
                        break;
                    case BeatmapObjectData beatmapObjectData:
                        beatmapData.AddBeatmapObjectDataInOrder(beatmapObjectData);
                        break;
                    case CustomEventData customEventData:
                        beatmapData.InsertCustomEventDataInOrder(customEventData);
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
                version2_6_0AndEarlier,
                customData.Copy(),
                beatmapCustomData.Copy(),
                levelCustomData.Copy());
            foreach (BeatmapDataItem beatmapDataItem in allBeatmapDataItems)
            {
                BeatmapDataItem copy = processDataItem(beatmapDataItem.GetCopy());
                if (copy != null)
                {
                    switch (copy)
                    {
                        case BeatmapEventData beatmapEventData:
                            beatmapData.InsertBeatmapEventDataInOrder(beatmapEventData);
                            break;
                        case BeatmapObjectData beatmapObjectData:
                            beatmapData.AddBeatmapObjectDataInOrder(beatmapObjectData);
                            break;
                        case CustomEventData customEventData:
                            beatmapData.InsertCustomEventDataInOrder(customEventData);
                            break;
                    }
                }
            }

            _isCreatingFilteredCopy = false;
            return beatmapData;
        }
    }
}
