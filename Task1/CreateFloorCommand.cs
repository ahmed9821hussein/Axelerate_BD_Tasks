using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Task1.ExtensionMethods;

namespace Task1
{
    [Transaction(TransactionMode.Manual)]
    public class CreateFloorCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Define input curves for different test cases
            List<Curve> minimalCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(5, 0, 0), new XYZ(5, 5, 0)),
                Line.CreateBound(new XYZ(5, 5, 0), new XYZ(0, 5, 0)),
                Line.CreateBound(new XYZ(0, 5, 0), new XYZ(0, 0, 0))
            };

            List<Curve> maximumCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1000, 0, 0)),
                Line.CreateBound(new XYZ(1000, 0, 0), new XYZ(1000, 1000, 0)),
                Line.CreateBound(new XYZ(1000, 1000, 0), new XYZ(0, 1000, 0)),
                Line.CreateBound(new XYZ(0, 1000, 0), new XYZ(0, 0, 0))
            };

            List<Curve> invalidCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(15, 5, 0)) // Non-contiguous curve
            };

            // Define the lines for the AxelerateCase (assume all z components are 0)
            List<Curve> axelerateCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(79, 0, 0)),
                Line.CreateBound(new XYZ(44, 25, 0), new XYZ(13, 25, 0)),
                Line.CreateBound(new XYZ(13, 40, 0), new XYZ(-8, 40, 0)),
                Line.CreateBound(new XYZ(55, 34, 0), new XYZ(55, 10, 0)),
                Line.CreateBound(new XYZ(79, 34, 0), new XYZ(55, 34, 0)),
                Line.CreateBound(new XYZ(0, 20, 0), new XYZ(0, 0, 0)),
                Line.CreateBound(new XYZ(55, 10, 0), new XYZ(44, 12, 0)),
                Line.CreateBound(new XYZ(-8, 40, 0), new XYZ(-8, 20, 0)),
                Line.CreateBound(new XYZ(79, 0, 0), new XYZ(79, 34, 0)),
                Line.CreateBound(new XYZ(44, 12, 0), new XYZ(44, 25, 0)),
                Line.CreateBound(new XYZ(-8, 20, 0), new XYZ(0, 20, 0)),
                Line.CreateBound(new XYZ(13, 25, 0), new XYZ(13, 40, 0))
            };

            // Define the lines for the L-Shape Case
            List<Curve> lShapeCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(10, 5, 0)),
                Line.CreateBound(new XYZ(10, 5, 0), new XYZ(5, 5, 0)),
                Line.CreateBound(new XYZ(5, 5, 0), new XYZ(5, 10, 0)),
                Line.CreateBound(new XYZ(5, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };

            // Define the lines for the U-Shape Case
            List<Curve> uShapeCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(10, 5, 0)),
                Line.CreateBound(new XYZ(10, 5, 0), new XYZ(5, 5, 0)),
                Line.CreateBound(new XYZ(5, 5, 0), new XYZ(5, 10, 0)),
                Line.CreateBound(new XYZ(5, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };

            // Define the lines for the Irregular Polygon Case
            List<Curve> irregularPolygonCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 2, 0)),
                Line.CreateBound(new XYZ(5, 2, 0), new XYZ(7, 5, 0)),
                Line.CreateBound(new XYZ(7, 5, 0), new XYZ(4, 7, 0)),
                Line.CreateBound(new XYZ(4, 7, 0), new XYZ(1, 4, 0)),
                Line.CreateBound(new XYZ(1, 4, 0), new XYZ(0, 0, 0))
            };

            // Define the lines for the Zig-Zag Shape Case
            List<Curve> zigZagCurves = new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(5, 0, 0), new XYZ(5, 2, 0)),
                Line.CreateBound(new XYZ(5, 2, 0), new XYZ(0, 2, 0)),
                Line.CreateBound(new XYZ(0, 2, 0), new XYZ(0, 4, 0)),
                Line.CreateBound(new XYZ(0, 4, 0), new XYZ(5, 4, 0)),
                Line.CreateBound(new XYZ(5, 4, 0), new XYZ(5, 6, 0)),
                Line.CreateBound(new XYZ(5, 6, 0), new XYZ(0, 6, 0)),
                Line.CreateBound(new XYZ(0, 6, 0), new XYZ(0, 8, 0)),
                Line.CreateBound(new XYZ(0, 8, 0), new XYZ(5, 8, 0)),
                Line.CreateBound(new XYZ(5, 8, 0), new XYZ(5, 10, 0)),
                Line.CreateBound(new XYZ(5, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };

             // Show TaskDialog to select the test case
            TaskDialog mainDialog = new TaskDialog("Select Test Case");
            mainDialog.MainInstruction = "Select the input curves for floor creation:";
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Minimal Input");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Maximum Input");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, "Invalid Input");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "AxelerateCase");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)5, "L-Shape Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)6, "U-Shape Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)7, "Irregular Polygon Floor");
            mainDialog.AddCommandLink((TaskDialogCommandLinkId)8, "Zig-Zag Shape Floor");

            TaskDialogResult result = mainDialog.Show();

            switch (result)
            {
                case TaskDialogResult.CommandLink1:
                    TestCreateFloor(commandData, minimalCurves, "Minimal Input");
                    break;
                case TaskDialogResult.CommandLink2:
                    TestCreateFloor(commandData, maximumCurves, "Maximum Input");
                    break;
                case TaskDialogResult.CommandLink3:
                    TestCreateFloor(commandData, invalidCurves, "Invalid Input");
                    break;
                case TaskDialogResult.CommandLink4:
                    TestCreateFloor(commandData, axelerateCurves, "AxelerateCase");
                    break;
                case (TaskDialogResult)5:
                    TestCreateFloor(commandData, lShapeCurves, "L-Shape Floor");
                    break;
                case (TaskDialogResult)6:
                    TestCreateFloor(commandData, uShapeCurves, "U-Shape Floor");
                    break;
                case (TaskDialogResult)7:
                    TestCreateFloor(commandData, irregularPolygonCurves, "Irregular Polygon Floor");
                    break;
                case (TaskDialogResult)8:
                    TestCreateFloor(commandData, zigZagCurves, "Zig-Zag Shape Floor");
                    break;
                default:
                    TaskDialog.Show("Error", "No test case selected.");
                    return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        private void TestCreateFloor(ExternalCommandData commandData, List<Curve> curves, string testName)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                CurveExtensions.SortCurvesContiguous(commandData.Application.Application.Create, curves, false);
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Error in {testName}", ex.Message);
                return;
            }

            CurveLoop curveLoop = CurveLoop.Create(curves);

            using (Transaction trans = new Transaction(doc, $"Create Floor - {testName}"))
            {
                trans.Start();

                Level level = LevelExtensions.GetLevel(doc);
                if (level == null)
                {
                    TaskDialog.Show($"Error in {testName}", "No level found.");
                    return;
                }

                ElementId floorTypeId = Floor.GetDefaultFloorType(doc, false);
                Floor floor = Floor.Create(doc, new List<CurveLoop> { curveLoop }, floorTypeId, level.Id);

                trans.Commit();
            }
        }
    }
}
