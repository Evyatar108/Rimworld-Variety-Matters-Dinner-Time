using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;

namespace VarietyMattersDT
{
    public class Mod_VMDT : Mod
    {
        Listing_Standard listingStandard = new Listing_Standard();

        public Mod_VMDT(ModContentPack content) : base(content)
        {
            GetSettings<ModSettings_VMDT>();
            if (ModSettings_VMDT.freshUpdate < 1) ModSettings_VMDT.freshUpdate++;
            WriteSettings();
            Harmony harmony = new Harmony("rimworld.varietymattersDT");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory()
        {
            return "VarietyMattersDinnerTime";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(100f, 50f, inRect.width * .8f, inRect.height);

            listingStandard.Begin(rect);
            //Meal Time
            listingStandard.Label("Meal Time:");
            string foodPosBuffer = ModSettings_VMDT.assignmentPos.ToString();
            string foodPosLabel = "Move food schedule assignment selector x spaces to the right:";
            LabeledFloatEntry(this.listingStandard.GetRect(24f), foodPosLabel, ref ModSettings_VMDT.assignmentPos, ref foodPosBuffer, 1f, 1f, 0f, 4f);
            listingStandard.GapLine();
            //Food Selection
            listingStandard.Label("Food Selection:");
            listingStandard.CheckboxLabeled("Prefer food in dining room, hospital, or prison: ", ref ModSettings_VMDT.preferDiningFood);
            listingStandard.CheckboxLabeled("Prefer food close to spoiling: ", ref ModSettings_VMDT.preferSpoiling);
            listingStandard.GapLine();
            //Food Thoughts
            listingStandard.Label("Tables Are For Meals:");
            listingStandard.CheckboxLabeled("Tables are optional for pemmican, fruit and certain non-meals: ", ref ModSettings_VMDT.foodsWithoutTable);
            listingStandard.CheckboxLabeled("Ate without table thought lasts longer and stacks: ", ref ModSettings_VMDT.useTableThought);
            //listingStandard.CheckboxLabeled("Fine meal thought lasts longer and stacks", ref ModSettings_VMDT.longerFine);
            listingStandard.GapLine();
            //Quality Cooking
            listingStandard.Label("Quality  Cooking:");
            listingStandard.CheckboxLabeled("Unskilled chefs may cook meals poorly", ref ModSettings_VMDT.cookingQuality);
            listingStandard.CheckboxLabeled("Lavish meals are memorable:", ref ModSettings_VMDT.memorableLavish);
            listingStandard.GapLine();
            //FreshlyCooked
            listingStandard.Label("Freshly Cooked:");
            listingStandard.CheckboxLabeled("Hot meals taste better", ref ModSettings_VMDT.warmMeals);
            listingStandard.CheckboxLabeled("Leftovers taste worse", ref ModSettings_VMDT.leftoverMeals);
            listingStandard.CheckboxLabeled("Frozen leftovers taste the worst", ref ModSettings_VMDT.frozenMeals);
            if (ModSettings_VMDT.warmMeals)
            {
                string warmLabel = "Hours to stay warm if not refrigerated (default = 20):";
                string warmBuffer = ModSettings_VMDT.warmHours.ToString();
                LabeledFloatEntry(listingStandard.GetRect(24f), warmLabel, ref ModSettings_VMDT.warmHours, ref warmBuffer, 1f, 5f, 1f, 72f);
            }
            if (ModSettings_VMDT.leftoverMeals)
            {
                string leftoverLabel = "Hours to become leftovers if not refrigerated (default = 40):";
                string leftoverBuffer = ModSettings_VMDT.leftoverHours.ToString();
                LabeledFloatEntry(listingStandard.GetRect(24f), leftoverLabel, ref ModSettings_VMDT.leftoverHours, ref leftoverBuffer, 1f, 10f, ModSettings_VMDT.warmHours, 72f);
            }
            if (ModSettings_VMDT.warmMeals || ModSettings_VMDT.leftoverMeals)
            {
                string refrigLabel = "Refrigerated Meal Multiplier (1x = no multiplier; default = 2x):";
                string refrigBuffer = ModSettings_VMDT.refrigMulti.ToString();
                LabeledFloatEntry(this.listingStandard.GetRect(24f), refrigLabel, ref ModSettings_VMDT.refrigMulti, ref refrigBuffer, .1f, 1f, 1f, 10f);
                string freshTempLabel = "Minimum stay-warm temperatue (default = 60)";
                string freshTempBuffer = ModSettings_VMDT.minFreshTemp.ToString();
                LabeledFloatEntry(this.listingStandard.GetRect(24f), freshTempLabel, ref ModSettings_VMDT.minFreshTemp, ref freshTempBuffer, 1f, 10f, 1f, 100f);
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public void LabeledFloatEntry(Rect rect, string label, ref float value, ref string editBuffer, float multiplier, float largeMultiplier, float min, float max)
        {
            int num = (int)rect.width / 15;
            Widgets.Label(rect, label);
            if (multiplier != largeMultiplier)
            {
                if (Widgets.ButtonText(new Rect(rect.xMax - (float)num * 5, rect.yMin, (float)num, rect.height), (-1 * largeMultiplier).ToString(), true, true, true))
                {
                    value -= largeMultiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                if (Widgets.ButtonText(new Rect(rect.xMax - (float)num, rect.yMin, (float)num, rect.height), "+" + largeMultiplier.ToString(), true, true, true))
                {
                    value += largeMultiplier * multiplier * GenUI.CurrentAdjustmentMultiplier();
                    editBuffer = value.ToString();
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - (float)num * 4, rect.yMin, (float)num, rect.height), (-1 * multiplier).ToString(), true, true, true))
            {
                value -= GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - (float)(num * 2), rect.yMin, (float)num, rect.height), "+" + multiplier.ToString(), true, true, true))
            {
                value += multiplier * GenUI.CurrentAdjustmentMultiplier();
                editBuffer = value.ToString();
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
            }
            Widgets.TextFieldNumeric<float>(new Rect(rect.xMax - (float)(num * 3), rect.yMin, (float)num, rect.height), ref value, ref editBuffer, min, max);
        }
        
        /*

        */
    }
}
