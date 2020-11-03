#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Binding = Autodesk.Revit.DB.Binding;

#endregion

namespace S4Plugin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public List<Flat> Flats = new List<Flat>();
        public Document document;
        public List<string> Sections = new List<string>();

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            // Access current selection
            ScanProject(doc);

            return Result.Succeeded;
        }


        public bool ScanProject(Document document)
        {
            bool result = true;
            this.document = document;
            this.Flats.Clear();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(document);
            RoomFilter roomFilter = new RoomFilter();
            filteredElementCollector.WherePasses(roomFilter);
            using (IEnumerator<Element> enumerator = filteredElementCollector.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Room room = (Room)enumerator.Current;
                    Parameter parameter = room.get_Parameter(new Guid(Converter.FlatNumber
                        ));
                    Parameter parameter2 = room.get_Parameter(new Guid(Converter.SectionNumber
                        ));
                    if (parameter.HasValue)
                    {
                        string flatNumber = parameter.AsString();
                        string text = parameter2.HasValue ? parameter2.AsString() : "";
                        if (!this.Sections.Contains(text))
                        {
                            this.Sections.Add(text);
                        }
                        Flat flat = this.GetFlatBySectionAndFlatNumber(text, flatNumber);
                        if (flat == null)
                        {
                            Parameter parameter3 = room.get_Parameter(new Guid(Converter.FlatInd));
                            string flatIndex = parameter3.HasValue ? parameter3.AsString() : "";
                            Parameter parameter4 = room.get_Parameter(new Guid(Converter.FlatTypeIndex));
                            FlatType type = (FlatType)parameter4.AsInteger();
                            flat = new Flat(document, flatNumber, text);
                            flat.FlatIndex = flatIndex;
                            flat.Type = type;
                            this.Flats.Add(flat);
                        }
                        flat.AddRoom(room);
                    }
                }
            }
            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Update flats");
                foreach (Flat current in this.Flats)
                {
                    if(current.FlatIndex == String.Empty && current.FlatNumber == String.Empty && current.SectionNumber == String.Empty) continue;                    
                    current.FlatCalculate();
                }
                transaction.Commit();
            }
            //if (filteredElementCollector.Count<Element>() == 0)
            //{
            //    this.FlatsRefreshed(this, new FlatsRefreshedEventArg
            //    {
            //        Document = document,
            //        UpdatedFlats = this.Flats
            //    });
            //}
            
            return result;
        }

        private Flat GetFlatBySectionAndFlatNumber(string sectionNumber, string flatNumber)
        {
            Flat flat = this.Flats.FirstOrDefault((Flat f) => f.SectionNumber == sectionNumber && f.FlatNumber == flatNumber);
            if (flat == null)
            {
            }
            return flat;
        }

    }
}
