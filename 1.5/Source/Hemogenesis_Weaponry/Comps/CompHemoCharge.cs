using System.Collections.Concurrent;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Utility;
using UnityEngine;
using Verse;

namespace Hemogenesis_Weaponry.Comps;

public class HemoChargeUtil
{
    public static ConcurrentDictionary<string, CompHemoCharge> HemoChargeTools = new();
}

public class CompHemoCharge : ThingComp, ICompWithCharges
{
    public int remainingCharges = 0;
    public bool useCharges = false;
    public bool allowBloodDraw = false;

    public CompProperties_HemoCharge HemoProps => (CompProperties_HemoCharge) props;


    public int RemainingCharges => remainingCharges;

    public int MaxCharges => HemoProps.maxCharges;

    public string LabelRemaining => $"{(object)RemainingCharges} / {(object)MaxCharges}";

    public DamageInfo AdjustDamageInfo(DamageInfo damageInfo, Thing damagedThing)
    {
        if (!useCharges || HemoProps.damageMultiplierForCharge <= 1 || Holder == null || (RemainingCharges <= 0 && !AttemptRecharge())) return damageInfo;
        remainingCharges--;
        damageInfo.SetAmount(damageInfo.Amount * HemoProps.damageMultiplierForCharge);
        if (HemoProps.hediffForUserOnHit != null)
        {
            Holder.health.hediffSet.TryGetHediff(HemoProps.hediffForUserOnHit, out Hediff userHediff);
            if (userHediff == null)
            {
                Holder.health.AddHediff(HemoProps.hediffForUserOnHit);
            }
            else
            {
                userHediff.Severity += HemoProps.severityPerHit;
            }
        }
        return damageInfo;
    }

    public bool AttemptRecharge()
    {
        //TODO: Draw from blood bag or wounding
        if (allowBloodDraw)
        {
            while (RemainingCharges < MaxCharges)
            {
                // Apply a cut hediff to the pawn and add a charge
                Hediff hediff = HediffMaker.MakeHediff(HemoProps.hediffForBloodCharge, Holder);
                if (!Holder.health.WouldDieAfterAddingHediff(hediff))
                {
                    Holder.health.AddHediff(hediff);
                    remainingCharges++;
                }
                else
                {
                    return RemainingCharges > 0;
                }
            }
        }

        return RemainingCharges > 0;
    }

    public override void Notify_UsedVerb(Pawn pawn, Verb verb)
    {
        base.Notify_UsedVerb(pawn, verb);
        HemoChargeUtil.HemoChargeTools.AddOrUpdate(verb.tool.id, this, (_, _) => this);
    }

    protected Pawn Holder => (parent?.ParentHolder as Pawn_EquipmentTracker)?.pawn;

    public override void Notify_KilledPawn(Pawn pawn)
    {
        base.Notify_KilledPawn(pawn);
        if (pawn.RaceProps.IsFlesh)
        {
            remainingCharges += HemoProps.chargesOnKill;
            if (remainingCharges > MaxCharges)
                remainingCharges = MaxCharges;
        }
    }

    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
            yield return gizmo;

        if (!HemoProps.displayGizmoWhileUndrafted && !(Holder?.Drafted ?? false)) yield break;
        yield return new Gizmo_BloodCharges(this);
        yield return new Command_Toggle()
        {
            defaultLabel = "FC_HemoWeapons_UseCharges".Translate(),
            toggleAction = () => { useCharges = !useCharges; },
            isActive = () => useCharges,
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/UseCharges")
        };
        yield return new Command_Toggle()
        {
            defaultLabel = "FC_HemoWeapons_UseBlood".Translate(),
            toggleAction = () => { allowBloodDraw = !allowBloodDraw; },
            isActive = () => allowBloodDraw,
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/AllowBloodDraw")
        };
        if (DebugSettings.ShowDevGizmos && RemainingCharges < MaxCharges)
        {
            Command_Action commandAction = new Command_Action
            {
                defaultLabel = "DEV: Reload to full",
                action = () => remainingCharges = MaxCharges
            };
            yield return commandAction;
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref useCharges, "useCharges");
        Scribe_Values.Look(ref allowBloodDraw, "allowBloodDraw");
    }

    public bool CanBeUsed(out string reason)
    {
        reason = "";
        if (RemainingCharges > 0 || AttemptRecharge())
            return true;
        reason = "FC_HemoWeapons_NoCharges".Translate();
        return false;
    }
}

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

public class Gizmo_BloodCharges : Gizmo
{
    private static readonly float Width = 110f;
    public CompHemoCharge hemoComp;

    public Gizmo_BloodCharges(CompHemoCharge comp)
    {
        hemoComp = comp;
    }

    public override float GetWidth(float maxWidth) => Width;

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        Rect rect1 = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Rect rect2 = rect1.ContractedBy(6f);
        Widgets.DrawWindowBackground(rect1);
        Rect rect3 = rect2 with { height = rect1.height / 2f };
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(rect3, "FC_HemoWeapons_BloodCharges".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        Rect rect4 = rect1;
        rect4.y += rect3.height - 5f;
        rect4.height = rect1.height / 2f;
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect4, $"{hemoComp.RemainingCharges} / {hemoComp.MaxCharges}");
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
        return new GizmoResult(GizmoState.Clear);
    }
}
