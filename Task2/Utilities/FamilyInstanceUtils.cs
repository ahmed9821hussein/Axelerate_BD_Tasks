using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;

namespace Task2.Utilities
{
    public static class FamilyInstanceUtils
    {
        public static void HandleFamilyOrientation(Document doc, FamilyInstance familyInstance, XYZ orientation, Curve wallCurve, XYZ location, XYZ roomMidPoint)
        {
            if (!familyInstance.FacingOrientation.IsAlmostEqualTo(orientation))
            {
                familyInstance.flipFacing();
            }

            if (GeoUtils.IsVerticalCurve(wallCurve))
            {
                VerticalOrientationCriteria(familyInstance, wallCurve, location, roomMidPoint, doc);
            }
            else
            {
                HorizontalOrientationCriteria(familyInstance, wallCurve, location, roomMidPoint, doc);
            }
        }

        private static void VerticalOrientationCriteria(FamilyInstance familyInstance, Curve wallCurve, XYZ location, XYZ roomMidPoint, Document doc)
        {
            XYZ direction = location - roomMidPoint;
            bool flipRequired = Math.Abs(wallCurve.GetEndPoint(0).X - wallCurve.GetEndPoint(1).X) < 0.001;

            if (flipRequired && direction.Y < 0)
            {
                if (!familyInstance.HandOrientation.IsAlmostEqualTo(new XYZ(0, 1, 0)))
                {
                    familyInstance.flipHand();
                }
                ElementTransformUtils.MoveElement(doc, familyInstance.Id, new XYZ(0, 1.5, 0));
            }
            else if (flipRequired && direction.Y >= 0)
            {
                if (!familyInstance.HandOrientation.IsAlmostEqualTo(new XYZ(0, -1, 0)))
                {
                    familyInstance.flipHand();
                }
                ElementTransformUtils.MoveElement(doc, familyInstance.Id, new XYZ(0, -1.5, 0));
            }
        }

        private static void HorizontalOrientationCriteria(FamilyInstance familyInstance, Curve wallCurve, XYZ location, XYZ roomMidPoint, Document doc)
        {
            XYZ direction = location - roomMidPoint;
            bool flipRequired = Math.Abs(wallCurve.GetEndPoint(0).Y - wallCurve.GetEndPoint(1).Y) < 0.001;

            if (flipRequired && direction.X < roomMidPoint.X)
            {
                if (!familyInstance.HandOrientation.IsAlmostEqualTo(new XYZ(1, 0, 0)))
                {
                    familyInstance.flipHand();
                }
                ElementTransformUtils.MoveElement(doc, familyInstance.Id, new XYZ(1.5, 0, 0));
            }
            else if (flipRequired && direction.X >= roomMidPoint.X)
            {
                if (!familyInstance.HandOrientation.IsAlmostEqualTo(new XYZ(-1, 0, 0)))
                {
                    familyInstance.flipHand();
                }
                ElementTransformUtils.MoveElement(doc, familyInstance.Id, new XYZ(-1.5, 0, 0));
            }
        }
    }
}
