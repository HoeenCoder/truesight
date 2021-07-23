﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TrueSight
{
    [DefOf]
    public static class TS_DefOf
    {
        public static HediffDef TS_TrueSight;
    }
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("TrueSight.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(JobDriver_Blind), "Blind")]
    public static class JobDriver_Blind_Patch
    {
        public static void Postfix(Pawn pawn, Pawn doer)
        {
            if (!pawn.Dead)
            {
                var hediff = HediffMaker.MakeHediff(TS_DefOf.TS_TrueSight, pawn);
                pawn.health.AddHediff(hediff);
            }
        }
    }
    public class Hediff_TrueSight : HediffWithComps
    {
        public override bool ShouldRemove => pawn.health.hediffSet.GetNotMissingParts().Any((BodyPartRecord x) => x.def == BodyPartDefOf.Eye)
            || pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) || !pawn.Ideo.IdeoApprovesOfBlindness();
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                if (ShouldChangeSeverity(out float newSeverity))
                {
                    this.severityInt = newSeverity;
                }
            }
        }


        private bool ShouldChangeSeverity(out float newSeverity)
        {
            if (!this.pawn.HasPsylink && this.severityInt > 0)
            {
                newSeverity = 0;
                return true;
            }
            var psylinkLevel = pawn.GetPsylinkLevel() / 10f;
            if (this.severityInt != psylinkLevel)
            {
                newSeverity = psylinkLevel;
                return true;
            }
            newSeverity = -1f;
            return false;

        }
    }
}
