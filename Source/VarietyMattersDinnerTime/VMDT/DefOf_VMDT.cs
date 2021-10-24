using RimWorld;
using Verse;

namespace VarietyMattersDT
{
    [DefOf]
    public static class DefOf_VMDT
    {
        public static TimeAssignmentDef VMDT_Food;
        public static ThoughtDef VMDT_AteWithoutTable;
        public static ThoughtDef VMDT_AteLavishMeal;
        public static HediffDef VMDT_Overate;

        static DefOf_VMDT()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefOf_VMDT));
        }
    }
}
