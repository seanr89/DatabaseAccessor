using System;
using System.Collections.Generic;

namespace MyGenericContext.Models
{
    /// <summary>
    /// Object model class to handle the data storage and maintenance of class object parameter details
    /// 
    /// </summary>
    public class ObjectPropertyDetails
    {
        /// <summary>
        /// Constructor
        /// Added to handle object list instantiation if null
        /// </summary>
        public ObjectPropertyDetails()
        {
            if(this.ClassProperties == null)
                ClassProperties = new List<ObjectPropertyDetails>();
        }

        /// <summary>
        /// GET:SET: the name of the class object that the property is apart of
        /// </summary>
        /// <returns></returns>
        public string ObjectClassName { get; set; }

        /// <summary>
        /// GET:SET: The name of the property object
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// GET:SET: The Type of the property
        /// </summary>
        /// <returns></returns>
        public Type PropertyType { get; set; }

        /// <summary>
        /// GET:SET: if the object is a class object (but not a string)
        /// This object stores all the properties of the current object
        /// </summary>
        /// <returns></returns>
        public List<ObjectPropertyDetails> ClassProperties { get; set; }

        /// <summary>
        /// GET:SET the parent object details of the current object property
        /// </summary>
        /// <returns></returns>
        //public ObjectPropertyDetails ParentObject { get; set; }

        #region Optional Properties

        public bool IsClass { get; set; }

        public bool IsEnumerable {get; set;}

        #endregion 

    }
}