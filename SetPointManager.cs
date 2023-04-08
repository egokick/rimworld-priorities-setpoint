using RimWorld;
using SetPointPriorities;
using System;
using System.Collections.Generic;
using Verse;

public class SetPointManager : IExposable
{
    public void ExposeData()
    {
        Scribe_Collections.Look(ref ActiveSetPoints, "ActiveSetPoints", LookMode.Deep);
    }
    public static SetPointManager Instance { get; private set; }

    public List<SetPoint> ActiveSetPoints = new List<SetPoint>();

    public SetPointManager()
    {
        try
        {
            Instance = this;
            ActiveSetPoints = new List<SetPoint>();
        }
        catch (Exception ex)
        {
            Log.Message(ex.Message);
            Console.WriteLine(ex.Message);
        }
    }

    public void AddSetPoint(SetPoint setPoint)
    {
        try 
        {
            Log.Message("AddSetPoint: " + setPoint.WorkType);
            ActiveSetPoints.Add(setPoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void RemoveSetPoint(SetPoint setPoint)
    {
        try
        {
            ActiveSetPoints.Remove(setPoint);
        }
        catch(Exception e) 
        {
            Console.WriteLine(e.Message);
        }
    }

    public void UpdateSetPoints()
    {
        Log.Message("UpdateSetPoints START");
        try
        {
            foreach (SetPoint setPoint in ActiveSetPoints)
            {

                // Get the resource count in the pawn's map
                int resourceCount = setPoint.Pawn.Map.resourceCounter.GetCount(setPoint.Resource);
                Log.Message("UpdateSetPoints.resourceCount: " + resourceCount.ToString());
                Log.Message("UpdateSetPoints.setPoint is null: " + (setPoint is null).ToString());
                Log.Message("UpdateSetPoints.setPoint.Enabled: "  + setPoint.Enabled.ToString());
                Log.Message("UpdateSetPoints.setPoint.TriggerThreshold: " + setPoint.TriggerThreshold.ToString());

                // Check if the setPoint should be activated or deactivated
                if (!setPoint.Enabled && resourceCount <= setPoint.TriggerThreshold)
                {
                    setPoint.Activate();
                }
                else if (setPoint.Enabled && resourceCount >= setPoint.DisableThreshold)
                {
                    setPoint.Deactivate();
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}
