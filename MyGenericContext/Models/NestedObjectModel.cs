using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGenericContext.Models
{
    public class NestedObjectModel
    {
        public NestedObjectModel()
        {
            ID = 1;
            Name = "NestedObjectModel1";
            MyObjects = new List<SecondaryObject>();
            MyObjects.Add(new SecondaryObject());
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public List<SecondaryObject> MyObjects { get; set; }
    }

    public class SecondaryObject
    {
        public SecondaryObject()
        {
            ID = 2;
            Name = "SecondaryObject";
            Enabled = false;
            Third = new ThirdObject();
        }
        public int ID { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public ThirdObject Third { get; set; }
    }

    public class ThirdObject
    {
        public int ID { get; set; }
    }
}