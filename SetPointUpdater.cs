using Verse;

namespace SetPointPriorities
{
    public class SetPointUpdater : GameComponent
    {
        private const int UpdateInterval = 1000; // Update every 1000 ticks (30 mins in-game )

        public SetPointUpdater(Game game)
        {
            if (SetPointManager.Instance is null)
            {
                new SetPointManager();
            }

        }

        public override void GameComponentTick()
        {
            if (GenTicks.TicksGame % UpdateInterval == 0)
            {
                Log.Message("Called GameComponentTick and UpdateSetPoints method");
                SetPointManager.Instance.UpdateSetPoints();
            }
        }
    }

}
