using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
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

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} called", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            _logger.LogInformation("Before calling next filter");
            
            await next();

            _logger.LogInformation("After calling next filter");

            context.HttpContext.Response.Headers[_headerKey] = _headerValue;
        }
    }
}
