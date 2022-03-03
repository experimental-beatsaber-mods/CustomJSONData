using System.Reflection;
using CustomJSONData.Installers;
using HarmonyLib;
using IPA;
using JetBrains.Annotations;
using SiraUtil.Zenject;

namespace CustomJSONData
{
    [Plugin(RuntimeOptions.DynamicInit)]
    internal class Plugin
    {
        private readonly Harmony _harmonyInstance = new("com.aeroluna.CustomJSONData");

#pragma warning disable CA1822
        [UsedImplicitly]
        [Init]
        public Plugin(IPA.Logging.Logger l, Zenjector zenjector)
        {
            Logger.logger = l;
            zenjector.Install<CallbackControllerInstaller>(Location.Player);
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
