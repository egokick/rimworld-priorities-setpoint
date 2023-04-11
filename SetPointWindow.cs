using Newtonsoft.Json;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;
using Verse;
using Random = System.Random;

public class SetPointWindow : Window
{ 
    private int activeThreshold;
    private int inactiveThreshold;
    private Pawn selectedPawn;
    private WorkTypeDef selectedWorkType;
    private int activePriority;
    private int inactivePriority;
    private ThingDef selectedResource;

    private List<ThingDef> resources;
    private List<WorkTypeDef> workTypeDefs;
    private Vector2 scrollPosition = Vector2.zero;


    private SetPoint newSetPoint = null;


    public override Vector2 InitialSize => new Vector2(600f, 700f);

    public SetPointWindow(SetPoint setPoint)
    {
        if (setPoint != null)
        { 
            activeThreshold = setPoint.ActiveThreshold;
            inactiveThreshold = setPoint.InactiveThreshold;
            selectedPawn = setPoint.Pawn;
            selectedWorkType = setPoint.WorkType;
            activePriority = setPoint.ActivePriority;
            inactivePriority = setPoint.InactivePriority;
            selectedResource = setPoint.Resource;
            newSetPoint = setPoint;
        }
        else
        {
            newSetPoint = new SetPoint();
        }

        Setup();
    }

    public SetPointWindow()
    {
        newSetPoint = new SetPoint();
        Setup();
    }

    private void Setup()
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

            listingStandard.Label("Select Resource:");
            Rect textFieldResource = listingStandard.GetRect(30);
            resourceSearchText = Widgets.TextField(textFieldResource, resourceSearchText);

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

            activeThreshold = SliderWithTextField("Active When Resource Drops Below", activeThreshold, 0, 2000, 600, 60, listingStandard);
            inactiveThreshold = SliderWithTextField("Inactive When Resource Rises Above", inactiveThreshold, 0, 2000, 600, 60, listingStandard);

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
                    if (selectedPawn != null && selectedPawn.workSettings.WorkIsActive(workTypeDef))
                    {
                        options.Add(new FloatMenuOption(workTypeDef.defName, () => { selectedWorkType = workTypeDef; }));
                    }
                    else if (selectedPawn is null)
                    {
                        options.Add(new FloatMenuOption(workTypeDef.defName, () => { selectedWorkType = workTypeDef; }));
                    }
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            listingStandard.Label("Active Priority:");
            DrawActivePriorityBox(listingStandard.GetRect(30));

            listingStandard.Label("Inactive Priority:");            
            DrawInactivePriorityBox(listingStandard.GetRect(30));


            if (listingStandard.ButtonText("Save Set Point"))
            {
                Log.Message("Save Set Point button clicked");

                if (selectedPawn != null && selectedWorkType != null && selectedResource != null)
                {
                    Log.Message("selectedPawn != null && selectedWorkType != null && selectedResource != null");
                                         
                    try
                    {
                        if (newSetPoint is null)
                        {
                            Log.Message("newSetPoint is null");

                            newSetPoint = new SetPoint();
                        } 
                         
                        newSetPoint.ActiveThreshold = activeThreshold;
                        Log.Message("newSetPoint ActiveThreshold: " + newSetPoint.ActiveThreshold);

                        newSetPoint.InactiveThreshold = inactiveThreshold;
                        Log.Message("newSetPoint InactiveThreshold: " + newSetPoint.InactiveThreshold);

                        newSetPoint.Pawn = selectedPawn;
                        Log.Message("newSetPoint Pawn: " + newSetPoint.Pawn);

                        newSetPoint.WorkType = selectedWorkType;
                        Log.Message("newSetPoint WorkType: " + newSetPoint.WorkType);

                        newSetPoint.ActivePriority = activePriority;
                        Log.Message("newSetPoint ActivePriority: " + newSetPoint.ActivePriority);

                        newSetPoint.InactivePriority = inactivePriority;
                        Log.Message("newSetPoint InactivePriority: " + newSetPoint.InactivePriority);

                        newSetPoint.Resource = selectedResource;
                        Log.Message("newSetPoint Resource: " + newSetPoint.Resource);

                        // Add the new SetPoint to the SetPointManager
                        Log.Message("about to add setpoint");
                        SetPointManager.Instance.AddSetPoint(newSetPoint);

                        // Close the window
                        this.Close();

                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    } 
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
                    Messages.Message($"SetPoint Saved: {newSetPoint.Pawn.Name} {newSetPoint.WorkType.labelShort} {newSetPoint.Resource.defName}", MessageTypeDefOf.TaskCompletion, false);
                }
            }
             
            listingStandard.End();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, ex.StackTrace);
        }
    }


    private int SliderWithTextField(string label, int currentValue, int minValue, int maxValue, float sliderWidth, float textFieldWidth, Listing_Standard listingStandard)
    {
        listingStandard.Label(label);

        bool sliderChanged = false;
        int newSliderValue;
        string textFieldValue;

        Rect sliderRect = listingStandard.GetRect(24f);
        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
        {
            newSliderValue = (int)Mathf.Round(GUI.HorizontalSlider(sliderRect, currentValue, minValue, maxValue));
            sliderChanged = true;
        }
        else
        {
            newSliderValue = currentValue;
            GUI.HorizontalSlider(sliderRect, currentValue, minValue, maxValue);
            sliderChanged = false;
        }

        listingStandard.Gap(listingStandard.verticalSpacing);

        Rect textFieldRect = listingStandard.GetRect(24f);
        textFieldValue = GUI.TextField(textFieldRect, currentValue.ToString());

        if (int.TryParse(textFieldValue, out int parsedValue))
        {
            if (parsedValue != currentValue)
            {
                currentValue = Mathf.Clamp(parsedValue, minValue, maxValue);
            }
        }
        else
        {
            textFieldValue = currentValue.ToString();
        }

        if (sliderChanged && newSliderValue != currentValue)
        {
            currentValue = newSliderValue;
        }

        listingStandard.Gap(listingStandard.verticalSpacing);

        return currentValue;
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