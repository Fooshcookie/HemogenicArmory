using Verse;

namespace Hemogenesis_Weaponry.DamageWorkers;

public class DamageWorker_HemogenicStab : DamageWorker_Stab
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing) => base.Apply(this.AdjustDamageInfo(dinfo, thing), thing);
}

public class DamageWorker_HemogenicInjury : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing) => base.Apply(this.AdjustDamageInfo(dinfo, thing), thing);
}

public class DamageWorker_HemogenicCut : DamageWorker_Cut
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing) => base.Apply(this.AdjustDamageInfo(dinfo, thing), thing);
}
