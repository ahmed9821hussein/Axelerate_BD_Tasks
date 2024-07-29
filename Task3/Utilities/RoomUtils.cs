using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Task3.Utilities
{
    public static class RoomUtils
    {
        public static Room PickRoom(UIDocument uidoc)
        {
            var sel = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new RoomSelectionFilter(), "Select a room");
            return uidoc.Document.GetElement(sel.ElementId) as Room;
        }

        public static List<FamilyInstance> GetDoorsInRoom(Room room, Document doc)
        {
            List<FamilyInstance> doors = new List<FamilyInstance>();
            var boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            foreach (var segmentList in boundarySegments)
            {
                foreach (var segment in segmentList)
                {
                    var wall = doc.GetElement(segment.ElementId) as Wall;
                    if (wall != null)
                    {
                        var doorInstances = new FilteredElementCollector(doc, wall.Id)
                            .OfCategory(BuiltInCategory.OST_Doors)
                            .WhereElementIsNotElementType()
                            .Cast<FamilyInstance>()
                            .ToList();
                        doors.AddRange(doorInstances);
                    }
                }
            }
            return doors;
        }
    }

    public class RoomSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Room;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
