using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Task3.Utilities
{
    public static class RoomUtils
    {
        public static Room SelectRoom(UIDocument uidoc)
        {
            Reference pickedRef = uidoc.Selection.PickObject(ObjectType.Element, "Select a room");
            return uidoc.Document.GetElement(pickedRef) as Room;
        }

        public static List<XYZ> GetRoomAndDoorPoints(Document doc, Room room)
        {
            List<XYZ> points = new List<XYZ>();
            var boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            // Get Room Corner Points
            foreach (var boundary in boundarySegments)
            {
                foreach (var segment in boundary)
                {
                    points.Add(segment.GetCurve().GetEndPoint(0));
                }
            }

            // Get Door Points and Calculate Door Boundary Points
            foreach (var boundary in boundarySegments)
            {
                foreach (var segment in boundary)
                {
                    var wall = doc.GetElement(segment.ElementId) as Wall;
                    if (wall != null)
                    {
                        var doors = new FilteredElementCollector(doc)
                                    .OfCategory(BuiltInCategory.OST_Doors)
                                    .WhereElementIsNotElementType()
                                    .Cast<FamilyInstance>()
                                    .Where(d => d.Host.Id == wall.Id);

                        foreach (var door in doors)
                        {
                            var doorLocation = (door.Location as LocationPoint).Point;

                            var doorWidth = door.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH).AsDouble();
                            var halfWallThickness = wall.Width / 2;

                            // Calculate start and end points of the door
                            XYZ doorDirection = wall.Orientation;
                            XYZ doorRight = doorDirection.CrossProduct(XYZ.BasisZ);
                            XYZ p1 = doorLocation - doorRight * (doorWidth / 2);
                            XYZ p2 = doorLocation + doorRight * (doorWidth / 2);

                            // Calculate inner wall points using perpendicular projection
                            XYZ p1_inner = p1 + doorDirection * (halfWallThickness / 4);
                            XYZ p2_inner = p2 + doorDirection * (halfWallThickness / 4);

                            // Calculate intersection points with inner wall line
                            XYZ p1_wall = p1 - doorDirection * halfWallThickness;
                            XYZ p2_wall = p2 - doorDirection * halfWallThickness;

                            // Add points if they are within 30 cm of any room boundary
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p1,0.7))
                                points.Add(p1);
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p2, 0.7))
                                points.Add(p2);
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p1_inner, 0.7))
                                points.Add(p1_inner);
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p2_inner, 0.7))
                                points.Add(p2_inner);
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p1_wall, 0.7))
                                points.Add(p1_wall);
                            if (GeometryUtils.IsPointCloseToRoomBoundaries(boundarySegments, p2_wall, 0.7))
                                points.Add(p2_wall);
                        }
                    }
                }
            }

            return points;
        }

        public static void CreateFloorFromPoints(Document doc, List<XYZ> points, ElementId levelId)
        {
            CurveLoop curveLoop = new CurveLoop();
            for (int i = 0; i < points.Count; i++)
            {
                curveLoop.Append(Line.CreateBound(points[i], points[(i + 1) % points.Count]));
            }

            ElementId floorTypeId = Floor.GetDefaultFloorType(doc, false);
            Level level = doc.GetElement(levelId) as Level;

            using (Transaction trans = new Transaction(doc, "Create Combined Floor"))
            {
                trans.Start();
                Floor newFloor = Floor.Create(doc, new List<CurveLoop> { curveLoop }, floorTypeId, level.Id);
                trans.Commit();
            }
        }
    }
}
