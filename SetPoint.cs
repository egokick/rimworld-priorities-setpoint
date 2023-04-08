using Mono.Cecil;
using System;
using Verse;

public class SetPoint : IExposable
{

    public int TriggerThreshold;
    public int DisableThreshold;
    public Pawn Pawn;
    public WorkTypeDef WorkType;
    public int ActivePriority;
    public int InactivePriority;
    public bool Enabled;
    public ThingDef Resource;

    public SetPoint() { }

    public SetPoint(int triggerThreshold, int disableThreshold, Pawn pawn, WorkTypeDef workType, int activePriority, int inactivePriority, ThingDef resource)
    {
        TriggerThreshold = triggerThreshold;
        DisableThreshold = disableThreshold;
        Pawn = pawn;
        WorkType = workType;
        ActivePriority = activePriority;
        InactivePriority = inactivePriority;
        Enabled = false;
        Resource = resource;
    }

    public void Activate()
    {
        if (!Enabled)
        {
            UpdatePawnPriority(ActivePriority);
            Enabled = true;
        }
    }

    public void Deactivate()
    {
        if (Enabled)
        {
            UpdatePawnPriority(InactivePriority);
            Enabled = false;
        }
    }

    private void UpdatePawnPriority(int newPriority)
    {
        try
        {
            if (Pawn != null && Pawn.workSettings != null)
            {
                Pawn.workSettings.SetPriority(WorkType, newPriority);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref TriggerThreshold, "triggerThreshold");
        Scribe_Values.Look(ref DisableThreshold, "disableThreshold");
        Scribe_References.Look(ref Pawn, "pawn");
        Scribe_Defs.Look(ref WorkType, "workType");
        Scribe_Values.Look(ref ActivePriority, "activePriority");
        Scribe_Values.Look(ref InactivePriority, "inactivePriority");
        Scribe_Values.Look(ref Enabled, "enabled");
        Scribe_Defs.Look(ref Resource, "resource");
    }

}
