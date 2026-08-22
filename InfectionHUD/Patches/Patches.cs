using System.Diagnostics;
using GameNetcodeStuff;
using HarmonyLib;

namespace InfectionHUD.Patches;

[HarmonyPatch]
public static class Patches
{
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CadaverGrowthAI), nameof(CadaverGrowthAI.ProgressPlayerInfections))]
    public static void CadaverGrowthAI_ProgressPlayerInfections(CadaverGrowthAI __instance)
    {
        InfectionHUD.Logger.LogDebug($"Infections count={__instance.playerInfections.Length}");
        foreach (PlayerInfection infection in __instance.playerInfections)
        {
            InfectionHUD.Logger.LogDebug($"Infection meter: {infection.infectionMeter}");
            InfectionHUD.Logger.LogDebug($"Burst meter: {infection.burstMeter}");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public static void UpdatePrefix(PlayerControllerB __instance)
    {
        if (_stopwatch.ElapsedMilliseconds < 1000) return;

        InfectionHUD.Logger.LogInfo("PlayerControllerB.Update ran...Patch is working!");
        _stopwatch.Restart();
    }
}
