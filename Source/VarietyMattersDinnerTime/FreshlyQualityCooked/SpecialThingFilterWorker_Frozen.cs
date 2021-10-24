using Verse;

namespace VarietyMattersDT
{
    public class SpecialThingFilterWorker_Frozen : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            CompFreshness compFreshness = t.TryGetComp<CompFreshness>();
            return compFreshness != null && compFreshness.Freshness == FreshnessCategory.FrozenLeftover;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp(typeof(CompFreshness));
        }
    }
}
