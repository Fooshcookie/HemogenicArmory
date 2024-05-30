using System.Collections.Generic;
using Hemogenesis_Weaponry.Comps;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Hemogenesis_Weaponry.Jobs;

public class JobDriver_HemoDrain : JobDriver
{
    public static FleckDef BloodFleck = DefDatabase<FleckDef>.GetNamed("BloodSplashLong");
    protected Pawn Victim => (Pawn)job.targetA.Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve((LocalTargetInfo)(Thing)Victim, job, errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnNotDowned(TargetIndex.A);
        Toil execute = ToilMaker.MakeToil();
        execute.defaultCompleteMode = ToilCompleteMode.Delay;
        execute.defaultDuration = 120;
        execute.handlingFacing = true;
        execute.activeSkill = () => SkillDefOf.Melee;
        execute.FailOnNotDowned(TargetIndex.A);
        execute.WithProgressBarToilDelay(TargetIndex.A, true);
        execute.initAction = () =>
        {
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(Victim.DrawPos, Victim.Map, BloodFleck, 3f);
            Victim.Map.flecks.CreateFleck(dataStatic);
            MoteMaker.ThrowText(Victim.Position.ToVector3(), Victim.MapHeld,
                "FC_HemoWeapons_HemoDrainText".Translate((job.source as Thing)?.def?.label ?? "Blade"), Color.red);
        };
        execute.AddFinishAction(() =>
        {
            int num = Mathf.Max(GenMath.RoundRandom(Victim.BodySize * 12f), 1);
            for (int index = 0; index < num; ++index) Victim.health.DropBloodFilth();
            BodyPartRecord bodyPartRecord = ExecutionUtility.ExecuteCutPart(Victim);
            int partHealth = (int)Victim.health.hediffSet.GetPartHealth(bodyPartRecord);
            int amount = Mathf.Min(partHealth - 1, 1);
            if (ModsConfig.BiotechActive && Victim.genes != null && Victim.genes.HasActiveGene(GeneDefOf.Deathless))
                amount = partHealth;
            DamageInfo dinfo = new(DamageDefOf.ExecutionCut, amount, 999f, instigator: pawn, hitPart: bodyPartRecord, instigatorGuilty: true, spawnFilth: true);
            Victim.TakeDamage(dinfo);
            if (!Victim.Dead)
                Victim.Kill(dinfo);
            SoundDefOf.Execute_Cut.PlayOneShot((SoundInfo)(Thing)Victim);
            // ThoughtUtility.GiveThoughtsForPawnExecuted(this.Victim, execute.actor, PawnExecutionKind.GenericBrutal);
            // TaleRecorder.RecordTale(TaleDefOf.ExecutedPrisoner, (object) this.pawn, (object) this.Victim);
            (job.source as ThingWithComps)?.GetComp<CompHemoCharge>()?.RechargeFully();
        });
        yield return execute;
    }
}
