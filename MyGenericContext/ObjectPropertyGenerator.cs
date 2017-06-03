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

        }   

        /// <summary>
        /// Static method to handle the recursive searching through an object to find and log all object properties
        /// </summary>
        /// <param name="obj">The object to search</param>
        /// <param name="propertyList">The current list of properties that have been generated and stored! default is null</param>
        /// <param name="parentObj">The parent object of the current obj (by default is null)</param>
        /// <returns>A list of object parameters</returns>
        public List<ObjectPropertyDetails> ReadObjectAndParseProperties(object obj, List<ObjectPropertyDetails> propertyList = null, 
                                                                                object parentObj = null)
        {
            _Logger.LogInformation(LoggingEvents.GENERIC_MESSAGE, $"Method: {UtilityMethods.GetCallerMemberName()} for object {obj.GetType().ToString()}");
            throw new NotImplementedException();

            //Try and load a ModelList with the propertyList parameter
            List<ObjectPropertyDetails> ModelList = propertyList;

            //If the provided object is null - return the ModelList of objectproperties
            if(obj == null) return ModelList;

            //Check the list and if null - initialise else do nothing
            if(ModelList == null) ModelList = new List<ObjectPropertyDetails>();

            //Create default object for the current obj and intialise
            ObjectPropertyDetails Model = new ObjectPropertyDetails();
            Type objType = obj.GetType();
            //Check if the current object is a class
            if(IsCurrentClassObjectTypeAClass(obj.GetType().GetTypeInfo()))
            {
                Debug.WriteLine($"object found: {obj.GetType()}");
                Model.IsClass = true;
                Model.PropertyType = objType;
                //Set the name as the type of the object
                Model.Name = obj.GetType().ToString();

                //Ok so now what??

                //First add the item to the ModelList for safe keeping!!!
                ModelList.Add(Model);

                //Next get all properties of the current object
                PropertyInfo[] Properties = objType.GetProperties();
                ObjectPropertyDetails propertyDetails = null;
                //Then loop through all the current properties
                foreach(PropertyInfo property in Properties)
                {
                    //now create object to check if the current property is a collection (IEnumerable)
                    Type propertyType = property.GetType();
                    var elems = propertyType as IEnumerable;
                    //If the item is not null
                    if(elems != null)
                    {
                        //The item is collection and therefore we need to split this up!
                        //and maybe continue to loop through this again!
                        //ReadEnumerableContent(property);
                        ObjectPropertyDetails CollectionObject = new ObjectPropertyDetails();
                        CollectionObject.IsClass = false;
                        CollectionObject.IsEnumerable = true;
                        CollectionObject.PropertyType = propertyType;
                        //Set the name as the type of the object
                        CollectionObject.Name = obj.GetType().ToString();

                        //Add the List object to the current object model
                        Model.ClassProperties.Add(CollectionObject);

                        //Then recursively call the read object again
                        ReadObjectAndParseProperties(CollectionObject, ModelList, Model);
                    }
                    else //The object is not a collection
                    {
                        //we now need to check again to confirm that this is not also an object
                        if(IsCurrentClassObjectTypeAClass(property.GetType().GetTypeInfo()) == false)
                        {
                            ObjectPropertyDetails GenericObject = new ObjectPropertyDetails();
                            GenericObject.IsClass = false;
                            GenericObject.IsEnumerable = false;
                            GenericObject.Name = property.Name;
                            GenericObject.PropertyType = property.GetType();

                            Model.ClassProperties.Add(GenericObject);
                        }
                        else //if this is then we need to parse again!!
                        {
                            ReadObjectAndParseProperties(property, ModelList, Model);
                        }
                    }
                }

            }
            else //The current object is not somehow a class!!!
            {
                //Not sure what needs to be done, if anything!!
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
        /// Method to read the content for an enumerable object
        /// </summary>
        /// <param name="obj"></param>
        void ReadEnumerableContent(object obj)
        {
            throw new NotImplementedException();
        }
    }
}