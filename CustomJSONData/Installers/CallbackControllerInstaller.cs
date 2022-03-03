using CustomJSONData.HarmonyPatches;
using JetBrains.Annotations;
using Zenject;

namespace CustomJSONData.Installers
{
    [UsedImplicitly]
    internal class CallbackControllerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CustomEventCallbackController>().AsSingle();
            Container.BindInterfacesTo<BeatmapObjectCallbackControllerSetNewBeatmapData>().AsSingle();
        }
    }
}
