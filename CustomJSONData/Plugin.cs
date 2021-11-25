using System.Reflection;
using HarmonyLib;
using IPA;
using JetBrains.Annotations;

namespace CustomJSONData
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
#pragma warning disable CA1822
        [UsedImplicitly]
        [Init]
        public void Init(IPA.Logging.Logger l)
        {
            Logger.logger = l;
        }

        [UsedImplicitly]
        [OnStart]
        public void OnApplicationStart()
        {
            Harmony harmony = new("com.aeroluna.BeatSaber.CustomJSONData");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
#pragma warning restore CA1822
    }
}
