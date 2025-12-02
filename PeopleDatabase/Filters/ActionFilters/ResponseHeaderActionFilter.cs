using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _headerKey;
        private readonly string _headerValue;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string headerKey, string headerValue)
        {
            _logger = logger;
            _headerKey = headerKey;
            _headerValue = headerValue;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} called", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} called", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));

            context.HttpContext.Response.Headers[_headerKey] = _headerValue;
        }
    }
}
