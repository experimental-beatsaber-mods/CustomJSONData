using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using JetBrains.Annotations;

namespace CustomJSONData.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataMirrorTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal class BeatDataMirrorTransformCreateTransformData
    {
        private static readonly ConstructorInfo _beatmapDataCtor = AccessTools.FirstConstructor(typeof(BeatmapData), _ => true);
        private static readonly MethodInfo _copyCustomData = AccessTools.Method(typeof(BeatDataMirrorTransformCreateTransformData), nameof(CopyCustomBeatmapData));

        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Newobj, _beatmapDataCtor))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(new CodeInstruction(OpCodes.Call, _copyCustomData))
                .InstructionEnumeration();
        }

        private static BeatmapData CopyCustomBeatmapData(int numberOfLines, IReadonlyBeatmapData beatmapData)
        {
            if (beatmapData is CustomBeatmapData customBeatmapData)
            {
                return customBeatmapData.BaseCopy();
            }

            return new BeatmapData(numberOfLines);
        }
    }
}
