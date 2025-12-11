using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : ActionFilterAttribute
    {
        private readonly string _headerKey;
        private readonly string _headerValue;

        public ResponseHeaderActionFilter(string headerKey, string headerValue, int order)
        {
            _headerKey = headerKey;
            _headerValue = headerValue;
            Order = order;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers[_headerKey] = _headerValue;
        }
    }
}
