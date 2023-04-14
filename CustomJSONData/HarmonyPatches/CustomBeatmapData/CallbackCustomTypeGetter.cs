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
