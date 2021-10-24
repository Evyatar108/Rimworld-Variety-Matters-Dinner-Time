using Verse;

namespace VarietyMattersDT
{
    class SpecialThingFilterWorker_Leftovers : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            CompFreshness compFreshness = t.TryGetComp<CompFreshness>();
            return compFreshness != null && compFreshness.Freshness == FreshnessCategory.Leftover;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp(typeof(CompFreshness));
        }
    }
}
