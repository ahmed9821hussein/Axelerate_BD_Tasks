using Autodesk.Revit.DB;
using System.Linq;

namespace Task1.ExtensionMethods
{

    public static class LevelExtensions
    {
        public static Level GetLevel(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Level));
            return collector.Cast<Level>().FirstOrDefault();
        }
    }

}
