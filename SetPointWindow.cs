using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

public class SetPointWindow : Window
{
    private int triggerThreshold;
    private int disableThreshold;
    private Pawn selectedPawn;
    private WorkTypeDef selectedWorkType;
    private int activePriority;
    private int inactivePriority;
    private ThingDef selectedResource;

    private List<ThingDef> resources;
    private List<WorkTypeDef> workTypeDefs;
    private Vector2 scrollPosition = Vector2.zero;

    public override Vector2 InitialSize => new Vector2(600f, 900f);

    public SetPointWindow()
    {
        try
        {

            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;

            resources = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(x => x.CountAsResource)
                .OrderBy(x => x.label)
                .ToList();

            workTypeDefs = DefDatabase<WorkTypeDef>.AllDefsListForReading
                .OrderBy(w => w.label)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private string resourceSearchText = "";

    public override void DoWindowContents(Rect inRect)
    {
        try
        {
            if (SetPointManager.Instance is null)
            {
                new SetPointManager();
            }

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect); 
          
            // Add UI elements for creating a SetPoint
            listingStandard.Label("Trigger Threshold:");
            string triggerThresholdStr = triggerThreshold.ToString();            
            int triggerThresholdValue = triggerThreshold;
            triggerThreshold = (int)listingStandard.Slider(triggerThreshold, 0, 1000);
            listingStandard.TextFieldNumeric(ref triggerThresholdValue, ref triggerThresholdStr, 0, 1000);
            triggerThreshold = int.TryParse(triggerThresholdStr, out int parsedTriggerThreshold) ? parsedTriggerThreshold : triggerThreshold;


            listingStandard.Label("Disable Threshold:");
            string disableThesholdStr = disableThreshold.ToString();
            int disableThresholdValue = disableThreshold;
            disableThreshold = (int)listingStandard.Slider(disableThreshold, 0, 1000);
            listingStandard.TextFieldNumeric(ref disableThresholdValue, ref disableThesholdStr, 0, 1000);
            disableThreshold = int.TryParse(disableThesholdStr, out int parsedDisableThreshold) ? parsedDisableThreshold : disableThreshold;

            listingStandard.Label("Select Pawn:");
            if (Find.CurrentMap != null)
            {
                var pawns = Find.CurrentMap.mapPawns.FreeColonists;
                if (listingStandard.ButtonText(selectedPawn != null ? selectedPawn.Name.ToStringFull : "No pawn selected"))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    foreach (Pawn pawn in pawns)
                    {
                        options.Add(new FloatMenuOption(pawn.Name.ToStringFull, () => { selectedPawn = pawn; }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                }
            }

            listingStandard.Label("Select Work Type:"); 
            if (listingStandard.ButtonText(!string.IsNullOrEmpty(selectedWorkType?.defName) ? selectedWorkType.defName : "No work type selected"))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (WorkTypeDef workTypeDef in workTypeDefs)
                {
                    options.Add(new FloatMenuOption(workTypeDef.defName, () => { selectedWorkType = workTypeDef; }));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            listingStandard.Label("Active Priority:");
            DrawActivePriorityBox(listingStandard.GetRect(30));

            listingStandard.Label("Inactive Priority:");            
            DrawInactivePriorityBox(listingStandard.GetRect(30));

            listingStandard.Label("Select Resource:");
            Rect textFieldRect = listingStandard.GetRect(30);
            resourceSearchText = Widgets.TextField(textFieldRect, resourceSearchText);

            // Filter resources based on search text
            List<ThingDef> visibleResources = resources.Where(resource => string.IsNullOrEmpty(resourceSearchText) || resource.label.IndexOf(resourceSearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            // Calculate the inner scroll view height based on the number of visible resources
            float scrollViewHeight = visibleResources.Count * 30;

            Rect listRect = listingStandard.GetRect(150);
            Widgets.BeginScrollView(listRect, ref scrollPosition, new Rect(0, 0, listRect.width - 16, scrollViewHeight));
            for (int i = 0; i < visibleResources.Count; i++)
            {
                ThingDef resource = visibleResources[i];

                Rect rowRect = new Rect(0, i * 30, listRect.width - 16, 30);
                GUIStyle buttonStyle = new GUIStyle(Text.CurFontStyle);
                if (selectedResource == resource)
                {
                    buttonStyle.normal.textColor = Color.cyan; // Change the color to cyan when the resource is selected
                }

                if (GUI.Button(rowRect, resource.label, buttonStyle))
                {
                    selectedResource = resource;
                }
            }
            Widgets.EndScrollView();

            SetPoint newSetPoint = null;

            if (listingStandard.ButtonText("Create SetPoint"))
            {
                if (selectedPawn != null && selectedWorkType != null && selectedResource != null)
                {
                    newSetPoint = new SetPoint(triggerThreshold, disableThreshold, selectedPawn, selectedWorkType, activePriority, inactivePriority, selectedResource);

                    // Add the new SetPoint to the SetPointManager
                    SetPointManager.Instance.AddSetPoint(newSetPoint);

                    // Close the window
                    this.Close();

                }
                else
                {
                    // Display an error message if any required selection is missing
                    Messages.Message("You must select a pawn, work type, and resource before creating a SetPoint.", MessageTypeDefOf.RejectInput, false);
                }
                this.Close();

                if (newSetPoint is null)
                {
                    Messages.Message("SetPoint was null... nothing happened", MessageTypeDefOf.RejectInput);
                }
                else
                {
                    Messages.Message("SetPoint created: " + newSetPoint.WorkType.labelShort, MessageTypeDefOf.TaskCompletion, false);
                }
            }

            //if (listingStandard.ButtonText("Apply"))
            //{
            //    // Apply the settings and close the window
            //    this.Close();

            //    if (newSetPoint is null)
            //    {
            //        Messages.Message("SetPoint was null... nothing happened", MessageTypeDefOf.RejectInput);
            //    }
            //    else
            //    {
            //        Messages.Message("SetPoint created: " + newSetPoint.WorkType.labelShort, MessageTypeDefOf.TaskCompletion, false);
            //    }
            //}
            listingStandard.End();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, ex.StackTrace);
        }
    }

    private void DrawActivePriorityBox(Rect rect)
    {
        if (Widgets.ButtonText(rect, activePriority.ToString()))
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            for (int i = 0; i <= 4; i++)
            {
                int currentOption = i;
                options.Add(new FloatMenuOption(currentOption.ToString(), () => { activePriority = currentOption; }));
            }
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }

    private void DrawInactivePriorityBox(Rect rect)
    {
        if (Widgets.ButtonText(rect, inactivePriority.ToString()))
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            for (int i = 0; i <= 4; i++)
            {
                int currentOption = i;
                options.Add(new FloatMenuOption(currentOption.ToString(), () => { inactivePriority = currentOption; }));
            }
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }


}