using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Task2.Utilities;

namespace Task2.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceWCCommand : IExternalCommand
    {
        public static UIDocument CommandUIdoc { get; set; }
        public static Document CommandDoc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application CommandApp { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CommandApp = commandData.Application.Application;
            CommandUIdoc = commandData.Application.ActiveUIDocument;
            CommandDoc = CommandUIdoc.Document;

            IList<BoundarySegment> boundarySegmentList = new List<BoundarySegment>();
            XYZ roomMidPoint = XYZ.Zero;

            try
            {
                var wallReference = CommandUIdoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                var wallElement = CommandDoc.GetElement(wallReference) as Wall;

                if (wallElement == null)
                {
                    TaskDialog.Show("Error", "Please select a valid wall.");
                    return Result.Failed;
                }

                var wcFamilySymbol = new FilteredElementCollector(CommandDoc)
                                     .OfCategory(BuiltInCategory.OST_GenericModel)
                                     .WhereElementIsElementType()
                                     .Where(a => a.Name == "ADA")
                                     .Cast<FamilySymbol>()
                                     .FirstOrDefault();

                if (wcFamilySymbol == null)
                {
                    TaskDialog.Show("Error", "WC family not found.");
                    return Result.Failed;
                }

                var roomFound = RoomUtils.GetRoomsNextToSelectedWall(wallElement, out boundarySegmentList);

                if (roomFound == null)
                {
                    TaskDialog.Show("Error", "No room found next to the selected wall.");
                    return Result.Failed;
                }

                var selectedWallCurveInRoom = GeoUtils.GetWallCurveInsideRoom(boundarySegmentList, wallElement);
                var doorsLocPoints = RoomUtils.GetDoorsLocationInRoom(roomFound, boundarySegmentList);

                if (doorsLocPoints == null || doorsLocPoints.Count == 0)
                {
                    TaskDialog.Show("Error", "No doors found in the room.");
                    return Result.Failed;
                }

                var familyLocationPoint = GeoUtils.FarthestPointToCurve(doorsLocPoints, selectedWallCurveInRoom);
                var familyOrientation = GeoUtils.GetCorrectFamilyOrientation(roomFound, familyLocationPoint, selectedWallCurveInRoom, out roomMidPoint);

                using (Transaction tr = new Transaction(CommandDoc, "Place Family"))
                {
                    tr.Start();

                    if (!wcFamilySymbol.IsActive)
                    {
                        wcFamilySymbol.Activate();
                    }

                    var familyCreated = CommandDoc.Create.NewFamilyInstance(familyLocationPoint, wcFamilySymbol, familyOrientation, wallElement, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                    FamilyInstanceUtils.FamilyOrientationHandler(CommandDoc, familyCreated, familyOrientation, selectedWallCurveInRoom, familyLocationPoint, roomMidPoint);

                    tr.Commit();
                }

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
