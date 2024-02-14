using BepInEx;
using HarmonyLib;

namespace InfiniteShotgunReload;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	void Awake()
	{
		// Plugin startup logic
		Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
		Harmony.CreateAndPatchAll(typeof(ShotgunPatch));
			
	}
}