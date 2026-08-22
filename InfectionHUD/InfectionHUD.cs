using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace InfectionHUD;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class InfectionHUD : BaseUnityPlugin
{
    public static InfectionHUD Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    private readonly Harmony _harmony;
    private int _frames;
    private readonly Guid _guid = Guid.NewGuid();

    // Bad practice to use ctor from Unity perspective...so be careful
    public InfectionHUD()
    {
        Logger = base.Logger;
        Instance = this;
        Logger.LogInfo("Ctor INFECTION-HUD");
        
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded! instance guid={_guid.ToString()}");

        string pluginsDllPath = Path.Join(Paths.PluginPath, Info.Metadata.GUID + ".dll");
        if (IsDevHotReload && File.Exists(pluginsDllPath))
        {
            Logger.LogWarning("Found existing DLL in /plugins. Removing to avoid conflict with ScriptEngine...");
            File.Delete(pluginsDllPath);
        }

        Logger.LogDebug("Patching...");

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Logger.LogDebug("Finished patching!");
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        
    }

    // ReSharper disable once UnusedMember.Local
    private void OnDestroy()
    {
        Logger.LogInfo($"OnDestroy called on instance guid={_guid.ToString()}");
        if (IsDevHotReload) // We want to unpatch only when in dev mode. Skip in PROD
        {
            Logger.LogDebug("Unpatching...");

            _harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }

    private void Update()
    {
        _frames++;
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(20, 20, 800, 30), $"INFECTION HUD ALIVE | frames={_frames}");
    }

    private bool IsDevHotReload => gameObject.name.Contains("ScriptEngine");
}
