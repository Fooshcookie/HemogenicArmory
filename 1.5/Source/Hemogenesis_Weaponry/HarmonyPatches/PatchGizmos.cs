using System.Collections.Generic;
using HarmonyLib;
using Hemogenesis_Weaponry.Comps;
using Verse;

namespace Hemogenesis_Weaponry.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_EquipmentTracker), "GetGizmos")]
public static class PatchGizmos
{
    [HarmonyPostfix]
    public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_EquipmentTracker __instance)
    {
        foreach (Gizmo gizmo in __result)
        {
            yield return gizmo;
        }

        if (!__instance.pawn.IsPlayerControlled) yield break;
        foreach (ThingWithComps thingWithComps in __instance.AllEquipmentListForReading)
        {
            foreach (Gizmo gizmo in thingWithComps.TryGetComp<CompHemoCharge>()?.CompGetEquipmentGizmosExtra() ?? [])
            {
                yield return gizmo;
            }
        }
    }
}
