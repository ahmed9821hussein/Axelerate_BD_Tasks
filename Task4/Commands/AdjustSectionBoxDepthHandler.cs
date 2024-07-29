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
                using (Transaction tx = new Transaction(_doc, "Adjust Section Box Depth"))
                {
                    tx.Start();

                    ViewSection originalView = _doc.GetElement(_viewId) as ViewSection;
                    if (originalView != null && originalView.ViewType == ViewType.Section)
                    {
                        if (createdElementIds.Contains(originalView.Id))
                            return;

                        BoundingBoxXYZ cropBox = originalView.CropBox;
                        Level level = SectionBoxUtils.GetLevel(_doc, _doc.ActiveView as ViewPlan);
                        double levelElevation = level.Elevation;

                        double offset = 10.0; // 10 feet above and below
                        BoundingBoxXYZ newBox = new BoundingBoxXYZ
                        {
                            Min = new XYZ(cropBox.Min.X, cropBox.Min.Y, levelElevation - offset),
                            Max = new XYZ(cropBox.Max.X, cropBox.Max.Y, levelElevation + offset),
                            Transform = cropBox.Transform
                        };

                        originalView.CropBox = newBox;
                        originalView.CropBoxActive = true;
                        originalView.CropBoxVisible = true;

                        createdElementIds.Add(originalView.Id);
                    }
                    else
                    {
                        TaskDialog.Show("Error", "The active view is not a section view or is null.");
                        tx.RollBack();
                        return;
                    }

                    tx.Commit();
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
