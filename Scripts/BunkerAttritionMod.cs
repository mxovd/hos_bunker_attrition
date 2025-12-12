using System;
using HarmonyLib;
using UnityEngine;

class BunkerAttritionMod : GameModification
{
    private Harmony _harmony;

    public BunkerAttritionMod(Mod mod) : base(mod)
    {
        Log("Registering Bunker Attrition...");
    }

    public override void OnModInitialization(Mod mod)
    {
        Log("Initializing Bunker Attrition...");

        PatchGame();
    }

    public override void OnModUnloaded()
    {
        Log("Unloading Bunker Attrition...");

        _harmony?.UnpatchAll(_harmony.Id);
    }

    private void PatchGame()
    {
        Log("Patching...");

        _harmony = new Harmony("com.hexofsteel.bunker-attrition");
        _harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Unit), "NextTurn")]
static class BunkerAttrition_UnitNextTurn_Patch
{
    // Keeps bunkers from staying invincible when stranded behind enemy lines.
    private const float DamagePercentPerTurn = 0.20f; // 20% of max HP each turn
    private const int MinimumDamagePerTurn = 4;       // ensure progress even on low-HP bunkers

    static void Postfix(Unit __instance)
    {
        try
        {
            if (__instance == null)
            {
                return;
            }

            if (!IsBunker(__instance))
            {
                return;
            }

            Tile tile = __instance.unitGO?.tileGO?.tile;
            Player bunkerOwner = __instance.OwnerPlayer;
            Player terrainOwner = tile?.terrainOwner;

            if (tile == null || bunkerOwner == null || terrainOwner == null)
            {
                return;
            }

            // Only apply when the bunker sits on enemy-owned land.
            if (!bunkerOwner.IsAtWarWith(terrainOwner))
            {
                return;
            }

            int damage = Math.Max(MinimumDamagePerTurn,
                (int)Math.Round(__instance.MaxHP * DamagePercentPerTurn, MidpointRounding.AwayFromZero));

            __instance.CurrHP = Math.Max(0, __instance.CurrHP - damage);

            if (__instance.CurrHP > 0)
            {
                return;
            }

            // Bunker gives up once it runs out of HP.
            if (__instance.unitGO != null)
            {
                __instance.unitGO.DestroyUnit(p_playAnimation: false, p_sendRPC: true);
            }
            else
            {
                // Fallback: remove the unit to avoid leaving a dead entry.
                bunkerOwner.RemoveUnit(__instance, p_sendRPC: true);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[BunkerAttrition] Failed to apply attrition: {ex}");
        }
    }

    private static bool IsBunker(Unit unit)
    {
        // Base game typically checks bunkers via Name.Contains("Bunker"), so mirror that.
        return unit != null && !string.IsNullOrEmpty(unit.Name)
            && (unit.Name.Contains("Bunker") || unit.Name.IndexOf("bunker", StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
