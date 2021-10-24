using RimWorld;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
    [HarmonyPatch]
    public class MealTimePriorities
    {
        [HarmonyPatch(typeof(JobGiver_GetFood), "GetPriority")]
        [HarmonyPostfix]
        public static void Postfix_GetFoodPriority(ref float __result, Pawn pawn)
        {
            //Log.Message("Starting check");
            Need_Food food = pawn.needs.food;
            if (food == null || pawn.Faction == null || !pawn.Faction.IsPlayer || pawn.NonHumanlikeOrWildMan())
            {
                //Log.Message("Only pawns that need food, are tame, and aren't in prison schedule their own dinner!");
                return;
            }
            //Log.Message("Check for patients");
            if (food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                //Log.Message(pawn.Name.ToString() + " is waiting to be fed.");
                return;
            }
            //Log.Message("Check for dinner time");
            TimeAssignmentDef timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
            if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
            {
                //Log.Message(pawn.Name + " current food level is " + food.CurLevelPercentage.ToString());
                //Log.Message(pawn.Name + " likes to eat when food level is " + (pawn.RaceProps.FoodLevelPercentageWantEat * 1.4).ToString());
                if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat * 1.4)
                {
                    //Log.Message(pawn.Name.ToString() + " is ready to eat!");
                    __result = 9.5f;
                }
            }
            else if (__result != 0f && food.CurCategory < HungerCategory.UrgentlyHungry) //
            {
                //Log.Message(pawn.Name.ToString() + " wants to eat but can wait until dinner if its close.");
                int hour = GenLocalDate.HourInteger(pawn);
                //Log.Message("hour is " + hour);
                for (int i = 0; i < 3; i++)
                {
                    hour++;
                    if (hour == 24)
                    {
                        hour = 0;
                    }
                    timeAssignmentDef = ((pawn.timetable.GetAssignment(hour) == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.GetAssignment(hour));
                    if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
                    {
                        //Log.Message(pawn.Name + " is going to wait for dinner.");
                        __result = 0f;
                        break;
                    }
                }
            }
            return;
        }

        [HarmonyPatch(typeof(ThinkNode_Priority_GetJoy), "GetPriority")]
        [HarmonyPrefix]
        public static bool Prefix_GetJoy(ref float __result, Pawn pawn)
        {
            Need_Joy joy = pawn.needs.joy;
            if (joy != null && !JoyUtility.LordPreventsGettingJoy(pawn) 
                && !pawn.IsPrisoner 
                && pawn.Faction != null 
                && pawn.Faction.IsPlayer 
                && !pawn.NonHumanlikeOrWildMan())
            {
                TimeAssignmentDef timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
                if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
                {
                    __result = 0f;
                    if (joy.CurLevel < 0.95f && (pawn.needs.food == null || pawn.needs.food.CurLevel > pawn.RaceProps.FoodLevelPercentageWantEat * 1.4))
                    {
                        __result = 7f;
                    }
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(JobGiver_GetRest), "GetPriority")]
        [HarmonyPrefix]
        public static bool Prefix_GetRest(ref float __result, Pawn pawn)
        {
			Need_Rest rest = pawn.needs.rest;
			if (rest != null && !pawn.IsPrisoner && pawn.Faction != null && pawn.Faction.IsPlayer && !pawn.NonHumanlikeOrWildMan())
            { 
			    TimeAssignmentDef timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
                if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
                {
                    __result = 0f;
                    if (rest.CurLevel < 0.3f && pawn.needs.food.CurLevel > pawn.RaceProps.FoodLevelPercentageWantEat * 1.4)
                    {
                        __result = 8f;
                    }
                    return false;
                }
            }
			return true;
        }
        [HarmonyPatch(typeof(JobGiver_Work), "GetPriority")]
        [HarmonyPrefix]
        public static bool Prefix_Work(ref float __result, Pawn pawn)
        {
            if (pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                TimeAssignmentDef timeAssignmentDef = (pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
                if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
                {
                    __result = 2f;
                    return false;
                }
            }
            return true;
        }
    }
}
