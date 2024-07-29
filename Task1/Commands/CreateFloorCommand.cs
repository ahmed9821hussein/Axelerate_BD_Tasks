using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Task1.Utilities;
using static Task1.Utilities.FloorCreationTester;
using static Task1.Utilities.CurveDefinitions;

namespace Task1.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateFloorCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Show TaskDialog to select the test case
            TaskDialog mainDialog = new TaskDialog("Select Test Case");
            mainDialog.MainInstruction = "Select the input curves for floor creation:";
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)1, "AxelerateCase");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)2, "Minimal Input");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)3, "Maximum Input");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)4, "Invalid Input");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)5, "L-Shape Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)6, "U-Shape Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)7, "Irregular Polygon Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)8, "Non-Planar Case");

            TaskDialogResult result = mainDialog.Show();

            switch (result)
            {
                case (TaskDialogResult)1:
                    TestCreateFloor(commandData, GetAxelerateCurves(), "AxelerateCase");
                    break;
                case (TaskDialogResult)2:
                    TestCreateFloor(commandData, GetMinimalCurves(), "Minimal Input");
                    break;
                case (TaskDialogResult)3:
                    TestCreateFloor(commandData, GetMaximumCurves(), "Maximum Input");
                    break;
                case (TaskDialogResult)4:
                    TestCreateFloor(commandData, GetInvalidCurves(), "Invalid Input");
                    break;
                case (TaskDialogResult)5:
                    TestCreateFloor(commandData, GetLShapeCurves(), "L-Shape Floor");
                    break;
                case (TaskDialogResult)6:
                    TestCreateFloor(commandData, GetUShapeCurves(), "U-Shape Floor");
                    break;
                case (TaskDialogResult)7:
                    TestCreateFloor(commandData, GetIrregularPolygonCurves(), "Irregular Polygon Floor");
                    break;
                case (TaskDialogResult)8:
                    TestCreateFloor(commandData, GetNonPlanarCurves(), "Non-Planar Case");
                    break;
                default:
                    TaskDialog.Show("Error", "No test case selected.");
                    return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
