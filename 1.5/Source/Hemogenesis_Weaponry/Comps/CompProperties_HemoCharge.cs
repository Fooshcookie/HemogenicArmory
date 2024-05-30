using RimWorld;
using Verse;

namespace Hemogenesis_Weaponry.Comps;

public class CompProperties_HemoCharge : CompProperties
{
    public int maxCharges = 3;
    public int chargesOnKill = 1;
    public HediffDef hediffForBloodCharge;
    public HediffDef hediffForUserOnHit;
    public float severityPerHit = 0.1f;
    public float damageMultiplierForCharge = 1f;
    public bool displayGizmoWhileUndrafted;

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);
        hediffForBloodCharge ??= HediffDefOf.Cut;
        hediffForUserOnHit ??= HediffDefOf.BloodRage;
    }

    public CompProperties_HemoCharge() => compClass = typeof(CompHemoCharge);
}