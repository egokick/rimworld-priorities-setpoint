using System;
using Verse;

public class SetPoint
{
    public int TriggerThreshold { get; set; }
    public int DisableThreshold { get; set; }
    public Pawn Pawn { get; set; } = new Pawn();
    public WorkTypeDef WorkType { get; set; }
    public int ActivePriority { get; set; }
    public int InactivePriority { get; set; }
    public bool Enabled { get; set; }
    public ThingDef Resource { get; set; }

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
}
