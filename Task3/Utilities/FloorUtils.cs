using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Task3.Utilities
{
    public static class GeometryUtils
    {
        public static List<XYZ> SortPointsClockwise(List<XYZ> points)
        {
            XYZ center = new XYZ(points.Average(p => p.X), points.Average(p => p.Y), points.Average(p => p.Z));
            return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X)).ToList();
        }

        public static bool IsPointCloseToRoomBoundaries(IList<IList<BoundarySegment>> boundarySegments, XYZ point, double maxDistance)
        {
            foreach (var boundary in boundarySegments)
            {
                foreach (var segment in boundary)
                {
                    var curve = segment.GetCurve();
                    if (curve.Distance(point) <= maxDistance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
