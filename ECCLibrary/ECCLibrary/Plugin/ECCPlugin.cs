using System.Reflection;

namespace ECCLibrary;

[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("com.snmodding.nautilus", "1.0.0.50")]
internal class ECCPlugin : BaseUnityPlugin
{
    public static ECCConfig config = OptionsPanelHandler.RegisterModOptions<ECCConfig>();

    public static ManualLogSource logger;

    private void Awake()
    {
        logger = Logger;

        logger.LogInfo($"Patching {PluginInfo.Name} ({PluginInfo.GUID}) v{PluginInfo.Version}...");

        Harmony harmony = new Harmony(PluginInfo.GUID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        logger.LogInfo("Attempting to create example creatures....");

        Examples.ExamplePatcher.PatchExampleCreatures();

        logger.LogInfo($"Finished patching {PluginInfo.Name}!");
    }
}