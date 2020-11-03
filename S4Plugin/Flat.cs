using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace S4Plugin
{
    

    public enum FlatType
    {
        Standart,
        Studio,
        Euro
    }

    public enum RoomType
    {
        Indefined,
        Living,
        Public,
        NotLiving,
        Loggia,
        Balcony
    }

    public class Flat
    {
        private List<Room> rooms = new List<Room>();

        private Document document;

        private string flatNumber;

        private string sectionNumber;

        private FlatType flatType;

        private string flatIndex;

        public string FlatNumber
        {
            get
            {
                return this.flatNumber;
            }
            set
            {
                if (this.flatNumber != value)
                {
                    this.flatNumber = value;
                    this.NeedUpdateSpecification = false;
                    this.Modify = true;
                }
            }
        }

        public string SectionNumber
        {
            get
            {
                return this.sectionNumber;
            }
            set
            {
                if (this.sectionNumber != value)
                {
                    this.sectionNumber = value;
                    this.NeedUpdateSpecification = false;
                    this.Modify = true;
                }
            }
        }

        public FlatType Type
        {
            get
            {
                return this.flatType;
            }
            set
            {
                this.flatType = value;
                this.Modify = true;
            }
        }

        public string FlatIndex
        {
            get
            {
                return this.flatIndex;
            }
            set
            {
                this.flatIndex = value;
                this.Modify = true;
            }
        }

        public bool NeedUpdateSpecification
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get
            {
                return this.rooms.Count<Room>() == 0;
            }
        }

        public int LivingRoomCount
        {
            get;
            private set;
        }

        public double LivingArea
        {
            get;
            private set;
        }

        public double Full_Area
        {
            get;
            private set;
        }

        public double GeneralArea
        {
            get;
            private set;
        }

        public double ReducedArea
        {
            get;
            private set;
        }

        public double LivingArea_m2
        {
            get { return LivingArea.Ft2ToM2();}
        }

        public double GeneralArea_m2
        {
            get
            {
                return this.GeneralArea.Ft2ToM2();
            }
        }

        public double ReducedArea_m2
        {
            get
            {
                return this.ReducedArea.Ft2ToM2();
            }
        }

        public Guid GUID
        {
            get;
            private set;
        }

        public bool Modify
        {
            get;
            private set;
        }

        public Flat(Document document, string flatNumber, string sectionNumber)
        {
            this.document = document;
            this.flatNumber = flatNumber;
            this.sectionNumber = sectionNumber;
            this.GUID = Guid.NewGuid();
            this.Modify = true;
        }

        public void SetAsModifing()
        {
            this.Modify = true;
            this.NeedUpdateSpecification = true;
        }

        public List<Room> GetAllRooms()
        {
            return this.rooms;
        }

        public bool AddRoom(Room room)
        {
            bool result = true;
            if (room.Area == 0.0)
            {
                return false;
            }
            if (!(from r in this.rooms
                select r.Id).Contains(room.Id))
            {
                this.rooms.Add(room);
            }
            else
            {
            }
            this.rooms = (from r in this.rooms
                orderby r.Area descending
                select r).ToList<Room>();
            this.Modify = true;
            this.NeedUpdateSpecification = true;
            return result;
        }

        public void AddRooms(List<Room> rooms)
        {
            foreach (Room current in rooms)
            {
                this.AddRoom(current);
            }
            this.Modify = true;
        }

        public bool RemoveRoom(Room room)
        {
            return this.RemoveRoom(room.Id);
        }

        public bool MoveToSection(string Section)
        {
            bool result = true;
            this.Modify = true;
            return result;
        }

        public void FlatCalculate()
        {
            this.Full_Area = 0.0;

            foreach (Room current in this.rooms)
            {
                this.Full_Area += current.Area;
            }
            foreach (Room current2 in this.rooms)
            {
                current2.get_Parameter(new Guid(Converter.FullArea)).Set(this.Full_Area);
            }
            this.Modify = false;
        }

        //public void UpdateGeneralProperty()
        //{
        //    foreach (Room current in this.rooms)
        //    {
        //        current.get_Parameter(new Guid(ParamGUIDS.FlatNumber)).Set(this.flatNumber);
        //        current.get_Parameter(new Guid(ParamGUIDS.FlatTypeIndex)).Set((int)this.flatType);
        //        string text = this.flatType.ToLocChar();
        //        current.get_Parameter(new Guid(ParamGUIDS.FlatType)).Set(text);
        //        current.get_Parameter(new Guid(ParamGUIDS.FlatInd)).Set(this.flatIndex);
        //        current.get_Parameter(new Guid(ParamGUIDS.SectionNumber)).Set(this.sectionNumber);
        //    }
        //    this.Modify = false;
        //}

        public bool RoomExist(Room room)
        {
            int arg_0D_0 = room.Id.IntegerValue;
            return (from r in this.rooms
                select r.Id.IntegerValue).Contains(room.Id.IntegerValue);
        }

        public bool RoomExist(ElementId Id)
        {
            return (from r in this.rooms
                select r.Id.IntegerValue).Contains(Id.IntegerValue);
        }

        public bool RemoveRoom(ElementId Id)
        {
            bool result = true;
            this.rooms.Remove(this.rooms.FirstOrDefault((Room f) => f.Id.IntegerValue == Id.IntegerValue));
            this.Modify = true;
            this.NeedUpdateSpecification = true;
            return result;
        }

        public void ClearFromMisticRooms()
        {
            int num = 0;
            while (this.rooms.Count > num)
            {
                Room room = this.rooms[num];
                if (room.Location == null)
                {
                    this.rooms.Remove(room);
                    this.Modify = true;
                    this.NeedUpdateSpecification = true;
                    num--;
                }
                num++;
            }
        }

        public void SpecificationUpdated()
        {
            this.NeedUpdateSpecification = false;
        }

        internal ElementId[] GetRoomElemetIDs()
        {
            throw new NotImplementedException();
        }
    }
}
