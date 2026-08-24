using System;
using System.Diagnostics;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace InfectionHUD.Patches;

[HarmonyPatch]
public static class Patches
{
    private static readonly ManualLogSource Logger = InfectionHUD.Logger;
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private static readonly Stopwatch _stopwatch2 = Stopwatch.StartNew();

    public static TextMeshProUGUI InfectionText = null!;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CadaverGrowthAI), "ProgressPlayerInfections")]
    public static void CadaverGrowthAI_ProgressPlayerInfections(CadaverGrowthAI __instance)
    {
        // Reduce the update frequency
        if (_stopwatch.ElapsedMilliseconds < 1000) return;

        int playerId = StartOfRound.Instance.thisClientPlayerId;
        PlayerInfection localPlayerInfection = __instance.playerInfections[playerId];

        UpdateInfectionTextByPercent(localPlayerInfection);

        _stopwatch.Restart();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HUDManager), "Start")]
    public static void HUDManager_Start(HUDManager __instance)
    {
        var textGameObject = new GameObject("MyCustomHUDText");

        // Parent it to an existing HUD container so it renders on the canvas. This way, hiding/showing is handled by the base game code
        textGameObject.transform.SetParent(__instance.weightCounter.transform.parent, false);

        InfectionText = textGameObject.AddComponent<TextMeshProUGUI>();
        InfectionText.font = __instance.weightCounter.font;
        InfectionText.fontSize = 14;
        InfectionText.color = Color.green;
        InfectionText.alignment = TextAlignmentOptions.Left;
        InfectionText.text = "";

        InfectionText.rectTransform.localPosition = new Vector3(-20, -30, 0);
    }

    private static void UpdateInfectionTextByPercent(PlayerInfection value)
    {
        Logger.LogDebug($"UpdatingPercentValue called with infection={value.infectionMeter}");
        if (InfectionText == null) return;

        if (value.burstMeter > 0)
        {
            float burst = value.burstMeter;
            InfectionText.text = $"{MathF.Floor(burst * 100)}% burst";
        }
        else
        {
            float infection = Math.Clamp(value.infectionMeter, 0, 1);
            InfectionText.color = Color.Lerp(Color.green, Color.red, infection);
            InfectionText.text = infection == 0 ? "" : $"{MathF.Floor(infection * 100)}% infected";
        }
    }

#if DEBUG
    private static float kek = 0f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public static void UpdatePrefix(PlayerControllerB __instance)
    {
        // Simulator helper patch
        if (_stopwatch2.ElapsedMilliseconds < 1000) return;

        kek += 0.05f;
        if (kek > 1f)
        {
            kek = 0;
        }
        UpdateInfectionTextByPercent(new PlayerInfection
        {
            infectionMeter = kek,
        });
        _stopwatch2.Restart();
    }
#endif
}
