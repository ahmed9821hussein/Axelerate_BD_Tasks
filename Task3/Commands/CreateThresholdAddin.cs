using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using Task3.Utilities;

namespace Task3.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateThresholdAddin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Step 1: Select the Room
                Room selectedRoom = RoomUtils.SelectRoom(uidoc);
                if (selectedRoom == null)
                {
                    TaskDialog.Show("Error", "No room selected or found.");
                    return Result.Failed;
                }

                // Step 2: Get Points and Create Floor
                List<XYZ> points = RoomUtils.GetRoomAndDoorPoints(doc, selectedRoom);
                points = GeometryUtils.SortPointsClockwise(points);
                RoomUtils.CreateFloorFromPoints(doc, points, selectedRoom.LevelId);

                TaskDialog.Show("Success", "Floor with thresholds added successfully.");
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
