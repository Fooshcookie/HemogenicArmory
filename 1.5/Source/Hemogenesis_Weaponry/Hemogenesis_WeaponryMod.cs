using Verse;
using UnityEngine;
using HarmonyLib;

namespace Hemogenesis_Weaponry;

public class Hemogenesis_WeaponryMod : Mod
{
    public Hemogenesis_WeaponryMod(ModContentPack content) : base(content)
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("Fishcookie.rimworld.Hemogenesis_Weaponry.main");	
        harmony.PatchAll();
    }
}
