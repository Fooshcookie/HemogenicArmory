using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace Hemogenesis_Weaponry.Comps;

public class HemoChargeUtil
{
    public static ConditionalWeakTable<Thing, List<WeakReference<CompHemoCharge>>> HemoChargeCompsByPawn = new();

    public static List<WeakReference<CompHemoCharge>> FindCompFor(Thing instigator, bool forceRefresh = false)
    {
        if (instigator == null) return [];
        List<WeakReference<CompHemoCharge>> comps = HemoChargeCompsByPawn.GetOrCreateValue(instigator);
        if (!comps.Empty() && !forceRefresh) return comps;
        RefreshList(instigator as Pawn, comps);
        return comps;
    }

    public static void Cleanup(Pawn p)
    {
        if (p != null) HemoChargeCompsByPawn.Remove(p);
    }

    public static void Refresh(Pawn pawn)
    {
        if (pawn == null) return;
        FindCompFor(pawn, true);
    }

    public static void RefreshList(Pawn pawn, List<WeakReference<CompHemoCharge>> comps)
    {
        if (pawn == null || comps == null) return;
        comps.Clear();
        foreach (ThingWithComps thingWithComps in pawn.equipment?.AllEquipmentListForReading ?? [])
        {
            if (thingWithComps.TryGetComp<CompHemoCharge>() is { } comp) comps.Add(new WeakReference<CompHemoCharge>(comp));
        }
    }
}
