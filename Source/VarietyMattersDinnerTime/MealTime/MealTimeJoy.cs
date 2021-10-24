using RimWorld;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
    [HarmonyPatch]
    public class MealTimeJoy
    {
        [HarmonyPatch(typeof(JoyGiver), "GetChance")]
        public static void Postfix(ref float __result, JoyGiver __instance, Pawn pawn)
        {
            if (!pawn.IsPrisoner && !pawn.NonHumanlikeOrWildMan() && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                TimeAssignmentDef timeAssignmentDef = ((pawn.timetable == null) ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment);
                JoyKindDef joyKind = __instance.def.joyKind;
                JobDef joyJob = __instance.def.jobDef;
                if (timeAssignmentDef == DefOf_VMDT.VMDT_Food)
                {
                    if (joyJob == JobDefOf.SocialRelax || joyKind == JoyKindDefOf.Gluttonous)
                    {
                        //Log.Message(joyJob.label.ToString() + " is great dinner time recreation.");
                        __result *= 1.4f;
                    }
                    else if (joyKind.defName == "Television")
                    {
                        __result = 0f;
                    }
                    else if (joyKind.defName != "Chemical" ||  joyJob.driverClass.Name != "JobDriver_PlayPoker" || joyJob.driverClass.Name != "JobDriver_PlayBilliards")
                    {
                        //Log.Message(joyJob.label.ToString() + " sounds fun for later.");
                        __result *= .6f;
                    }
                }
                else if (pawn.timetable.times.Contains(DefOf_VMDT.VMDT_Food))
                {
                    if (joyJob == JobDefOf.SocialRelax || joyKind == JoyKindDefOf.Gluttonous)
                    {
                        //Log.Message("I love relaxing and eating dessert, but I should probably have dinner first.");
                        __result *= 5f;
                    }
                }
            }
            return;
        }
    }
}
