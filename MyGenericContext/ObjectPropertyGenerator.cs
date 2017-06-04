using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MyGenericContext.Models;
using MyGenericContext.Utilities;

namespace MyGenericContext
{
    public class ObjectPropertyGenerator
    {
        private readonly ILogger _Logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectPropertyGenerator()
        {
            _Logger = ApplicationLoggerProvider.CreateLogger<ObjectPropertyGenerator>();
        }   

        /// <summary>
        /// Static method to handle the recursive searching through an object to find and log all object properties
        /// </summary>
        /// <param name="obj">The object to search</param>
        /// <param name="propertyList">The current list of properties that have been generated and stored! default is null</param>
        /// <param name="parentObj">The parent object of the current obj (by default is null)</param>
        /// <returns>A list of object parameters</returns>
        public List<ObjectPropertyDetails> ReadObjectAndParseProperties(object obj, List<ObjectPropertyDetails> propertyList = null, 
                                                                                ObjectPropertyDetails parentObj = null)
        {
            _Logger.LogInformation(LoggingEvents.GENERIC_MESSAGE, $"Method: {UtilityMethods.GetCallerMemberName()} for object {obj.GetType().ToString()}");

            //Try and load a ModelList with the propertyList parameter
            List<ObjectPropertyDetails> ModelList = propertyList;

            //If the provided object is null - return the ModelList of objectproperties
            if(obj == null) return ModelList;

            //Check the list and if null - initialise else do nothing
            if(ModelList == null) ModelList = new List<ObjectPropertyDetails>();

            //Create default object for the current obj and intialise
            ObjectPropertyDetails Model = new ObjectPropertyDetails();
            Type objType = obj.GetType();

            Debug.WriteLine($"current object is {obj.GetType().ToString()}");
            //Check if the current object is a class
            if(IsCurrentClassObjectTypeAClass(obj.GetType().GetTypeInfo()))
            {
                Debug.WriteLine($"class object found: {obj.GetType().GetTypeInfo().ToString()}");
                Model.IsClass = true;
                Model.PropertyType = objType;
                //Set the name as the type of the object
                Model.Name = obj.GetType().ToString();

                //First add the item to the ModelList for safe keeping!!!
                ModelList.Add(Model);

                //Next get all properties of the current object
                PropertyInfo[] Properties = objType.GetProperties();
                Debug.WriteLine($"number of properties found {Properties.Length}");

                //Then loop through all the current properties
                foreach(PropertyInfo property in Properties)
                {
                    Debug.WriteLine($"type v1 is {property.PropertyType.ToString()}");
                    Debug.WriteLine($"current property found: {property.GetType().GetTypeInfo().ToString()} with name {property.Name}");
                    //now create object to check if the current property is a collection (IEnumerable)
                    Type propertyType = property.GetType();
                    var elems = propertyType as IEnumerable;
                    //If the item is not null
                    if(elems != null)
                    {
                        Debug.WriteLine($"IEnumerable object found: {propertyType.ToString()}");    
                        //Initialise a new object to handle storing object data for the list!
                        ObjectPropertyDetails CollectionObject = new ObjectPropertyDetails();
                        CollectionObject.IsClass = false;
                        CollectionObject.IsEnumerable = true;
                        CollectionObject.PropertyType = propertyType;
                        //Set the name as the type of the object
                        CollectionObject.Name = obj.GetType().ToString();
                        CollectionObject.ParentObject = Model;

                        //Add the List object to the current object model
                        Model.ClassProperties.Add(CollectionObject);

                        //Note - we need to loop through this object here!!
                        //The item is collection and therefore we need to split this up!
                        foreach(var item in elems)
                        {
                            //Then recursively call the read object again
                            ReadObjectAndParseProperties(CollectionObject, ModelList, Model);
                        }
                    }
                    else //The object is not a collection
                    {
                        Debug.WriteLine($"property Not a IEnumerable");
                        //we now need to check again to confirm that this is not also an object
                        if(IsCurrentClassObjectTypeAClass(property.PropertyType.GetTypeInfo()) == false)
                        {
                            Debug.WriteLine($"property is not a class");
                            ObjectPropertyDetails GenericObject = new ObjectPropertyDetails();
                            GenericObject.IsClass = false;
                            GenericObject.IsEnumerable = false;
                            GenericObject.Name = property.Name;
                            GenericObject.PropertyType = property.GetType();
                            GenericObject.ParentObject = Model;

                            Model.ClassProperties.Add(GenericObject);
                        }
                        else //if this is then we need to parse again!!
                        {
                            Debug.WriteLine($"property {property.GetType().GetTypeInfo().ToString()} is marked a class");
                            ReadObjectAndParseProperties(property, ModelList, Model);
                        }
                    }
                }

            }
            else //The current object is not somehow a class!!!
            {
                //warning may beed to add in a loop check here
                if(IsCurrentObjectTypeAnIEnumerable(obj))
                {
                    Type propertyType = obj.GetType();
                    Model.IsClass = false;
                    Model.IsEnumerable = true;
                    Model.PropertyType = objType;
                    //Set the name as the type of the object
                    Model.Name = obj.GetType().ToString();
                    
                    //First add the item to the parentobj for safe keeping!!!
                    parentObj.ClassProperties.Add(Model);

                    //Initialse the elem object for loop through the list
                    var elems = propertyType as IEnumerable;
                    foreach(var item in elems)
                    {
                            //Then recursively call the read object again
                            ReadObjectAndParseProperties(item, ModelList, Model);
                    }
                }
                else
                {
                    ObjectPropertyDetails GenericObject = new ObjectPropertyDetails();
                    GenericObject.IsClass = false;
                    GenericObject.IsEnumerable = false;
                    GenericObject.Name = objType.Name;
                    GenericObject.PropertyType = objType.GetType();
                    GenericObject.ParentObject = parentObj;
                    //Add the item to the included parameter parent object
                    parentObj.ClassProperties.Add(GenericObject);
                }
            }


            //Long term final event is to return a List
            return ModelList;
        }

        /// <summary>
        /// Method to return if a provided type references a class!
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsCurrentClassObjectTypeAClass(TypeInfo type)
        {
            bool result = false;

            if(type.IsClass && type.ToString() != "System.String")
                result = true;

            return result;
        }

        /// <summary>
        /// Operation to check if the supplied object is a IEnumerable type
        /// </summary>
        /// <param name="obj">The object to be checked</param>
        /// <returns>A boolean variable: true if obj is an IEnumerable</returns>
        bool IsCurrentObjectTypeAnIEnumerable(object obj)
        {
            bool result = false;

            Type propertyType = obj.GetType();
            var elems = propertyType as IEnumerable;
            //If the item is not null
            if(elems != null)
            {
                result = true;
            }

            return result;
        }
    }
}