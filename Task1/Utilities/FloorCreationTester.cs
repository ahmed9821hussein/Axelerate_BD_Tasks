using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Task1.ExtensionMethods;

namespace Task1.Utilities
{
    public static class FloorCreationTester
    {
        public static void TestCreateFloor(ExternalCommandData commandData, List<Curve> curves, string testName)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Check if all points lie in the same XY plane
            if (!CurveMethods.AreCurvesPlanar(curves))
            {
                TaskDialog.Show($"Error in {testName}", "The curves do not lie in the same XY plane.");
                return;
            }

            try
            {
                CurveMethods.SortCurvesContiguous(commandData.Application.Application.Create, curves, false);
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Error in {testName}", $"Curve sorting failed: {ex.Message}");
                return;
            }

            // Check if the curves form a valid closed loop
            for (int i = 0; i < curves.Count; i++)
            {
                Curve currentCurve = curves[i];
                Curve nextCurve = curves[(i + 1) % curves.Count];

                if (!currentCurve.GetEndPoint(1).IsAlmostEqualTo(nextCurve.GetEndPoint(0)))
                {
                    TaskDialog.Show($"Error in {testName}", $"Curves do not form a closed loop at segment {i + 1}.");
                    return;
                }
            }

            CurveLoop curveLoop = CurveLoop.Create(curves);

            using (Transaction trans = new Transaction(doc, $"Create Floor - {testName}"))
            {
                trans.Start();

                Level level = LevelMethods.GetLevel(doc);
                if (level == null)
                {
                    TaskDialog.Show($"Error in {testName}", "No level found.");
                    return;
                }

                ElementId floorTypeId = Floor.GetDefaultFloorType(doc, false);
                try
                {
                    Floor floor = Floor.Create(doc, new List<CurveLoop> { curveLoop }, floorTypeId, level.Id);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show($"Error in {testName}", $"Floor creation failed: {ex.Message}");
                    trans.RollBack();
                    return;
                }

                trans.Commit();
            }
        }
    }
}
