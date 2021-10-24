using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
	[HarmonyPatch]
	public class CookingQuality
    {
		public static List<ThoughtDef> poorlyCookedThoughts = new List<ThoughtDef>()
		{
			ThoughtDef.Named("VMDT_Burnt"),
			ThoughtDef.Named("VMDT_Overcooked"),
			ThoughtDef.Named("VMDT_Overseasoned"),
			ThoughtDef.Named("VMDT_SmallPortions"),
			ThoughtDef.Named("VMDT_Undercooked"),
			ThoughtDef.Named("VMDT_Unforgettable"),
			ThoughtDef.Named("VMDT_Underseasoned"),
			ThoughtDef.Named("VMDT_LargePortions")
		};
		
		[HarmonyPatch(typeof(GenRecipe), "PostProcessProduct")]
		[HarmonyPostfix]
		public static void Postfix(Thing __result, RecipeDef recipeDef, Pawn worker)
		{
			if (!ModSettings_VMDT.cookingQuality)
            {
				return;
            }
			CompFreshness compFreshness = __result.TryGetComp<CompFreshness>();
			if (compFreshness != null && __result.def.ingestible != null)
            {
				FoodPreferability preferability = __result.def.ingestible.preferability;
				int cookSkill = worker.skills.GetSkill(SkillDefOf.Cooking).Level;
				float fullSkill = 0;
				switch (preferability)
                {
					case FoodPreferability.MealLavish:
						fullSkill = (int)preferability * 2f;
						break;
					case FoodPreferability.MealFine:
						fullSkill = (int)preferability * 1.5f;
						break;
					case FoodPreferability.MealSimple:
					case FoodPreferability.MealAwful:
						fullSkill = (int)preferability;
						break;
                }
				float badChance = Math.Max(0, (fullSkill - cookSkill) * (int)preferability / 200);
				//Log.Message("Chance for poorly cooked is " + badChance);
				float num = Rand.Range(0f, 1f);
				//Log.Message("Random number is " + num);
				if (num < badChance)
				{
					//Log.Message(worker.Name + " cooked a meal poorly.");
					//compFreshness.badChance = recipeDef.products[0].count;
					compFreshness.badChance = 1f;
				}

            }
		}
		
		public static void PoorlyCookedEffects(Pawn ingester, Thing thing)
		{
			int rand = Rand.Range(0, poorlyCookedThoughts.Count - 1);
			ThoughtDef thought = poorlyCookedThoughts[rand];
			if (thought == ThoughtDef.Named("VMDT_SmallPortions"))
			{
				thing.TryGetComp<CompFreshness>().smallPortion = true;
            }
			if (thought == ThoughtDef.Named("VMDT_LargePortions"))
            {
				float severity = ingester.needs.food.CurLevelPercentage;
				Hediff firstHediffOfDef = ingester.health.hediffSet.GetFirstHediffOfDef(DefOf_VMDT.VMDT_Overate, false);
				if (firstHediffOfDef == null)
				{
					ingester.health.AddHediff(HediffMaker.MakeHediff(DefOf_VMDT.VMDT_Overate, ingester, null), null, null, null);
					firstHediffOfDef = ingester.health.hediffSet.GetFirstHediffOfDef(DefOf_VMDT.VMDT_Overate, false);
				}
				firstHediffOfDef.Severity += severity;
			}
			ingester.needs.mood.thoughts.memories.TryGainMemory(thought);
			//Log.Message(ingester.Name + " gained thought " + thought.Label);
		}

		[HarmonyPatch(typeof(Thing),"Ingested")]
		[HarmonyPostfix]
		public static void SmallPortions(ref float __result, Thing __instance)
        {
			CompFreshness comp = __instance.TryGetComp<CompFreshness>();
			if (comp != null && comp.smallPortion)
            {
				__result -= .3f;
				//Log.Message("Small portion has " + __result + " nutrition");
			}
        }
	}
}
