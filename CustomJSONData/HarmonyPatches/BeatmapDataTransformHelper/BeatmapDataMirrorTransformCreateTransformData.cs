namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(BeatmapDataMirrorTransform))]
    [HarmonyPatch("CreateTransformedData")]
    internal class BeatDataMirrorTransformCreateTransformData
    {
        private static readonly ConstructorInfo _beatmapDataCtor = AccessTools.FirstConstructor(typeof(BeatmapData), _ => true);
        private static readonly MethodInfo _copyCustomData = AccessTools.Method(typeof(BeatDataMirrorTransformCreateTransformData), nameof(CopyCustomBeatmapData));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundCtor = false;
#pragma warning disable CS0252
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundCtor &&
                    instructionList[i].opcode == OpCodes.Newobj &&
                    instructionList[i].operand == _beatmapDataCtor)
                {
                    foundCtor = true;
                    instructionList[i] = new CodeInstruction(OpCodes.Ldarg_0);
                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, _copyCustomData));
                }
            }
#pragma warning restore CS0252
            if (!foundCtor)
            {
                Logger.Log("Failed to patch BeatmapDataMirrorTransform!", IPA.Logging.Logger.Level.Error);
            }

            return instructionList.AsEnumerable();
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
