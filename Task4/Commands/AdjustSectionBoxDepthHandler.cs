using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Task4.Utilities;

namespace Task4.Commands
{
    public class AdjustSectionBoxDepthHandler : IExternalEventHandler
    {
        private Document _doc;
        private ElementId _viewId;
        private static List<ElementId> createdElementIds = new List<ElementId>();

        public void SetParameters(Document doc, ElementId viewId)
        {
            _doc = doc;
            _viewId = viewId;
        }

        public void Execute(UIApplication app)
        {
            try
            {
                BoundingBoxXYZ cropBox = null;
                ElementId viewTypeId = null;
                string originalViewName = null;
                Transform originalTransform = null;
                XYZ originalMin = null;
                XYZ originalMax = null;

                using (TransactionGroup txGroup = new TransactionGroup(_doc, "Adjust Section Box Depth"))
                {
                    txGroup.Start();

                    using (Transaction txDelete = new Transaction(_doc, "Delete Original Section View"))
                    {
                        txDelete.Start();

                        ViewSection originalView = _doc.GetElement(_viewId) as ViewSection;
                        if (originalView != null && originalView.ViewType == ViewType.Section)
                        {
                            if (createdElementIds.Contains(originalView.Id))
                                return;

                            cropBox = originalView.CropBox;
                            viewTypeId = originalView.GetTypeId();
                            originalViewName = originalView.Name;
                            originalTransform = cropBox.Transform;
                            originalMin = cropBox.Min;
                            originalMax = cropBox.Max;

                            _doc.Delete(originalView.Id);
                            txDelete.Commit();
                        }
                        else
                        {
                            TaskDialog.Show("Error", "The active view is not a section view or is null.");
                            txDelete.RollBack();
                            return;
                        }
                    }

                    ViewSection newSectionView = null;
                    using (Transaction txCreate = new Transaction(_doc, "Create Copy with Adjusted BoundingBox"))
                    {
                        txCreate.Start();

                        if (cropBox != null)
                        {
                            ViewPlan floorPlanView = _doc.ActiveView as ViewPlan;
                            Level level = SectionBoxUtils.GetLevel(_doc, floorPlanView);
                            double levelElevation = level.Elevation;

                            double offset = 10.0; // 10 feet above and below
                            BoundingBoxXYZ newBox = new BoundingBoxXYZ
                            {
                                Min = new XYZ(cropBox.Min.X, cropBox.Min.Y, levelElevation - offset),
                                Max = new XYZ(cropBox.Max.X, cropBox.Max.Y, levelElevation + offset)
                            };

                            newBox.Transform = originalTransform;
                            newSectionView = ViewSection.CreateSection(_doc, viewTypeId, newBox);
                            newSectionView.Name = originalViewName;

                            newSectionView.CropBox = newBox;
                            newSectionView.CropBoxActive = true;
                            newSectionView.CropBoxVisible = true;

                            createdElementIds.Add(newSectionView.Id);
                            txCreate.Commit();
                        }
                    }

                    if (newSectionView != null)
                    {
                        using (Transaction txReference = new Transaction(_doc, "Create Reference Section"))
                        {
                            txReference.Start();
                            ViewPlan floorPlanView = _doc.ActiveView as ViewPlan;

                            Transform referenceTransform = cropBox.Transform;
                            XYZ originalHeadPoint = referenceTransform.OfPoint(new XYZ(originalMin.X, originalMin.Y, 0));
                            XYZ originalTailPoint = referenceTransform.OfPoint(new XYZ(originalMax.X, originalMax.Y, 0));

                            ViewSection.CreateReferenceSection(_doc, floorPlanView.Id, newSectionView.Id, originalHeadPoint, originalTailPoint);
                            txReference.Commit();
                        }
                    }

                    txGroup.Assimilate();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Failed to complete the operation: {ex.Message}");
            }
        }

        public string GetName()
        {
            return "Adjust Section Box Depth Handler";
        }
    }
}
