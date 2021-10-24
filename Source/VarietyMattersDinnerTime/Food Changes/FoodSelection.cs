using RimWorld;
using Verse;
using HarmonyLib;

namespace VarietyMattersDT
{
    [HarmonyPatch]
    public class FoodSelection
    {
        private static Pawn Packer { get; set; }

        [HarmonyPatch(typeof(JobGiver_PackFood), "TryGiveJob")]
        [HarmonyPrefix]
        public static void TrackPawn(Pawn pawn)
        {
            Packer = pawn;
            //Log.Message("Trying to give pack job to " + pawn.Name);
        }

        [HarmonyPatch(typeof(JobGiver_PackFood), "TryGiveJob")]
        [HarmonyPostfix]
        public static void StopTrackingPawn()
        {
            Packer = null;
        }

        [HarmonyPatch(typeof(FoodUtility), "FoodOptimality")]
        public static void Prefix(Pawn eater, Thing foodSource, ref float dist, ref bool takingToInventory)
        {

            if (Packer != null && Packer == eater)
            {
                takingToInventory = true;
            }

            if (ModSettings_VMDT.preferDiningFood && foodSource != null & eater != null && !eater.NonHumanlikeOrWildMan() && !takingToInventory)
            {
                Room room = RegionAndRoomQuery.GetRoom(foodSource);
                Room room2 = RegionAndRoomQuery.GetRoom(eater);
                if (foodSource is Building_NutrientPasteDispenser)
                {
                    //Log.Message("Found nutrient paste dispenser");
                    IntVec3 intVec = foodSource.def.hasInteractionCell ? foodSource.InteractionCell : foodSource.Position;
                    room = intVec.GetRoom(foodSource.Map);
                    //Log.Message("Dispenser is in a " + room.Role.ToString());
                }
                if (room != null && room2 != null)
                {
                    if (FoodUtility.ShouldBeFedBySomeone(eater) && room == room2)
                    {
                        //Log.Message("Needs to be fed.");
                        dist = 0f;
                        return;
                    }
                }
                if (room != null && room.Role != null)
                {
                    if (room.Role == RoomRoleDefOf.DiningRoom)
                    {
                        //Log.Message("Found " + foodSource.ToString() + " in the dining room, it is " + dist.ToString() + " spaces away.");
                        if (dist < 75)
                            dist = 0f;
                        else if (dist < 150)
                            dist -= dist / 2;
                    }
                    else if (room2 != null && room.Role == RoomRoleDefOf.PrisonCell && eater.IsPrisoner && room == room2)
                    {
                        //Log.Message(eater.Name + " gets prison food.");
                        dist = 0f;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(FoodUtility), "FoodOptimality")]
        public static void Postfix(ref float __result, Thing foodSource, ThingDef foodDef, bool takingToInventory)
        {
            if (ModSettings_VMDT.foodsWithoutTable && takingToInventory && foodDef.HasModExtension<DefMod_VMDT>())
            {
                __result += foodDef.GetModExtension<DefMod_VMDT>().packBonus;
            }

            else if (ModSettings_VMDT.preferSpoiling && !takingToInventory && foodSource.AmbientTemperature > 0f)
            {
                CompRottable compRottable = foodSource.TryGetComp<CompRottable>();
                if (compRottable != null)
                    __result += 12 * (1 + compRottable.RotProgressPct);
            }
        }
    }
}
