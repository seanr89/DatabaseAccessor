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
        int count = 0;

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
            count++;
            Debug.WriteLine($"Method: {UtilityMethods.GetCallerMemberName()} for object {obj.GetType().ToString()} with count {count}");

            //Try and load a ModelList with the propertyList parameter
            List<ObjectPropertyDetails> ModelList = propertyList;

            //If the provided object is null - return the ModelList of objectproperties
            if(obj == null) 
            {
                Debug.WriteLine($"obj is null");
                return ModelList;
            }

            //Check the list and if null - initialise else do nothing
            if(ModelList == null) ModelList = new List<ObjectPropertyDetails>();

            //a default object to be used (i think)
            ObjectPropertyDetails Model = null;

            // check if parentObj is not null (this is a objectpropertydetails)
            if(parentObj != null)
            {
                Model = parentObj;
            }
            //Create default object for the current obj and intialise
            else
            {
                Model = new ObjectPropertyDetails();   
            }
            //get the object type
            Type objType = obj.GetType();

            Debug.WriteLine($"current object is {obj.ToString()}");

            //Check if the current object is a class
            if(IsCurrentClassObjectTypeAClass(obj.GetType()))
            {
                ObjectPropertyDetails classDetails = null;
                //if there is a parent object present
                if(parentObj != null)
                {
                    //we need to initialse a new class to store data
                    classDetails = new ObjectPropertyDetails();
                    classDetails.IsClass = true;
                    classDetails.PropertyType = objType;
                    //Set the name as the type of the object (unsure if this is correct!!)
                    classDetails.Name = obj.GetType().ToString();

                    Model.ClassProperties.Add(classDetails);
                }
                else
                {
                    //set the class details to be the current Model - which is a new object parameter details
                    classDetails = Model;

                    ModelList.Add(classDetails);
                }

                //ModelList.Add(classDetails);
                //Debug.WriteLine($"class object found: {obj.GetType().GetTypeInfo().ToString()}");

                //Next get all properties of the current object
                PropertyInfo[] Properties = objType.GetProperties();
                Debug.WriteLine($"number of properties found {Properties.Length}");

                //Then loop through all the current properties
                foreach(PropertyInfo property in Properties)
                {
                    //Debug.WriteLine($"type v1 is {property.PropertyType.ToString()}");
                    // if(parentObj != null)
                    // {
                    //     Debug.WriteLine($"current property found: {property.PropertyType.ToString()} with name {property.Name} for parent: {parentObj.GetType()}");
                    // }
                    // else
                    // {
                    //     Debug.WriteLine($"current property found: {property.PropertyType.ToString()} with name {property.Name} for parent: {obj.GetType()}");
                    // }

                    //now create object to check if the current property is a collection (IEnumerable)
                    var props = property.GetValue(obj, null);
                    var elems = props as IEnumerable;
                    //If the item is not null
                    if(elems != null && IsPropertyAString(property) == false)
                    {
                        Debug.WriteLine($"IEnumerable object found: {property.PropertyType.ToString()}");    
                        //Initialise a new object to handle storing object data for the list!
                        ObjectPropertyDetails CollectionObject = new ObjectPropertyDetails();
                        CollectionObject.IsClass = false;
                        CollectionObject.IsEnumerable = true;
                        CollectionObject.PropertyType = property.PropertyType;
                        //Set the name as the type of the object
                        CollectionObject.Name = obj.GetType().ToString();
                        //CollectionObject.ParentObject = Model;

                        //Add the List object to the current object model
                        classDetails.ClassProperties.Add(CollectionObject);

                        //Note - we need to loop through this object here!!
                        //The item is collection and therefore we need to split this up!

                        foreach(var item in elems)
                        {
                             //Then recursively call the read object again
                             Debug.WriteLine($"read properties of list object item {item.GetType().ToString()}");
                             ReadObjectAndParseProperties(item, ModelList, CollectionObject);
                        }
                    }
                    else //The object is not a collection
                    {
                        //Debug.WriteLine($"property is not an IEnumerable");
                        //we now need to check again to confirm that this is not also an object
                        if(IsCurrentClassObjectTypeAClass(property.PropertyType) == false)
                        {
                            Debug.WriteLine($"property is not a class, with type: {property.PropertyType} and name: {property.Name}");
                            ObjectPropertyDetails GenericObject = new ObjectPropertyDetails();
                            GenericObject.IsClass = false;
                            GenericObject.IsEnumerable = false;
                            GenericObject.Name = property.Name;
                            GenericObject.PropertyType = property.PropertyType;

                            classDetails.ClassProperties.Add(GenericObject);
                        }
                        else //if this is then we need to parse again!!
                        {
                            Debug.WriteLine($"property {property.PropertyType} is marked as a class into object {classDetails.PropertyType}");
                            var itemClass = UtilityMethods.CreateObjectFromPropertyType(property.PropertyType);
                            ReadObjectAndParseProperties(itemClass, ModelList, classDetails);
                        }
                    }
                }

            }
            else //The current object is not somehow a class!!!
            {
                Debug.WriteLine($"Current obj is not a class object");

                // //warning may beed to add in a loop check here
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
                    //GenericObject.ParentObject = parentObj;
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
        bool IsCurrentClassObjectTypeAClass(Type type)
        {
            //_Logger.LogInformation(LoggingEvents.GENERIC_MESSAGE, $"method {UtilityMethods.GetCallerMemberName()} for type {type.ToString()}");
            bool result = false;

            if(type.GetTypeInfo().IsClass && type.ToString() != "System.String")
                result = true;

            return result;
        }

        /// <summary>
        /// TODO - define
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        bool IsPropertyAString(PropertyInfo prop)
        {
            bool result = false;

            if(prop.PropertyType.ToString() == "System.String")
            {
                result = true;
            }
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