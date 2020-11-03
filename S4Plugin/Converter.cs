using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace S4Plugin
{
    public static class Converter
    {
        public static string FlatInd = "6C73AEA5-8F23-469C-80E4-CBC38C79849A";
        public static string FlatNumber = "151CF92B-D6C9-4252-AC3D-88F2A80CB54A";
        public static string GeneralArea = "9ACB3C71-2254-480B-B6C3-428255E29D65";

        public static string AreaFactor = "3750C00A-DAFC-4692-A5D5-DC83139AFD0B";

        public static string FlatType = "5285CA67-8C16-42D5-ABCD-89E4EF88988A";

        public static string RoomTypeIndex = "98DEBC67-5F14-4B00-BCF6-FF927BCF0DC5";

        public static string FlatCount = "0921A5EF-D8A0-4920-BE46-E7756F647A79";

        public static string FlatTypeIndex = "5EE12172-7C05-412D-97EB-BDC735211838";

        public static string AreaK = "72CCD8F8-ACC3-40D3-9522-C90ABC62C7F0";

        public static string RoomType = "F500439B-5995-4F4A-812D-C92A7FA81209";

        public static string ReducedArea = "27B4EF63-EB00-47FB-BE5F-D0D1B8F8BC0A";

        public static string SectionNumber = "CD562900-14CC-4102-9A51-31284095402C";

        public static string RoomCount = "4A621E6D-EC7C-4407-AF31-BA082E3DA5B9";

        public static string FlatRoomNumber = "E0ED8336-62CD-41D5-ACB8-65184DAA792F";

        public static string LivingSpace = "47646CAA-2FC5-4FB3-A806-95E2254A2C64";

        public static string FullArea = "6C7321A5-8D23-469C-81E4-CBC11C798411";
        public static string ParamToString(this ParameterType type)
        {
            return type.ToString().ToUpper();
        }

        public static double Ft2ToM2(this double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, DisplayUnitType.DUT_SQUARE_METERS);
        }

        public static double M2ToFt2(this double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_SQUARE_METERS);
        }

        public static double FtToM(this double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, 0);
        }

        public static double MToFt(this double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, 0);
        }
    }

    public static class RoomHelpers
    {
        //public static double GetLivingArea(this Room room)
        //{
        //    double result = 0.0;
        //    RoomType roomType = room.GetRoomType();
        //    if (roomType == RoomType.Living)
        //    {
        //        result = room.Area;
        //    }
        //    return result;
        //}

        //public static double GetGeneralArea(this Room room)
        //{
        //    double result = 0.0;
        //    RoomType roomType = room.GetRoomType();
        //    if (roomType != RoomType.Balcony && roomType != RoomType.Loggia)
        //    {
        //        result = room.GetReducedArea();
        //    }
        //    return result;
        //}

        //public static double GetReducedArea(this Room room)
        //{
        //    double value = room.Area * room.GetRoomAreaFactor();
        //    return value;
        //}

        public static double GetS4Area(this Room room)
        {
            double value = room.Area;
            return value;
        }

        public static double GetRoomAreaFactor(this Room room)
        {
            return room.get_Parameter(new Guid(Converter.AreaFactor)).AsDouble();
        }

        public static RoomType GetRoomType(this Room room)
        {
            return (RoomType) room.get_Parameter(new Guid(Converter.RoomTypeIndex)).AsInteger();
        }
    }
}
