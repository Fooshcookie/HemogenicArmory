using RimWorld;
using Verse;

namespace Hemogenesis_Weaponry;

[DefOf]
public static class Hemogenesis_WeaponryDefOf
{
    public static ThoughtDef FC_HemoWeapons_OwnBloodDrawn;
    public static JobDef FC_HemoWeapons_HemoDrain;

    static Hemogenesis_WeaponryDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Hemogenesis_WeaponryDefOf));
}
