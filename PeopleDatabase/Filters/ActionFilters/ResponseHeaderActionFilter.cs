using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _headerKey;
        private readonly string _headerValue;

        public int Order { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string headerKey, string headerValue, int order)
        {
            _logger = logger;
            _headerKey = headerKey;
            _headerValue = headerValue;
            Order = order;
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
