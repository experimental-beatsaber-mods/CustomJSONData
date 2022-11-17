using System;
using System.Collections.Generic;
using IPA.Utilities;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapDataSortedListForTypeAndIds<TBase> : BeatmapDataSortedListForTypeAndIds<TBase>
        where TBase : BeatmapDataItem
    {
        private static readonly FieldAccessor<BeatmapDataSortedListForTypeAndIds<TBase>, Dictionary<ValueTuple<Type, int>, ISortedList<TBase>>>.Accessor _itemsAccessor =
            FieldAccessor<BeatmapDataSortedListForTypeAndIds<TBase>, Dictionary<ValueTuple<Type, int>, ISortedList<TBase>>>.GetAccessor(nameof(_items));

        public CustomBeatmapDataSortedListForTypeAndIds(BeatmapDataSortedListForTypeAndIds<TBase> original)
        {
            BeatmapDataSortedListForTypeAndIds<TBase> @this = this;
            _itemsAccessor(ref @this) = _itemsAccessor(ref original);
            _sortedListsDataProcessors.Add(typeof(CustomEventData), null);
        }

        // GetType is a real stinky way of indexing stuff
        public override LinkedListNode<TBase> InsertItem(TBase item)
        {
            LinkedListNode<TBase> linkedListNode = GetList(CustomBeatmapData.GetCustomType(item), item.subtypeGroupIdentifier).Insert(item);
            _itemToNodeMap[item] = linkedListNode;
            return linkedListNode;
        }

        public override void RemoveItem(TBase item)
        {
            ISortedList<TBase> list = GetList(CustomBeatmapData.GetCustomType(item), item.subtypeGroupIdentifier);
            if (_itemToNodeMap.TryGetValue(item, out LinkedListNode<TBase> node))
            {
                list.Remove(node);
            }
        }
    }
}
