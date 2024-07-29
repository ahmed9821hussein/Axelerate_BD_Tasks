using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Task2.Utilities
{
    public static class RoomUtils
    {
        public static Room GetRoomNextToSelectedWall(Wall selectedWall, out IList<BoundarySegment> boundarySegment)
        {
            boundarySegment = null;
            Room roomFound = null;

            var bathroomRooms = new FilteredElementCollector(selectedWall.Document)
                          .OfCategory(BuiltInCategory.OST_Rooms)
                          .WhereElementIsNotElementType()
                          .Cast<Room>()
                          .Where(room => room.Name.Contains("Bathroom"))
                          .ToList();

            if (!bathroomRooms.Any())
            {
                TaskDialog.Show("Alert", "No Bathrooms Found!");
                return null;
            }

            SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            };

            foreach (Room room in bathroomRooms)
            {
                foreach (var boundarySegments in room.GetBoundarySegments(options))
                {
                    foreach (var segment in boundarySegments)
                    {
                        if (segment.ElementId == selectedWall.Id)
                        {
                            roomFound = room;
                            boundarySegment = boundarySegments;
                            return roomFound;
                        }
                    }
                }
            }

            return roomFound;
        }

        public static List<XYZ> GetDoorsLocationInRoom(Room room, IList<BoundarySegment> boundarySegmentList)
        {
            var doorPoints = new List<XYZ>();

            foreach (var segment in boundarySegmentList)
            {
                var wall = room.Document.GetElement(segment.ElementId) as Wall;
                if (wall == null)
                {
                    continue;
                }

                var hostedElements = wall.FindInserts(true, true, true, true);
                var doorInstances = new FilteredElementCollector(room.Document, hostedElements)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .ToList();

                foreach (var door in doorInstances)
                {
                    var location = door.Location as LocationPoint;
                    if (location != null)
                    {
                        doorPoints.Add(location.Point);
                    }
                }
            }

            if (!doorPoints.Any())
            {
                TaskDialog.Show("Error", "No doors found in the room.");
            }

            return doorPoints;
        }
    }
}
