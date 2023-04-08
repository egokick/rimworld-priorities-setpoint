using RimWorld;
using Verse;

namespace SetPointPriorities
{
    public class SetPointMainButtonWorker : MainButtonWorker
    {
        public override void Activate()
        {
            SetPointDialog dialog = new SetPointDialog();
            Find.WindowStack.Add(dialog);
        }
    } 
}
