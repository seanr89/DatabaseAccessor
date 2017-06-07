
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MyGenericContext.Models;
using MyGenericContext.Utilities;

namespace MyGenericContext.Filters
{
    public class LogFilterTypeAttribute : TypeFilterAttribute {
        public LogFilterTypeAttribute()
            : base(typeof(LogFilterTypeAttribute)) { }

        private class SimplerLogFilter : IActionFilter {
            private readonly ILogger<LogFilterTypeAttribute> _Logger;

            public SimplerLogFilter(ILogger<LogFilterTypeAttribute> logger) {
                this._Logger = logger;
            }

            public void OnActionExecuting(ActionExecutingContext context) { }

            public void OnActionExecuted(ActionExecutedContext context) 
            {
                if (!context.Canceled) 
                {
                    _Logger.LogInformation(LoggingEvents.GENERIC_MESSAGE, $"Executed action {UtilityMethods.GetCallerMemberName()}");
                }
            }
        }
    }
}