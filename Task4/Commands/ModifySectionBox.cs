using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Task4.Utilities;

namespace Task4.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ModifySectionBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Step 1: Get the active view
                View3D activeView = doc.ActiveView as View3D;
                if (activeView == null)
                {
                    TaskDialog.Show("Error", "The active view is not a 3D view.");
                    return Result.Failed;
                }

                // Step 2: Modify the section box
                SectionBoxUtils.ModifySectionBoxDepth(doc, activeView, 10);

                TaskDialog.Show("Success", "Section box modified successfully.");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }
    }
}
