using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Task3.Utilities;

namespace Task3.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceThresholdCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Pick a room
                Room room = RoomUtils.PickRoom(uidoc);
                if (room == null)
                {
                    message = "No room selected.";
                    return Result.Failed;
                }

                // Get the floor and doors in the room
                Floor floor = FloorUtils.GetFloorInRoom(room, doc);
                List<FamilyInstance> doors = RoomUtils.GetDoorsInRoom(room, doc);

                if (floor == null || doors.Count == 0)
                {
                    message = "Room does not contain a floor or any doors.";
                    return Result.Failed;
                }

                using (Transaction trans = new Transaction(doc, "Create Threshold"))
                {
                    trans.Start();
                    foreach (var door in doors)
                    {
                        FloorUtils.CreateThreshold(doc, floor, door);
                    }
                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
