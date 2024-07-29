using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Task4.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class App : IExternalApplication
    {
        public static ExternalEvent _externalEvent;
        public static AdjustSectionBoxDepthHandler _handler;
        private static SectionViewUpdater _updater;

        public Result OnStartup(UIControlledApplication application)
        {
            _handler = new AdjustSectionBoxDepthHandler();
            _externalEvent = ExternalEvent.Create(_handler);

            _updater = new SectionViewUpdater(_externalEvent, _handler);
            UpdaterRegistry.RegisterUpdater(_updater, true);

            ElementClassFilter viewSectionFilter = new ElementClassFilter(typeof(ViewSection));
            UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), viewSectionFilter, Element.GetChangeTypeElementAddition());

            TaskDialog.Show("Startup Task4", "Ready to adjust section box depths based on the activated floor plan.");
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            UpdaterRegistry.UnregisterUpdater(_updater.GetUpdaterId());
            return Result.Succeeded;
        }
    }
}
