
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
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Define the lines (assume all z components are 0)
            List<Curve> curves = new List<Curve>
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

            try
            {
                CurveExtensions.SortCurvesContiguous(commandData.Application.Application.Create, curves, false);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            CurveLoop curveLoop = CurveLoop.Create(curves);

            using (Transaction trans = new Transaction(doc, "Create Floor"))
            {
                trans.Start();

                Level level = LevelExtensions.GetLevel(doc);
                if (level == null)
                {
                    TaskDialog.Show("Error", "No level found.");
                    return Result.Failed;
                }

                ElementId floorTypeId = Floor.GetDefaultFloorType(doc, false);
                Floor floor = Floor.Create(doc, new List<CurveLoop> { curveLoop }, floorTypeId, level.Id);

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }

}