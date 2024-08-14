using Hemogenesis_Weaponry.Comps;
using Verse;

namespace Hemogenesis_Weaponry.DamageWorkers;

public static class DamageWorkerExtensions
{
    public static DamageInfo AdjustDamageInfo(this DamageWorker_AddInjury worker, DamageInfo dinfo, Thing thing) =>
        thing is Pawn pawn && pawn.RaceProps.IsFlesh
            ? HemoChargeUtil.FindCompFor(dinfo.Instigator)
                ?.FirstOrDefault(t => t.Target?.parent.def == dinfo.Weapon)
                ?.Target?.AdjustDamageInfo(dinfo, thing) ?? dinfo
            : dinfo;
}
