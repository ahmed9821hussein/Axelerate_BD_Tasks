using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Task1.Utilities
{
    public static class CurveDefinitions
    {
        public static List<Curve> GetMinimalCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(5, 0, 0), new XYZ(5, 5, 0)),
                Line.CreateBound(new XYZ(5, 5, 0), new XYZ(0, 5, 0)),
                Line.CreateBound(new XYZ(0, 5, 0), new XYZ(0, 0, 0))
            };
        }

        public static List<Curve> GetMaximumCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1000, 0, 0)),
                Line.CreateBound(new XYZ(1000, 0, 0), new XYZ(1000, 1000, 0)),
                Line.CreateBound(new XYZ(1000, 1000, 0), new XYZ(0, 1000, 0)),
                Line.CreateBound(new XYZ(0, 1000, 0), new XYZ(0, 0, 0))
            };
        }

        public static List<Curve> GetInvalidCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(15, 5, 0)) // Non-contiguous curve
            };
        }

        public static List<Curve> GetAxelerateCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(79, 0, 0)),
                Line.CreateBound(new XYZ(44, 25, 0), new XYZ(13, 25, 0)),
                Line.CreateBound(new XYZ(13, 40, 0), new XYZ(-8, 40, 0)),
                Line.CreateBound(new XYZ(55, 34, 0), new XYZ(55, 10, 0)),
                Line.CreateBound(new XYZ(79, 34, 0), new XYZ(55, 34, 0)),
                Line.CreateBound(new XYZ(0, 20, 0), new XYZ(0, 0, 0)),
                Line.CreateBound(new XYZ(55, 10, 0), new XYZ(44, 12, 0)),
                Line.CreateBound(new XYZ(-8, 40, 0), new XYZ(-8, 20, 0)),
                Line.CreateBound(new XYZ(79, 0, 0), new XYZ(79, 34, 0)),
                Line.CreateBound(new XYZ(44, 12, 0), new XYZ(44, 25, 0)),
                Line.CreateBound(new XYZ(-8, 20, 0), new XYZ(0, 20, 0)),
                Line.CreateBound(new XYZ(13, 25, 0), new XYZ(13, 40, 0))
            };
        }

        public static List<Curve> GetLShapeCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(10, 5, 0)),
                Line.CreateBound(new XYZ(10, 5, 0), new XYZ(5, 5, 0)),
                Line.CreateBound(new XYZ(5, 5, 0), new XYZ(5, 10, 0)),
                Line.CreateBound(new XYZ(5, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };
        }

        public static List<Curve> GetUShapeCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0)),
                Line.CreateBound(new XYZ(10, 0, 0), new XYZ(10, 10, 0)),
                Line.CreateBound(new XYZ(10, 10, 0), new XYZ(7, 10, 0)),
                Line.CreateBound(new XYZ(7, 10, 0), new XYZ(7, 3, 0)),
                Line.CreateBound(new XYZ(7, 3, 0), new XYZ(3, 3, 0)),
                Line.CreateBound(new XYZ(3, 3, 0), new XYZ(3, 10, 0)),
                Line.CreateBound(new XYZ(3, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };
        }

        public static List<Curve> GetIrregularPolygonCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 2, 0)),
                Line.CreateBound(new XYZ(5, 2, 0), new XYZ(7, 5, 0)),
                Line.CreateBound(new XYZ(7, 5, 0), new XYZ(4, 7, 0)),
                Line.CreateBound(new XYZ(4, 7, 0), new XYZ(1, 4, 0)),
                Line.CreateBound(new XYZ(1, 4, 0), new XYZ(0, 0, 0))
            };
        }

        public static List<Curve> GetNonPlanarCurves()
        {
            return new List<Curve>
            {
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)),
                Line.CreateBound(new XYZ(5, 0, 0), new XYZ(5, 2, 1)), // Z-coordinate is different
                Line.CreateBound(new XYZ(5, 2, 1), new XYZ(0, 2, 0)),
                Line.CreateBound(new XYZ(0, 2, 0), new XYZ(0, 4, 0)),
                Line.CreateBound(new XYZ(0, 4, 0), new XYZ(5, 4, 0)),
                Line.CreateBound(new XYZ(5, 4, 0), new XYZ(5, 6, 0)),
                Line.CreateBound(new XYZ(5, 6, 0), new XYZ(0, 6, 0)),
                Line.CreateBound(new XYZ(0, 6, 0), new XYZ(0, 8, 0)),
                Line.CreateBound(new XYZ(0, 8, 0), new XYZ(5, 8, 0)),
                Line.CreateBound(new XYZ(5, 8, 0), new XYZ(5, 10, 0)),
                Line.CreateBound(new XYZ(5, 10, 0), new XYZ(0, 10, 0)),
                Line.CreateBound(new XYZ(0, 10, 0), new XYZ(0, 0, 0))
            };
        }
    }
}
