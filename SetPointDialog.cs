using System;
using UnityEngine;
using Verse;


namespace SetPointPriorities
{
    public class SetPointDialog : Window
    {
        // Set default window properties
        public override Vector2 InitialSize => new Vector2(300f, 200f);

        public SetPointDialog()
        {
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            try
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(inRect);

                // Add a button to open the SetPointWindow
                if (listingStandard.ButtonText("Open SetPoint Manager"))
                {
                    SetPointWindow window = new SetPointWindow();
                    Find.WindowStack.Add(window);
                }

                listingStandard.End();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}
