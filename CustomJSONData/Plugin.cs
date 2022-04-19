using HarmonyLib;
using IPA;
using JetBrains.Annotations;

namespace CustomJSONData
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        private readonly Harmony _harmonyInstance = new("aeroluna.CustomJSONData");

#pragma warning disable CA1822
        [UsedImplicitly]
        [Init]
        public Plugin(IPA.Logging.Logger l)
        {
            Logger.logger = l;
        }

        [UsedImplicitly]
        [OnEnable]
        public void OnEnable()
        {
            _harmonyInstance.PatchAll(typeof(Plugin).Assembly);
        }

        [UsedImplicitly]
        [OnDisable]
        public void OnDisable()
        {
            _harmonyInstance.UnpatchSelf();
        }
#pragma warning restore CA1822
    }
}
