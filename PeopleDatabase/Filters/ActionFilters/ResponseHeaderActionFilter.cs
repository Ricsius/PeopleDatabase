using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class ResponseHeaderActionFilterFactory : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly string _headerKey;
        private readonly string _headerValue;
        private int _order { get; set; }

        public ResponseHeaderActionFilterFactory(string headerKey, string headerValue, int order)
        {
            _headerKey = headerKey;
            _headerValue = headerValue;
            _order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            ILogger<ResponseHeaderActionFilter> logger = serviceProvider.GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
            ResponseHeaderActionFilter instance = new ResponseHeaderActionFilter(logger)
            {
                HeaderKey = _headerKey,
                HeaderValue = _headerValue,
                Order = _order
            };

            return instance;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string HeaderKey { get; set; } = string.Empty;
        public string HeaderValue { get; set; } = string.Empty;
        public int Order { get; set; }
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} called", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            _logger.LogInformation("Before calling next filter");

            context.HttpContext.Response.Headers[HeaderKey] = HeaderValue;

            await next();

            _logger.LogInformation("After calling next filter");
        }
    }
}
