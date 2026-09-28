// CallbacksInTime.CallCallbacks used to dispatch by reflecting the item's real runtime type
// (a plain `.GetType()` call in its IL), which meant a custom subtype (e.g. CustomNoteData)
// would never match callbacks registered for its vanilla base type (NoteData) unless something
// made GetType() lie about it -- which is what this transpiler did, swapping the call for
// CustomBeatmapData.GetCustomType (returns the base type for any ICustomData item except
// CustomEventData, which has no vanilla counterpart).
//
// As of 1.45.1, BeatmapDataItem itself carries a real itemTypeId (its own runtime type) and a
// baseTypeId (its direct base type's id, assigned once in the base constructor via
// BeatmapDataItem.GetTypeId), and CallCallbacks(BeatmapDataItem) dispatches on BOTH ids. Every
// affected custom subtype here (CustomNoteData : NoteData, CustomObstacleData : ObstacleData,
// CustomSliderData : SliderData, the CustomLight*BeatmapEventData / CustomBPMChangeBeatmapEventData
// / CustomBasicBeatmapEventData / CustomColorBoostBeatmapEventData types) is a direct subclass of
// its real vanilla type, so baseTypeId already resolves to exactly what GetCustomType used to fake
// -- the callback-matching problem this patch existed for doesn't exist anymore, natively. The
// transpiler's IL search (a `callvirt object.GetType()`) also no longer matches anything in the
// method body at all, since it doesn't call GetType() any more either way -- Harmony throws
// "Cannot set values at invalid position" trying to .Set() an unmatched position, which used to
// abort ALL of CustomJSONData's Harmony patches (PatchAll stops at the first failing patch class).
#if PRE_V1_45_1
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(CallbacksInTime))]
    internal static class CallbackCustomTypeGetter
    {
        private static readonly MethodInfo _getType = AccessTools.Method(typeof(object), nameof(GetType));
        private static readonly MethodInfo _getCustomType = AccessTools.Method(typeof(CustomBeatmapData), nameof(CustomBeatmapData.GetCustomType));

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CallbacksInTime.CallCallbacks), typeof(BeatmapDataItem))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _getType))
                .Set(OpCodes.Call, _getCustomType)
                .InstructionEnumeration();
        }
    }
}
#endif
