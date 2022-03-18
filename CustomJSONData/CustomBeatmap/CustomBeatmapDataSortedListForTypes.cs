using System;
using System.Collections.Generic;
using IPA.Utilities;

namespace CustomJSONData.CustomBeatmap
{
    public class CustomBeatmapDataSortedListForTypes<T> : BeatmapDataSortedListForTypes<T>
        where T : BeatmapDataItem
    {
        private static readonly FieldAccessor<BeatmapDataSortedListForTypes<T>, Dictionary<Type, ISortedList<T>>>.Accessor _itemsAccessor =
            FieldAccessor<BeatmapDataSortedListForTypes<T>, Dictionary<Type, ISortedList<T>>>.GetAccessor(nameof(_items));

        public CustomBeatmapDataSortedListForTypes(BeatmapDataSortedListForTypes<T> original)
        {
            BeatmapDataSortedListForTypes<T> @this = this;
            _itemsAccessor(ref @this) = _itemsAccessor(ref original);
        }

        // GetType is a real stinky way of indexing stuff
        public override void InsertItem(T item)
        {
            GetList(CustomBeatmapData.GetCustomType(item)).Insert(item);
        }

        public override void RemoveItem(T item)
        {
            ISortedList<T> list = GetList(CustomBeatmapData.GetCustomType(item));
            LinkedListNode<T> node = list.NodeForItem(item);
            list.Remove(node);
        }
    }
}
