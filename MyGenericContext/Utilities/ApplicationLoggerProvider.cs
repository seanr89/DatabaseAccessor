using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGenericContext.Utilities
{
    /// <summary>
    /// Static class to handle creation of loggers for classes that are not directly connected
    /// to the routing DI flow and therefore cannot interact and inject the logger directly
    /// </summary>
    public static class ApplicationLoggerProvider
    {
        /// <summary>
        /// Static object of the LoggerFactory implementation
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        /// <summary>
        /// Create a logger object for the provided type(T) class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>And object of the ILogger</returns>
        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();

        /// <summary>
        /// Method to handle logger creation based on the string name of a file
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <returns></returns>
        public static ILogger CreateLogger(string CategoryName) =>
                LoggerFactory.CreateLogger(CategoryName);
    }
}