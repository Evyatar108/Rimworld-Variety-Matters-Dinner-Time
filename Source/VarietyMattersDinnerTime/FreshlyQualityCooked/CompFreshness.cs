using RimWorld;
using UnityEngine;
using Verse;

namespace VarietyMattersDT
{
	public class CompFreshness : ThingComp
	{
		//Freshly Cooked
		private bool frozen = false;
		private float leftoverProgress = 0;
		private float leftoverProgressPct;
		private static int freezeTemp = 0;
		private static int refrigTemp = 10;

		//Quality Cooking 
		public float badChance = 0f;
		public bool smallPortion = false;

		public int TicksToLeftover
		{
			get
			{
				return Mathf.RoundToInt(ModSettings_VMDT.leftoverHours * 2500);
			}
		}

		public int WarmTicks
		{
			get
			{
				return Mathf.RoundToInt(ModSettings_VMDT.warmHours * 2500);
			}
		}

		// Freshly Cooked Properties
		public float LeftoverProgPct
		{
			get
			{
				if (leftoverProgressPct >= 1f)
				{
					return 1f;
				}
				return leftoverProgress / TicksToLeftover;
			}
			private set
			{
				leftoverProgressPct = value;
			}
		}

		public FreshnessCategory Freshness
		{
			get
			{
				if (ModSettings_VMDT.frozenMeals && (frozen || parent.AmbientTemperature <= freezeTemp))
				{
					return FreshnessCategory.FrozenLeftover;
				}
				if (ModSettings_VMDT.leftoverMeals && LeftoverProgPct >= 1f)
				{
					return FreshnessCategory.Leftover;
				}
				if (ModSettings_VMDT.warmMeals && leftoverProgress < WarmTicks)
				{
					return FreshnessCategory.Warm;
				}
				return FreshnessCategory.Cool;
			}
		}

		//Methods
		public void Reset()
		{
			leftoverProgressPct = 0f;
		}

		public override void CompTick()
		{
			DoTicks(1);
		}

		public override void CompTickRare()
		{
			DoTicks(250);
		}

		private void DoTicks(int ticks)
		{
			if (!frozen)
			{
				float ambientTemperature = parent.AmbientTemperature;
				//Check for Frozen
				if (ambientTemperature <= freezeTemp)
				{
					LeftoverProgPct = 1f;
					frozen = true;
				}
				//Check for Losing Freshness
				else if ((ModSettings_VMDT.warmMeals || ModSettings_VMDT.leftoverMeals) && LeftoverProgPct < 1f)
				{
					//Refrigerated
					if (ambientTemperature <= refrigTemp)
					{
						leftoverProgress += ModSettings_VMDT.refrigMulti * (float)ticks;
					}
					//Not Refrigerated
					else if (ambientTemperature < ModSettings_VMDT.minFreshTemp)
					{
						leftoverProgress += (float)ticks;
					}
					if (leftoverProgress >= TicksToLeftover)
					{
						LeftoverProgPct = 1f;
					}
				}
			}
		}

		public override void PrePostIngested(Pawn ingester)
        {
			if (ingester.needs.mood != null)
			{
				if (ModSettings_VMDT.cookingQuality && badChance >= 1f)
				{
					//Log.Message(ingester.Name + " ate a poorly cooked meal.");
					CookingQuality.PoorlyCookedEffects(ingester, this.parent);
				}

				switch (Freshness)
				{
					case FreshnessCategory.FrozenLeftover:
						ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("VMDT_FrozenLeftovers"));
						break;
					case FreshnessCategory.Leftover:
						ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("VMDT_Leftovers"));
						break;
					case FreshnessCategory.Warm:
						ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("VMDT_HotMeal"));
						break;
					default:
						base.PrePostIngested(ingester);
						break;
				}
			}
        }

        public override bool AllowStackWith(Thing other)
        {
			//CompFreshness otherComp = ((ThingWithComps)other).GetComp<CompFreshness>();
			CompFreshness otherComp = other.TryGetComp<CompFreshness>();
			if (parent.AmbientTemperature > freezeTemp)
			{
				if (frozen != otherComp.frozen || Freshness != otherComp.Freshness)
				{
					return false;
				}
			}
			return base.AllowStackWith(other);
		}

		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(this.parent.stackCount + count);
			CompFreshness comp = otherStack.TryGetComp<CompFreshness>();
			
			//Freshly Cooked
			leftoverProgress = Mathf.Lerp(this.leftoverProgress, comp.leftoverProgress, t);

			//Quality Cooking
			badChance = Mathf.Lerp(this.badChance, comp.badChance, t);
		}
        public override void PostSplitOff(Thing piece)
		{
			CompFreshness newComp = piece.TryGetComp<CompFreshness>();
			newComp.frozen = frozen;
			newComp.leftoverProgress = leftoverProgress;

			//Quality Cooking
			if (ModSettings_VMDT.cookingQuality && badChance > 0f)
			{
				//Log.Message("Taking " + piece.stackCount);
				float totalMeals = this.parent.stackCount + piece.stackCount; //Log.Message("There are " + totalMeals + " total meals in this stack");
				if (badChance == 1f)
				{
					newComp.badChance = badChance;
					return;
				}

				if (newComp.badChance == 1f)
                {
					return;
                }

				float badMeals = (int)(badChance * totalMeals); //Log.Message("There are " + badMeals + " bad meals in this stack");
				float rand = Rand.Range(0f, 1f); //Log.Message("Random number is " + rand + " and bad chance is " + badChance);
				if (rand < badChance)
                {
					//Log.Message("Took a bad meal");
					badChance = (badMeals - piece.stackCount) / (totalMeals - piece.stackCount);
					newComp.badChance = 1f;
                }
				else
                {
					//Log.Message("Took a good meal");
					badChance = badMeals / (totalMeals - piece.stackCount);
					newComp.badChance = 0f;
                }
			}
		}

		//Display Progress
		public override string CompInspectStringExtra()
        {
			string str1 = base.CompInspectStringExtra();
			switch (Freshness)
			{
				case FreshnessCategory.FrozenLeftover:
					str1 = "Frozen Leftovers";
					break;
				case FreshnessCategory.Leftover:
					str1 = "Leftovers";
					break;
				case FreshnessCategory.Warm:
					if (parent.AmbientTemperature < ModSettings_VMDT.minFreshTemp)
						str1 = "Warm, but getting cold (" + (leftoverProgress / WarmTicks).ToStringPercent() + ")";
					else
						str1 = "Staying warm (" + (leftoverProgress / WarmTicks).ToStringPercent() + ")";
					break;
				default:
					if (ModSettings_VMDT.leftoverMeals)
						str1 = "Turning to leftovers (" + LeftoverProgPct.ToStringPercent() + ")";
					break;
			}
			/* Used for testing
			if (badChance >= 0f)
            {
				str1 = "Poorly cooked (" + (badChance).ToStringPercent() + ")";
            }
			*/
			return str1;
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref badChance, "poorlyCooked", 0f, false);
			Scribe_Values.Look(ref frozen, "frozen", false, false);
			Scribe_Values.Look<float>(ref leftoverProgress, "leftoverProgress", 0f, false);
			Scribe_Values.Look<float>(ref leftoverProgressPct, "leftoverProgressPct", 0f, false);
		}
	}
}
