using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace SetPointPriorities
{
    public class EditSetPointsWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(1200f, 400f);

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
            int setPointCount = SetPointManager.Instance.ActiveSetPoints.Count;
            for (int i = 0; i < setPointCount; i++)
            {
                SetPoint setPoint = SetPointManager.Instance.ActiveSetPoints[i];
                listingStandard.Label($"Pawn: {setPoint.Pawn.Name}, Work Type: {setPoint.WorkType}, Resource: {setPoint.Resource}, Active Threshold: {setPoint.ActiveThreshold}, Disable Threshold: {setPoint.InactiveThreshold}, Priority: {setPoint.ActivePriority}, {setPoint.InactivePriority}");
                Rect buttonRect = listingStandard.GetRect(30);

                if (Widgets.ButtonText(new Rect(buttonRect.x + 70, buttonRect.y, 60, buttonRect.height), "Delete"))
                {
                    SetPointManager.Instance.RemoveSetPoint(setPoint);
                }

                // Add Edit button
                if (Widgets.ButtonText(new Rect(buttonRect.x + 140, buttonRect.y, 60, buttonRect.height), "Edit"))
                {
                    Find.WindowStack.Add(new SetPointWindow(setPoint));
                }

                setPointCount = SetPointManager.Instance.ActiveSetPoints.Count;
            }

            listingStandard.End();
        }
    }
}
