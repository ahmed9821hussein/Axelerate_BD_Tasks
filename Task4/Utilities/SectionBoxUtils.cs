using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Task4.Utilities
{
    public static class SectionBoxUtils
    {
        public static Level GetLevel(Document doc, ViewPlan floorPlanView)
        {
            return doc.GetElement(floorPlanView.GenLevel.Id) as Level;
        }
    }
}
