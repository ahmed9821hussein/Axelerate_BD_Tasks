using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using Task2.Commands;

namespace Task2.Utilities
{
    public static class RoomUtils
    {

        public static Room GetRoomsNextToSelectedWall(Wall selectedWall, out IList<BoundarySegment> boundarySegment)
        {
            boundarySegment = null;
            Room roomsFound = null;

            var bathroomRooms = new FilteredElementCollector(PlaceWCCommand.CommandDoc)
                                .OfCategory(BuiltInCategory.OST_Rooms)
                                .WhereElementIsNotElementType()
                                .Where(room => room.Name.Contains("Bathroom"))
                                .Cast<Room>()
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
                foreach (IList<BoundarySegment> boundSegList in room.GetBoundarySegments(options))
                {
                    foreach (BoundarySegment boundSeg in boundSegList)
                    {
                        if (IsWallSegmentOfSelectedWall(boundSeg, selectedWall))
                        {
                            roomsFound = room;
                            boundarySegment = boundSegList;
                        }
                    }
                }
            }
            return roomsFound;
        }
        private static bool IsWallSegmentOfSelectedWall(BoundarySegment boundSeg, Wall selectedWall)
        {
            Element e = PlaceWCCommand.CommandDoc.GetElement(boundSeg.ElementId);
            Wall wall = e as Wall;
            return wall != null && wall.Id == selectedWall.Id;
        }

        public static bool CheckForExistingWCFamily(Document doc, Room room, string familyName)
        {
            var familyInstances = new FilteredElementCollector(doc)
                                  .OfClass(typeof(FamilyInstance))
                                  .WhereElementIsNotElementType()
                                  .Cast<FamilyInstance>()
                                  .Where(fi => fi.Symbol.Family.Name == familyName)
                                  .ToList();

            foreach (var instance in familyInstances)
            {
                if (instance.Room?.Id == room.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<XYZ> GetDoorsLocationInRoom(Room room, IList<BoundarySegment> boundarySegmentList)
        {
            if (boundarySegmentList == null)
            {
                return null;
            }

            List<XYZ> doorsLocationPoint = new List<XYZ>();

            foreach (BoundarySegment boundSeg in boundarySegmentList)
            {
                if (boundSeg == null)
                {
                    continue;
                }

                var wallInRoom = PlaceWCCommand.CommandDoc.GetElement(boundSeg.ElementId) as Wall;

                if (wallInRoom == null || !(wallInRoom is HostObject))
                {
                    continue;
                }

                var wallHostObj = wallInRoom as HostObject;
                var hostedElementsOnWall = wallHostObj.FindInserts(true, true, true, true);

                if (hostedElementsOnWall != null && hostedElementsOnWall.Any())
                {
                    var famInstanceCollector = new FilteredElementCollector(PlaceWCCommand.CommandDoc, hostedElementsOnWall)
                        .OfCategory(BuiltInCategory.OST_Doors)
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(A => A != null && (A.ToRoom?.Name == room.Name || A.FromRoom?.Name == room.Name))
                        .ToList();

                    doorsLocationPoint.AddRange(famInstanceCollector.Select(famInstance => (famInstance.Location as LocationPoint)?.Point)
                        .Where(locationPoint => locationPoint != null));
                }
            }

            if (!doorsLocationPoint.Any())
            {
                TaskDialog.Show("Error", $"No doors in this room");
                return null;
            }

            return doorsLocationPoint;
        }
        public static List<Curve> GetDoorsCurvesInRoom(Room room, IList<BoundarySegment> boundarySegmentList)
        {
            if (boundarySegmentList == null)
            {
                return null;
            }

            List<Curve> doorsCurves = new List<Curve>();

            foreach (BoundarySegment boundSeg in boundarySegmentList)
            {
                if (boundSeg == null)
                {
                    continue;
                }

                var wallInRoom = PlaceWCCommand.CommandDoc.GetElement(boundSeg.ElementId) as Wall;

                if (wallInRoom == null || !(wallInRoom is HostObject))
                {
                    continue;
                }

                var wallHostObj = wallInRoom as HostObject;
                var hostedElementsOnWall = wallHostObj.FindInserts(true, true, true, true);

                if (hostedElementsOnWall != null && hostedElementsOnWall.Any())
                {
                    var famInstanceCollector = new FilteredElementCollector(PlaceWCCommand.CommandDoc, hostedElementsOnWall)
                        .OfCategory(BuiltInCategory.OST_Doors)
                        .WhereElementIsNotElementType()
                        .Cast<FamilyInstance>()
                        .Where(A => A != null && (A.ToRoom?.Name == room.Name || A.FromRoom?.Name == room.Name))
                        .ToList();

                    foreach (var famInstance in famInstanceCollector)
                    {
                        var geometryElement = famInstance.get_Geometry(new Options());
                        foreach (var geometryInstance in geometryElement)
                        {
                            var instanceGeometry = geometryInstance as GeometryInstance;
                            if (instanceGeometry != null)
                            {
                                foreach (var geometryObject in instanceGeometry.GetInstanceGeometry())
                                {
                                    var curve = geometryObject as Curve;
                                    if (curve != null)
                                    {
                                        doorsCurves.Add(curve);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!doorsCurves.Any())
            {
                TaskDialog.Show("Error", $"No doors in this room");
                return null;
            }

            return doorsCurves;
        }
        public static List<FamilyInstance> GetWCInstancesInRoom(Room room, Document doc)
        {
            var wcInstances = new List<FamilyInstance>();

            // Get the bounding walls of the room
            var boundingWalls = GetWallsBoundingRoom(room, doc);

            if (!boundingWalls.Any())
            {
                TaskDialog.Show("Info", "No bounding walls found for the room.");
                return wcInstances;
            }

            // Iterate through the bounding walls to find hosted WC family instances
            foreach (var wall in boundingWalls)
            {
                var hostedElements = wall.FindInserts(true, true, true, true);
                foreach (var elementId in hostedElements)
                {
                    var familyInstance = doc.GetElement(elementId) as FamilyInstance;
                    if (familyInstance != null && familyInstance.Symbol.Family.Name == "ADA")
                    {
                        wcInstances.Add(familyInstance);
                    }
                }
            }


            return wcInstances;
        }

        public static List<Wall> GetWallsBoundingRoom(Room room, Document doc)
        {
            var boundaryOptions = new SpatialElementBoundaryOptions();
            var boundarySegments = room.GetBoundarySegments(boundaryOptions);
            var walls = new List<Wall>();

            foreach (var segmentList in boundarySegments)
            {
                foreach (var segment in segmentList)
                {
                    var element = doc.GetElement(segment.ElementId);
                    if (element is Wall wall)
                    {
                        walls.Add(wall);
                    }
                }
            }

            return walls;
        }


        private static bool IsPointOnCurve(Curve curve, XYZ point)
        {
            XYZ projectedPoint = curve.Project(point).XYZPoint;
            double distance = projectedPoint.DistanceTo(point);
            double tolerance = 0.001; // Tolerance for considering the point on the curve

            return distance < tolerance && curve.IsInside(projectedPoint);
        }

        private static bool IsInside(this Curve curve, XYZ point)
        {
            return curve.GetEndParameter(0) <= curve.Project(point).Parameter &&
                   curve.Project(point).Parameter <= curve.GetEndParameter(1);
        }


    }
}