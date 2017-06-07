
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MyGenericContext.Utilities;

namespace MyGenericContext.Filters
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogFilterAttribute> _Logger;

        public LogFilterAttribute(ILogger<LogFilterAttribute> Logger)
        {
            _Logger = Logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context) 
        {
            if (!context.Canceled) 
            {
                _Logger.LogInformation($"Executed action {UtilityMethods.GetCallerMemberName()}");
                base.OnActionExecuted(context);
            }
        }
    }
}