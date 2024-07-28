using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Task1.ExtensionMethods
{
    public static class CurveExtensions
    {
        private const double _sixteenth = (1.0 / 12.0) / 16.0;
        public static bool AreCurvesPlanar(List<Curve> curves)
        {
            // Check if all points lie in the same XY plane (Z-coordinate should be consistent)
            double zValue = curves[0].GetEndPoint(0).Z;

            foreach (Curve curve in curves)
            {
                if (curve.GetEndPoint(0).Z != zValue || curve.GetEndPoint(1).Z != zValue)
                {
                    return false;
                }
            }

            return true;
        }
        public static Curve CreateReversedCurve(Application creapp, Curve orig)
        {
            if (orig is Line)
            {
                return Line.CreateBound(orig.GetEndPoint(1), orig.GetEndPoint(0));
            }
            else if (orig is Arc)
            {
                return Arc.Create(orig.GetEndPoint(1), orig.GetEndPoint(0), orig.Evaluate(0.5, true));
            }
            else
            {
                throw new NotImplementedException("CreateReversedCurve for type " + orig.GetType().Name);
            }
        }

        public static void SortCurvesContiguous(Application creapp, IList<Curve> curves, bool debug_output)
        {
            int n = curves.Count;

            for (int i = 0; i < n; ++i)
            {
                Curve curve = curves[i];
                XYZ endPoint = curve.GetEndPoint(1);

                XYZ p;

                bool found = (i + 1 >= n);

                for (int j = i + 1; j < n; ++j)
                {
                    p = curves[j].GetEndPoint(0);

                    if (_sixteenth > p.DistanceTo(endPoint))
                    {
                        if (i + 1 != j)
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }

                    p = curves[j].GetEndPoint(1);

                    if (_sixteenth > p.DistanceTo(endPoint))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = CreateReversedCurve(creapp, curves[j]);
                        }
                        else
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = CreateReversedCurve(creapp, curves[j]);
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous: non-contiguous input curves");
                }
            }
        }
    }
}
