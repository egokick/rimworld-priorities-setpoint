using HarmonyLib;
using Verse;


namespace SetPointPriorities
{
    public static class SetPointPrioritiesPatches
    {
        static SetPointPrioritiesPatches()
        {
            var harmony = new Harmony("com.SetPoint.SetPointPriorities");
            harmony.PatchAll();
        }
    }

}
