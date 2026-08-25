using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace InfectionHUD;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class InfectionHUD : BaseUnityPlugin
{
    public static InfectionHUD Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;

    private readonly Harmony _harmony;

    // Bad practice to use ctor from Unity perspective...so be careful
    public InfectionHUD()
    {
        Logger = base.Logger;
        Instance = this;

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        Logger.LogDebug("Patching...");

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogDebug("Finished patching!");
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        // This fixes the dev hot-reload for the InfectionText, since doing a reload doesn't trigger HUDManager to get destroyed
        // Only this dll's plugin gets destroyed!
        if (HUDManager.Instance != null && HUDManager.Instance.gameObject.activeInHierarchy)
        {
            Patches.Patches.HUDManager_Start(HUDManager.Instance);
        }
    }

#if DEBUG
    private void Update()
    {
        if (Keyboard.current.f7Key.wasPressedThisFrame)
        {
            Patches.Patches.InfectionText.text = "<infection_reset_by_f7>";
        }
    }
#endif

    // ReSharper disable once UnusedMember.Local
    private void OnDestroy()
    {
        Logger.LogDebug("OnDestroy called");
        if (IsDevHotReload) // We want to unpatch only when in dev mode. Skip in PROD
        {
            Logger.LogDebug("Unpatching...");

            _harmony?.UnpatchSelf();
            Destroy(Patches.Patches.InfectionText?.gameObject);

            Logger.LogDebug("Finished unpatching!");
        }
    }

    private bool IsDevHotReload => gameObject.name.Contains("ScriptEngine");
}
