using BepInEx;
using HarmonyLib;

namespace InfiniteShotgunReload;

[BepInDependency("Hypick.BetterShotgun", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	void Awake()
	{
		// Plugin startup logic
		Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
		
		// check if BetterShotgun is installed to not patch
		if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Hypick.BetterShotgun"))
			Logger.LogWarning("BetterShotgun is installed, use the ReloadNoLimit config option instead and uninstall this mod.");
		else
			Harmony.CreateAndPatchAll(typeof(ShotgunPatch), PluginInfo.PLUGIN_GUID);
	}
}