using Autodesk.Revit.DB;
using System;

namespace Task4.Utilities
{
    public static class SectionBoxUtils
    {
        public static void ModifySectionBoxDepth(Document doc, View3D view, double additionalHeightInFeet)
        {
            if (view == null || !view.IsSectionBoxActive)
            {
                throw new InvalidOperationException("The active view does not have an active section box.");
            }

            using (Transaction trans = new Transaction(doc, "Modify Section Box Depth"))
            {
                trans.Start();

                BoundingBoxXYZ sectionBox = view.GetSectionBox();
                if (sectionBox != null)
                {
                    double additionalHeightInMM = additionalHeightInFeet * 0.3048;

                    sectionBox.Max = new XYZ(sectionBox.Max.X, sectionBox.Max.Y, sectionBox.Max.Z + additionalHeightInMM);
                    sectionBox.Min = new XYZ(sectionBox.Min.X, sectionBox.Min.Y, sectionBox.Min.Z - additionalHeightInMM);

                    view.SetSectionBox(sectionBox);
                }

                trans.Commit();
            }
        }
    }
}
