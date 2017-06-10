using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyGenericContext.Utilities
{
    /// <summary>
    /// Class to provide controls for event execution performance and time
    /// </summary>
    public class ExecutionPerformanceMonitor : IDisposable
    {
        private Stopwatch _StopWatch = null;
        private readonly bool _Enabled;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enabled">If performance monitoring is enabled (default = false)</param>
        public ExecutionPerformanceMonitor(bool enabled = false)
        {
            if(enabled)
            {
                _StopWatch = Stopwatch.StartNew();
            }
        }

        /// <summary>
        /// Operation to generate an ellapsed time message for the execution of the provided method name
        /// </summary>
        /// <param name="MethodName">The name of the method that is being executed</param>
        /// <returns>A formatted string documenting the ellapsed time/duration of the method</returns>
        public string CreatePerformanceTimeMessage(string MethodName)
        {
            string result = "";
            if (_StopWatch != null)
            {
                if(_Enabled)
                    result = string.Format($"{MethodName} ellapsed time = {_StopWatch.ElapsedMilliseconds.ToString()} Milliseconds");
                else
                    result = string.Format("Monitoring disabled");
            }
            else
            {
                result = string.Format($"No time performance for {MethodName}");
            }
            return result;
        }

        /// <summary>
        /// Inherited interface method
        /// stops the timer
        /// </summary>
        public void Dispose()
        {
            if(_StopWatch != null)
            _StopWatch.Stop();
            _StopWatch = null;
        }
    }
}