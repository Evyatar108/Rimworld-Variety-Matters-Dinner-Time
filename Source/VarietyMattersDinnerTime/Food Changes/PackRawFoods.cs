using RimWorld;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
    [HarmonyPatch(typeof(JobGiver_PackFood), "IsGoodPackableFoodFor")]
    public static class PackRawFoods
    {
        private static bool Prefix(ref bool __result, Thing food, Pawn forPawn)
        {
            if (ModSettings_VMDT.foodsWithoutTable)
            {
                __result = food.def.IsNutritionGivingIngestible && food.def.EverHaulable && food.def.ingestible.preferability >= FoodPreferability.RawTasty && forPawn.WillEat(food, null, false);
                return false;
            }
            return true;
        }
    }
}
