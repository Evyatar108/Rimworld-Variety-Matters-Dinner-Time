using UnityEngine;
using Verse;

namespace VarietyMattersDT
{
    public class ModSettings_VMDT : ModSettings
    {
        //Meal Time
        public static float assignmentPos = 0;

        //Food Selection
        public static bool preferDiningFood = true;
        public static bool preferSpoiling = true;

        //Food Thoughts
        public static bool foodsWithoutTable = false;
        public static bool useTableThought = false;
        public static bool memorableLavish = false;

        //Cooking Quality
        public static bool cookingQuality = false;

        //Freshly Cooked
        public static bool warmMeals = false;
        public static bool leftoverMeals = false;
        public static bool frozenMeals = false;
        public static float minFreshTemp = 60;
        public static float warmHours = 20;
        public static float leftoverHours = 40;
        public static float refrigMulti = 2;

        //Updates
        public static int freshUpdate = -1;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref assignmentPos, "foodPos", 0);
            Scribe_Values.Look(ref preferDiningFood, "preferDiningFood", true);
            Scribe_Values.Look(ref preferSpoiling, "preferSpoiling", true);
            //Food Thoughts
            Scribe_Values.Look(ref foodsWithoutTable, "foodsWithoutTable", false);
            Scribe_Values.Look(ref useTableThought, "useTableThought", false);
            Scribe_Values.Look(ref memorableLavish, "memorableLavish", false);
            //Scribe_Values.Look(ref longerFine, "longerLavish", false);
            //Cooking Quality
            Scribe_Values.Look(ref cookingQuality, "cookingQuality", false);
            //Freshly Cooked
            Scribe_Values.Look(ref warmMeals, "warmMeals", false);
            Scribe_Values.Look(ref leftoverMeals, "leftoverMeals", false);
            Scribe_Values.Look(ref frozenMeals, "frozenMeals", false);
            Scribe_Values.Look(ref warmHours, "warmHours", 20f);
            Scribe_Values.Look(ref leftoverHours, "leftoverHours", 40f);
            Scribe_Values.Look(ref refrigMulti, "refrigMulti", 2f);
            Scribe_Values.Look(ref minFreshTemp, "minFreshTemp", 60f);

            //Updates
            Scribe_Values.Look(ref freshUpdate, "freshUpdate", -1);
            base.ExposeData();
        }
    }
}
