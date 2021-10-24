using Verse;

namespace VarietyMattersDT
{
    public class UpdateDef : Def
    {
        public static void DisplayUpdate()
        {
            if (ModSettings_VMDT.freshUpdate == 0)
            {
                DiaNode diaNode = new DiaNode(DefDatabase<UpdateDef>.GetNamedSilentFail("VMDT_FreshlyCookedUpdate").description);
                diaNode.options.Add(DiaOption.DefaultOK);
                Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, false, "Variety Matters Dinner Time - New (Optional) Features"));
                ModSettings_VMDT.freshUpdate++;
            }
        }
    }
}
