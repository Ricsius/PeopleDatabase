namespace PeopleDatabase.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                string exceptionType = string.Empty;
                string exceptionMessage = string.Empty;

                if (ex.InnerException != null)
                {
                    exceptionType = ex.InnerException.GetType().ToString();
                    exceptionMessage = ex.InnerException.Message;
                }
                else 
                {
                    exceptionType = ex.GetType().ToString();
                    exceptionMessage = ex.Message;
                }

                _logger.LogError("{ExceptionType} {ExceptionMessage}", exceptionType, exceptionMessage);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsync("Error occured");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
