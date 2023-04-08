
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace SetPointPriorities
{
    public class EditSetPointsWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(600f, 400f);

        public EditSetPointsWindow()
        {
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Gap(10);
            listingStandard.Label("Active Set Points");
            listingStandard.Gap(10);

            // Display active setpoints
            foreach (SetPoint setPoint in SetPointManager.Instance.ActiveSetPoints)
            {
                listingStandard.Label($"Pawn: {setPoint.Pawn.Name}, Work Type: {setPoint.WorkType}, Resource: {setPoint.Resource}, Active Threshold: {setPoint.TriggerThreshold}, Disable Threshold: {setPoint.DisableThreshold}");
                Rect buttonRect = listingStandard.GetRect(30); 

                if (Widgets.ButtonText(new Rect(buttonRect.x + 70, buttonRect.y, 60, buttonRect.height), "Delete"))
                {
                    SetPointManager.Instance.RemoveSetPoint(setPoint);
                }
            }

            listingStandard.End();
        }
    }
}