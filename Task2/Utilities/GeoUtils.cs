using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Task2.Utilities
{
    public static class GeoUtils
    {
        public static XYZ FarthestPointFromDoors(List<XYZ> doorPoints, Curve wallCurve)
        {
            XYZ doorPoint = doorPoints.First();
            IList<XYZ> wallPoints = wallCurve.Tessellate();

            double maxDistance = double.MinValue;
            XYZ farthestPoint = null;

            foreach (XYZ wallPoint in wallPoints)
            {
                double distance = wallPoint.DistanceTo(doorPoint);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = wallPoint;
                }
            }

            return farthestPoint;
        }

        public static bool IsVerticalCurve(Curve curve)
        {
            return Math.Abs(curve.GetEndPoint(0).X - curve.GetEndPoint(1).X) < 0.001;
        }

        public static Curve GetWallCurveInsideRoom(IList<BoundarySegment> boundarySegmentList, Wall wallElement)
        {
            var matchingSegment = boundarySegmentList.FirstOrDefault(boundSeg => boundSeg.ElementId == wallElement.Id);

            return matchingSegment?.GetCurve();
        }

        public static XYZ GetCorrectFamilyOrientation(Room room, XYZ point, Curve wallCurve, out XYZ roomCentroid)
        {
            roomCentroid = (room.Location as LocationPoint).Point;
            XYZ directionVector = roomCentroid - point;

            if (IsVerticalCurve(wallCurve))
            {
                directionVector = new XYZ(Math.Sign(directionVector.X), 0, 0);
            }
            else
            {
                directionVector = new XYZ(0, Math.Sign(directionVector.Y), 0);
            }

            return directionVector;
        }
    }
}
