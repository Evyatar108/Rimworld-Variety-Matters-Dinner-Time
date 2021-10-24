using System;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace VarietyMattersDT
{
    [HarmonyPatch]
    public class VanillaFoodThoughts
    {
        [HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[] { typeof(ThoughtDef), typeof(Pawn), typeof(Precept) })]
        public static bool Prefix(ref ThoughtDef def, Pawn ___pawn)
        {
            if (def == ThoughtDefOf.AteWithoutTable)
            {
                if (ModSettings_VMDT.foodsWithoutTable)
                {
                    ThingDef foodDef = null;
                    if (___pawn.CurJob.GetTarget(TargetIndex.A) != null)
                    {
                        foodDef = ___pawn.CurJob.GetTarget(TargetIndex.A).Thing.def;
                    }
                    if (foodDef != null && foodDef.HasModExtension<DefMod_VMDT>())
                    {
                        return false;
                    }
                }
                if (ModSettings_VMDT.useTableThought)
                {
                    def = DefOf_VMDT.VMDT_AteWithoutTable;
                }
            }
            if (def == ThoughtDefOf.AteLavishMeal && ModSettings_VMDT.memorableLavish)
            {
                def = DefOf_VMDT.VMDT_AteLavishMeal;
            }
            return true;
        }
    }
}
