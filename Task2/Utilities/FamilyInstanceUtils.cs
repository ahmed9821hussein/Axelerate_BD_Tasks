﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Task2.Utilities
{
    public class FamilyInstanceUtils
    {
        public static void MoveFamilyToCorrectPlace(FamilyInstance familyInstanceCreated, Curve wallCurve, IList<BoundarySegment> boundarySegment, Document doc)
        {
            var listOfWallCurves = boundarySegment.Select(boundSeg =>
            {
                Element e = doc.GetElement(boundSeg.ElementId);
                Wall wall = e as Wall;
                return (wall.Location as LocationCurve).Curve;
            }).ToList();

            var famInstanceCurves = FamilyInstanceBoundaryLines(5, 5, (familyInstanceCreated.Location as LocationPoint).Point);


            if (GeoUtils.AreLinesIntersecting(listOfWallCurves, famInstanceCurves))
            {
                // Determine the direction to move based on the closest point
                XYZ closestPointOnWallCurve = GeoUtils.GetClosestPointOnCurve(wallCurve, (familyInstanceCreated.Location as LocationPoint).Point);

                // Calculate the translation vector
                XYZ translationVector = GeoUtils.GetTranslationVector((familyInstanceCreated.Location as LocationPoint).Point, closestPointOnWallCurve);

                // Move the family instance
                ElementTransformUtils.MoveElement(doc, familyInstanceCreated.Id, translationVector);

                // Check for intersection after the move
                famInstanceCurves = FamilyInstanceBoundaryLines(5, 5, (familyInstanceCreated.Location as LocationPoint).Point);
                while (GeoUtils.AreLinesIntersecting(listOfWallCurves, famInstanceCurves))
                {
                    // Move further if still intersecting
                    translationVector = GeoUtils.GetTranslationVector((familyInstanceCreated.Location as LocationPoint).Point, closestPointOnWallCurve);
                    ElementTransformUtils.MoveElement(doc, familyInstanceCreated.Id, translationVector);

                    // Check for intersection again
                    famInstanceCurves = FamilyInstanceBoundaryLines(5, 5, (familyInstanceCreated.Location as LocationPoint).Point);
                }
            }
        }
        public static void VerticalHandOrientationCriterea(FamilyInstance familyCreated, Curve curveWall, XYZ familyLocationPoint, XYZ roomMidPoint, Document doc)
        {
            XYZ directionToFamily = familyLocationPoint - roomMidPoint;

            if (Math.Abs(curveWall.GetEndPoint(0).X - curveWall.GetEndPoint(1).X) < 0.001)
            {
                // Check if the wall is left or right of the room midpoint
                if (curveWall.GetEndPoint(0).X < roomMidPoint.X)
                {
                    // Wall is left of room midpoint
                    if (directionToFamily.Y < 0)
                    {
                        // Below room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(0, 1, 0)))
                        {
                            familyCreated.flipHand();
                        }

                        var translation = new XYZ(0, 1.5, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                    else
                    {
                        // Above room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(0, -1, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(0, -1.5, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                }
                else
                {
                    // Wall is right of room midpoint
                    if (directionToFamily.Y < 0)
                    {
                        // Below room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(0, 1, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(0, 1.5, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                    else
                    {
                        // Above room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(0, -1, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(0, -1.5, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                }
            }

        }
        public static void HorizontalHandOrientationCriteria(FamilyInstance familyCreated, Curve curveWall, XYZ familyLocationPoint, XYZ roomMidPoint, Document doc)
        {
            XYZ directionToFamily = familyLocationPoint - roomMidPoint;

            if (Math.Abs(curveWall.GetEndPoint(0).Y - curveWall.GetEndPoint(1).Y) < 0.001)
            {
                // Check if the wall is left or right of the room midpoint
                if (familyLocationPoint.X < roomMidPoint.X)
                {
                    // Wall is left of room midpoint
                    if (directionToFamily.Y < 0)
                    {
                        // Below room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(1, 0, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(1.5, 0, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                    else
                    {
                        // Above room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(1, 0, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(1.5, 0, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                }
                else
                {
                    // Wall is right of room midpoint
                    if (directionToFamily.Y < 0)
                    {
                        // Below room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(-1, 0, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(-1.5, 0, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                    else
                    {
                        // Above room centroid
                        if (!familyCreated.HandOrientation.IsAlmostEqualTo(new XYZ(-1, 0, 0)))
                        {
                            familyCreated.flipHand();
                        }
                        var translation = new XYZ(-1.5, 0, 0);
                        ElementTransformUtils.MoveElement(doc, familyCreated.Id, translation);
                    }
                }
            }
        }
        public static void FamilyOrientationHandler(Document doc, FamilyInstance familyCreated, XYZ familyOrientation, Curve selectedWallCurveInRoom, XYZ familyLocationPoint, XYZ roomMidPoint)
        {
            if (!familyCreated.FacingOrientation.IsAlmostEqualTo(familyOrientation))
            {
                familyCreated.flipFacing();
            }

            if (GeoUtils.IsVerticalCurve(selectedWallCurveInRoom))
            {
                VerticalHandOrientationCriterea(familyCreated, selectedWallCurveInRoom, familyLocationPoint, roomMidPoint, doc);
            }
            else
            {
                HorizontalHandOrientationCriteria(familyCreated, selectedWallCurveInRoom, familyLocationPoint, roomMidPoint, doc);
            }
        }
        public static void MoveFamilyToAvoidOverlap(FamilyInstance newFamilyInstance, List<FamilyInstance> existingFamilies, Curve wallCurve, Document doc)
        {
            var newFamilyLocation = (newFamilyInstance.Location as LocationPoint).Point;
            var newFamilyCurves = FamilyInstanceBoundaryLines(5, 5, newFamilyLocation);

            foreach (var existingFamily in existingFamilies)
            {
                var existingFamilyLocation = (existingFamily.Location as LocationPoint).Point;
                var existingFamilyCurves = FamilyInstanceBoundaryLines(5, 5, existingFamilyLocation);

                while (GeoUtils.AreLinesIntersecting(newFamilyCurves, existingFamilyCurves))
                {
                    // Move along the wall direction to avoid overlap
                    XYZ direction = wallCurve.ComputeDerivatives(0, true).BasisX.Normalize();
                    XYZ translationVector = direction * 1.0; // Adjust distance as needed

                    // Move the new family instance
                    ElementTransformUtils.MoveElement(doc, newFamilyInstance.Id, translationVector);

                    // Update the new family location and curves after moving
                    newFamilyLocation = (newFamilyInstance.Location as LocationPoint).Point;
                    newFamilyCurves = FamilyInstanceBoundaryLines(5, 5, newFamilyLocation);
                }
            }
        }
        public static List<Curve> FamilyInstanceBoundaryLines(double familyInsWidth, double familyInsHeight, XYZ familyInsLocPoint, bool IsVerticalWall = false)
        {
            if (IsVerticalWall)
            {
                var height = familyInsHeight;
                var width = familyInsWidth;
                familyInsHeight = width;
                familyInsWidth = height;
            }
            List<Curve> lines = new List<Curve>();
            var instanceLine1 = Line.CreateBound(new XYZ(familyInsLocPoint.X - (familyInsWidth / 2), familyInsLocPoint.Y, 0), new XYZ(familyInsLocPoint.X + (familyInsWidth / 2), familyInsLocPoint.Y, 0)) as Curve;
            var instanceLine2 = Line.CreateBound(new XYZ(familyInsLocPoint.X - (familyInsWidth / 2), familyInsLocPoint.Y + (familyInsHeight / 2), 0), new XYZ(familyInsLocPoint.X + (familyInsWidth / 2), familyInsLocPoint.Y + (familyInsHeight / 2), 0)) as Curve;
            var instanceLine3 = Line.CreateBound(new XYZ(familyInsLocPoint.X - (familyInsWidth / 2), familyInsLocPoint.Y - (familyInsHeight / 2), 0), new XYZ(familyInsLocPoint.X + (familyInsWidth / 2), familyInsLocPoint.Y - (familyInsHeight / 2), 0)) as Curve;

            lines.Add(instanceLine1);
            lines.Add(instanceLine2);
            lines.Add(instanceLine3);

            return lines;
        }
    }
}
