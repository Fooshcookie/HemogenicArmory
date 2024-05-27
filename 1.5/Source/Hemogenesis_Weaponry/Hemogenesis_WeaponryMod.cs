using Verse;
using UnityEngine;
using HarmonyLib;

namespace Hemogenesis_Weaponry;

public class Hemogenesis_WeaponryMod : Mod
{
    public static Settings settings;

    public Hemogenesis_WeaponryMod(ModContentPack content) : base(content)
    {

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("Fishcookie.rimworld.Hemogenesis_Weaponry.main");	
        harmony.PatchAll();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Hemogenesis Weaponry_SettingsCategory".Translate();
    }
}
