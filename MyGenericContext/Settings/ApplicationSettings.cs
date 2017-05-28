using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGenericContext.Settings
{
    /// <summary>
    /// object model to store and maintain contents used in the triggering of application debug logs
    /// And other settings deemed application wide
    /// </summary>
    public class ApplicationSettings
    {
        public bool EnablePerformanceMonitoring { get; set; }
    }
}