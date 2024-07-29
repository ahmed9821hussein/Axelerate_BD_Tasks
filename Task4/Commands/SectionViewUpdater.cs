using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Task4.Commands
{
    public class SectionViewUpdater : IUpdater
    {
        private readonly ExternalEvent _externalEvent;
        private readonly AdjustSectionBoxDepthHandler _handler;

        private static AddInId appId = new AddInId(new Guid("D5011608-AA4C-4E00-9673-08C70812C0CA"));
        private static UpdaterId updaterId = new UpdaterId(appId, new Guid("C3C95E4D-C169-4510-B01C-9D07EFDE7E86"));

        public SectionViewUpdater(ExternalEvent externalEvent, AdjustSectionBoxDepthHandler handler)
        {
            _externalEvent = externalEvent;
            _handler = handler;
        }

        public void Execute(UpdaterData updateData)
        {
            Document doc = updateData.GetDocument();

            foreach (ElementId eid in updateData.GetAddedElementIds())
            {
                ViewSection viewSection = doc.GetElement(eid) as ViewSection;
                if (viewSection != null)
                {
                    _handler.SetParameters(doc, eid);
                    _externalEvent.Raise();
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "Automatically adjusts the bounding box when a section view is created.";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        public UpdaterId GetUpdaterId()
        {
            return updaterId;
        }

        public string GetUpdaterName()
        {
            return "View Bounding Box Updater";
        }
    }
}
