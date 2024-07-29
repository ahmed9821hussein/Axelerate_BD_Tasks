using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Task2.Utilities;

namespace Task2.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class PlaceWCCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            try
            {
                var wallReference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select a wall");
                var wallElement = doc.GetElement(wallReference) as Wall;

                if (wallElement == null)
                {
                    TaskDialog.Show("Error", "Please select a valid wall.");
                    return Result.Failed;
                }

                var wcFamilySymbol = new FilteredElementCollector(doc)
                                     .OfCategory(BuiltInCategory.OST_GenericModel)
                                     .WhereElementIsElementType()
                                     .Cast<FamilySymbol>()
                                     .FirstOrDefault(a => a.Name == "ADA"); // Update with the actual family name

                if (wcFamilySymbol == null)
                {
                    TaskDialog.Show("Error", "WC family not found.");
                    return Result.Failed;
                }

                var roomFound = RoomUtils.GetRoomNextToSelectedWall(wallElement, out var boundarySegmentList);

                if (roomFound == null)
                {
                    TaskDialog.Show("Error", "No room found next to the selected wall.");
                    return Result.Failed;
                }

                var selectedWallCurveInRoom = GeoUtils.GetWallCurveInsideRoom(boundarySegmentList, wallElement);
                var doorsLocPoints = RoomUtils.GetDoorsLocationInRoom(roomFound, boundarySegmentList);

                var familyLocationPoint = GeoUtils.FarthestPointFromDoors(doorsLocPoints, selectedWallCurveInRoom);
                var familyOrientation = GeoUtils.GetCorrectFamilyOrientation(roomFound, familyLocationPoint, selectedWallCurveInRoom, out var roomMidPoint);

                using (Transaction tr = new Transaction(doc, "Place WC Family"))
                {
                    tr.Start();

                    if (!wcFamilySymbol.IsActive)
                    {
                        wcFamilySymbol.Activate();
                    }

                    var familyCreated = doc.Create.NewFamilyInstance(familyLocationPoint, wcFamilySymbol, familyOrientation, wallElement, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    FamilyInstanceUtils.HandleFamilyOrientation(doc, familyCreated, familyOrientation, selectedWallCurveInRoom, familyLocationPoint, roomMidPoint);

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
