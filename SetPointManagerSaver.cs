
namespace SetPointPriorities
{
    using RimWorld.Planet;
    using Verse;

    public class SetPointManagerSaver : WorldComponent
    {
        public SetPointManagerSaver(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (SetPointManager.Instance != null)
            {
                SetPointManager.Instance.ExposeData();
            }
        }
    }
}