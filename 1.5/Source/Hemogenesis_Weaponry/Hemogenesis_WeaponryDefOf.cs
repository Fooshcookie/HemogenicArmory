using RimWorld;
using Verse;

namespace Hemogenesis_Weaponry;

[DefOf]
public static class Hemogenesis_WeaponryDefOf
{
    // Remember to annotate any Defs that require a DLC as needed e.g.
    // [MayRequireBiotech]
    // public static GeneDef YourPrefix_YourGeneDefName;
    
    static Hemogenesis_WeaponryDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Hemogenesis_WeaponryDefOf));
}
