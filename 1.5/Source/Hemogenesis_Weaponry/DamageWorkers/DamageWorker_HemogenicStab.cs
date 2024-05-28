using Hemogenesis_Weaponry.Comps;
using Verse;

namespace Hemogenesis_Weaponry.DamageWorkers;

public class DamageWorker_HemogenicStab: DamageWorker_Stab
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        return base.Apply(thing is Pawn pawn && pawn.RaceProps.IsFlesh
            ? HemoChargeUtil.HemoChargeTools.TryGetValue(dinfo.Tool.id)?.AdjustDamageInfo(dinfo, thing) ?? dinfo
            : dinfo, thing);
    }
}