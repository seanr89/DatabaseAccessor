using System;
using System.Collections.Generic;
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
            //Check the list and if null - initialise else do nothing
            if(ModelList == null) ModelList = new List<ObjectPropertyDetails>();

            //Create default object for the current obj and intialise
            ObjectPropertyDetails Model = new ObjectPropertyDetails();
            Type objType = obj.GetType();

            

            //If the provided object is null - return null?
            if(obj == null) return ModelList;


            //Long term final event is to return a List
            return ModelList;
        }

        bool isCurrentClassObjectTypeAClass(PropertyInfo type)
        {
            bool result = false;

            if(type.PropertyType.GetTypeInfo().IsClass && type.PropertyType.ToString() != "System.String")
                result = true;

            return result;
        }
    }
}