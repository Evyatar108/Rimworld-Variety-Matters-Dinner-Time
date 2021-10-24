using Verse;

namespace VarietyMattersDT
{
    public class Component_VMDT : GameComponent
    {

        public Component_VMDT(Game game)
        {
        }

        public override void FinalizeInit()
        {
            //ShowTablelessFoods();
            //UpdateDef.DisplayUpdate();
            base.FinalizeInit();
        }

        public static void ShowTablelessFoods()
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (def.HasModExtension<DefMod_VMDT>())
                {
                    Log.Message(def.label.ToString());
                }
            }
        }
    }
}
