using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;

namespace Task3.Utilities
{
    public static class FloorUtils
    {
        public static Floor GetFloorInRoom(Room room, Document doc)
        {
            var floor = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .Cast<Floor>()
                .FirstOrDefault(f => IsFloorInRoom(f, room));
            return floor;
        }

        private static bool IsFloorInRoom(Floor floor, Room room)
        {
            var bbox = floor.get_BoundingBox(null);
            var roomCenter = (room.Location as LocationPoint).Point;

            return bbox.Contains(roomCenter);
        }

        public static void CreateThreshold(Document doc, Floor floor, FamilyInstance door)
        {
            var doorLocation = (door.Location as LocationPoint).Point;
            var bbox = door.get_BoundingBox(null);
            var thresholdWidth = (bbox.Max.X - bbox.Min.X) / 2.0;

            XYZ p1 = doorLocation + new XYZ(-thresholdWidth, 0, 0);
            XYZ p2 = doorLocation + new XYZ(thresholdWidth, 0, 0);

            Line thresholdLine = Line.CreateBound(p1, p2);

            CurveArray curveArray = new CurveArray();
            curveArray.Append(thresholdLine);

            doc.Create.NewOpening(floor, curveArray, true);
        }

        private static bool Contains(this BoundingBoxXYZ bbox, XYZ point)
        {
            return bbox.Min.X <= point.X && point.X <= bbox.Max.X
                && bbox.Min.Y <= point.Y && point.Y <= bbox.Max.Y;
        }
    }
}
