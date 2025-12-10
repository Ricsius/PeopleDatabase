using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ExceptionFilters
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            string exceptionType = context.Exception.GetType().ToString();
            string exceptionMessage = context.Exception.Message;

            _logger.LogError("Exception filter {FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}",
                nameof(HandleExceptionFilter), nameof(OnException), exceptionType, exceptionMessage);

            if (_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult()
                {
                    Content = exceptionMessage,
                    StatusCode = 500
                };
            }
            else 
            {
                context.Result = new ContentResult()
                {
                    Content = "An unexpected error occured.",
                    StatusCode = 500
                };
            }
        }
    }
}
