
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyGenericContext.Models;
using Newtonsoft.Json;

namespace MyGenericContext.Utilities
{
    public static class UtilityMethods
    {
        private static readonly ILogger _Logger;

        /// <summary>
        /// Static constructor method
        /// Includes method to generate ApplicationLogger object
        /// </summary>
        static UtilityMethods()
        {
            _Logger = ApplicationLoggerProvider.CreateLogger(typeof(UtilityMethods).ToString());
        }

        /// <summary>
        /// Static operation to get the name of the method calling this method
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The method calling name</returns>
        public static string GetCallerMemberName([CallerMemberName]string name = "")
        {
            return name;
        }

        /// <summary>
        /// /// Static operation to check if the provided Datareader object has the provided column name
        /// </summary>
        /// <param name="r">the current datareader object to be searched</param>
        /// <param name="columnName">the column name to search for</param>
        /// <returns>A boolean variable</returns>
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            if(string.IsNullOrWhiteSpace(columnName))
            {
                return false;
            }

            try
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr.GetName(i).Equals(columnName))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (NullReferenceException)
            {
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, $"Method: {UtilityMethods.GetCallerMemberName()} for column {columnName}");
                return false;
            }
        }

        /// <summary>
        /// Static operation to create a string name for datastore search/query parameter
        /// </summary>
        /// <param name="classObject">the object to be searched</param>
        /// <param name="parameterName">The parameter name to be combined with the class object name</param>
        /// <returns>A string combined parameter to query for the '_'</returns>
        public static string CreateDataReaderNameSearchParameter(this object classObject, string parameterName)
        {
            string result = "";
            result = string.Format($"{classObject.GetType().Name}_{parameterName}");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType">cannot be null or white space</param>
        /// <param name="parameterName">cannot be null or white space</param>
        /// <returns></returns>
        public static string CreateDataReaderNameSearchParameterForObjectType(string objectType, string parameterName = "ID")
        {
            string result = "";

            //Do a null / white space check for either parameters provided
            if (string.IsNullOrWhiteSpace(objectType) || string.IsNullOrWhiteSpace(parameterName))
            {
                //we could throw just a standard exception here!!
                //throw new EmptyStringException();
                return result;
            }

            try
            {
                objectType = objectType.Split('.').Last();
            }
            catch (ArgumentException e)
            {
                //Don't care about this as we just want to user the
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, $"Method: {UtilityMethods.GetCallerMemberName()} with exception: {e.Message}");
            }
            finally
            {
                result = string.Format($"{objectType}_{parameterName}");
            }
            return result;
        }



        /// <summary>
        /// Return a provide date formatted to a string for usage with SQL query parameters
        /// </summary>
        /// <param name="date">Date in question to format</param>
        /// <returns>A date string formatted to yyyy-MM-dd HH:mm:ss</returns>
        public static string GenerateSQLFormattedDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Handle parsing of object properties into dictionary of property name and value
        /// </summary>
        /// <param name="obj">The object to query the properties for</param>
        /// <returns>A dictionary of property name keys and object values</returns>
        public static Dictionary<string, object> ParseObjectPropertiesToDictionary(object obj)
        {
            Dictionary<string, object> ModelDictionary = null;
            if (obj != null)
            {
                ModelDictionary = new Dictionary<string, object>();

                try
                {
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        ModelDictionary.Add(prop.Name, prop.GetValue(obj, null));
                    }
                }
                catch (Exception e)
                {
                    _Logger.LogError(LoggingEvents.GENERIC_ERROR, $"Method: {UtilityMethods.GetCallerMemberName()} with exception: {e.Message}");
                }
            }

            return ModelDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ConvertJsonStringToProvidedGenericType<T>(string json)
        {
            T result;
            if (string.IsNullOrWhiteSpace(json))
            {
                //throw new ArgumentException();  
                _Logger.LogError("1", $"Exception with method {UtilityMethods.GetCallerMemberName()} but parameter content is empty");
                return default(T);
            }
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return (T) (object) result;
            }
            catch (JsonSerializationException e)
            {
                _Logger.LogError("1", $"Exception with method {UtilityMethods.GetCallerMemberName()} with exception: {e.Message}");
                return default(T);
            }
        }

        //public static object SetValue(object obj, string parameterName, object parameterValue)
        //{
        //    try
        //    {
        //        obj.GetType().GetProperties().FirstOrDefault(x => x.Name == parameterName).SetValue(obj, parameterValue);
        //        return obj;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        /*
        public static void PrintProperties(object obj, int indent)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {
                    Debug.WriteLine($"property is {property.Name} is a IList");
                    foreach (var item in elems)
                    {
                        PrintProperties(item, indent + 3);
                    }
                }
                else
                {
                    var Type = property.PropertyType;
                    Debug.WriteLine($"Type is {Type} with name: {property.Name}");

                    int i = 1;
                    // This will not cut-off System.Collections because of the first check
                    // if (property.PropertyType.GetTypeInfo().Assembly == objType.GetTypeInfo().IsAssembly)
                    // {
                    //     Debug.WriteLine("{0}{1}:", indentString, property.Name);

                    //     PrintProperties(propValue, indent + 2);
                    // }
                    // else
                    // {
                    //     Debug.WriteLine("{0}{1}: {2}", indentString, property.Name, propValue);
                    // }
                }
            }
        }
        */
        /// <summary>
        /// Recursive
        /// useful method to parse an object type and get the constituent properties and nested object properties
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indent"></param>
        /// <param name="ParentObject"></param>
        public static void PrintProperties(object obj, int indent, string ParentObject = null)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var FoundParent = "";

                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;

                if (elems != null)
                {
                    FoundParent = elems[0].GetType().ToString();
                    //Debug.WriteLine($"Found a list of items with collection type {elems[0].GetType().ToString()}");
                    foreach (var item in elems)
                    {
                        PrintProperties(item, indent + 3, FoundParent);
                    }
                }
                else
                {
                    if (ParentObject != null)
                    {
                        //Debug.WriteLine($"{indentString} Current property Name: {property.Name} with type: {property.PropertyType.ToString()} For parent {ParentObject}");
                        if (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType.ToString() != "System.String")
                        {
                            //var InheritedObject = CreateObjectFromPropertyType(property.PropertyType);
                            //Debug.WriteLine($"{indentString} current property Name: {property.Name} is a class");
                            PrintProperties(propValue, indent + 2, ParentObject);
                        }
                    }
                    else
                    {
                        ParentObject = obj.GetType().ToString();
                        //Debug.WriteLine($"{indentString} Current property Name: {property.Name} with type: {property.PropertyType.ToString()} for base object {ParentObject}");
                    }
                }
            }
        }

        /// <summary>
        /// Static method to handle the recursive searching through an object to find and log all object properties
        /// </summary>
        /// <param name="obj">The object to search</param>
        /// <param name="propertyList">The current list of properties that have been generated and stored! default is null</param>
        /// <param name="parentObj">The parent object of the current obj (by default is null)</param>
        /// <returns>A list of object parameters</returns>
        public static List<ObjectPropertyDetails> ReadObjectAndParseProperties(object obj, List<ObjectPropertyDetails> propertyList = null, 
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

            //If the provided object is null - return null?
            if(obj == null) return ModelList;


            //Long term final event is to return a List
            return ModelList;
        }

        /// <summary>
        /// Static operation to trigger the instantiation of an object of the provided object type
        /// </summary>
        /// <param name="type">the provided object type</param>
        /// <returns></returns>
        public static object CreateObjectFromPropertyType(Type type)
        {
            object customObject = null;
            try
            {
                customObject = Activator.CreateInstance(type);

            }
            catch (Exception e)
            {
                _Logger.LogError(LoggingEvents.GENERIC_ERROR, $"error with exception {e.Message}");
            }
            return customObject;
        }
    }
}