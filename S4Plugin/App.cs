#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI;

#endregion

namespace S4Plugin
{
    class App : IExternalApplication
    {

        public List<FlatParameter> FlatParameters;

        public Result OnStartup(UIControlledApplication application)
        {
            FlatParameters = new List<FlatParameter>
            {
                new FlatParameter
                {
                    GUID = Converter.FullArea,
                    Name = "Площадь без коэфициентов",
                    DataType = ParameterType.Area,
                    Visible = false,
                    Categories = new List<BuiltInCategory>
                    {
                        BuiltInCategory.OST_Rooms
                    },
                    ParameterGroup = BuiltInParameterGroup.PG_DATA
                }
            };

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Площадь без коэфициента");
            //RibbonPanel ribbonPanel = application.CreateRibbonPanel("Надстройки", "Площадь без коэфициента");
            string location = Assembly.GetExecutingAssembly().Location;
            PushButtonData pushButtonData = new PushButtonData("Расчитать", "Расчитать", location, typeof(Command).FullName);
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            application.ControlledApplication.DocumentOpened += ControlledApplicationOnDocumentOpened;
            return Result.Succeeded;
        }

        internal void UploadParameters(Document document)
        {
            string sharedParametersFilename = document.Application.SharedParametersFilename;
            string text = this.GenerateSharedParameterFile();
            document.Application.SharedParametersFilename = text;
            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Load parameters");
                try
                {
                    foreach (FlatParameter current in this.FlatParameters)
                    {
                        current.BindParameter(document);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                transaction.Commit();
            }
            document.Application.SharedParametersFilename = sharedParametersFilename;
            document.Application.OpenSharedParameterFile();
            File.Delete(text);
        }

        private string GenerateSharedParameterFile()
        {
            string tempFileName = Path.GetTempFileName();
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            using (StreamWriter streamWriter = new StreamWriter(tempFileName, false, Encoding.Unicode))
            {
                streamWriter.WriteLine("# This is a Revit shared parameter file.");
                streamWriter.WriteLine("# Do not edit manually.");
                streamWriter.WriteLine("*META\tVERSION\tMINVERSION");
                streamWriter.WriteLine("META\t2\t1");
                streamWriter.WriteLine("*GROUP\tID\tNAME");
                streamWriter.WriteLine("GROUP\t1\tBROWNIE");
                streamWriter.WriteLine("*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE");
                foreach (FlatParameter current in this.FlatParameters)
                {
                    streamWriter.WriteLine(current.GetParamString());
                }
            }
            return tempFileName;
        }

        private void ControlledApplicationOnDocumentOpened(object sender, DocumentOpenedEventArgs documentOpenedEventArgs)
        {
            UploadParameters(documentOpenedEventArgs.Document);
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            a.ControlledApplication.DocumentOpened -= ControlledApplicationOnDocumentOpened;

            return Result.Succeeded;
        }
    }
}
