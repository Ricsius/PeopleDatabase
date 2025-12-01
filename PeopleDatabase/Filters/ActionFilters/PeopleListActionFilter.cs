using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using PeopleDatabase.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Globalization;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class PeopleListActionFilter : IActionFilter
    {
        private readonly string DefaultSearchBy = nameof(PersonResponse.Name);
        private readonly ILogger<PeopleListActionFilter> _logger;
        private IDictionary<string, object?>? _actionArguments;

        public PeopleListActionFilter(ILogger<PeopleListActionFilter> logger) 
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"{nameof(PeopleListActionFilter)}.{nameof(OnActionExecuting)} method");

            _actionArguments = context.ActionArguments;

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    _logger.LogInformation($"searchBy actual value: {searchBy}");

                    List<string> searchByOptions = new List<string> 
                    {
                        nameof(PersonResponse.Name),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryId),
                        nameof(PersonResponse.Address)
                    };

                    if (!searchByOptions.Any(o => o == searchBy))
                    {
                        context.ActionArguments["searchBy"] = DefaultSearchBy;
                        _logger.LogInformation($"searchBy updated to: {DefaultSearchBy}");
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"{nameof(PeopleListActionFilter)}.{nameof(OnActionExecuted)} method");

            if (_actionArguments == null)
            {
                return;
            }

            PeopleController controller = (PeopleController)context.Controller;
            _actionArguments.TryGetValue("searchBy", out object? searchBy);
            _actionArguments.TryGetValue("searchString", out object? searchString);
            _actionArguments.TryGetValue("sortBy", out object? sortBy);
            _actionArguments.TryGetValue("sortOrder", out object? sortOrder);

            controller.ViewBag.SearchBy = Convert.ToString(searchBy);
            controller.ViewBag.SearchString = Convert.ToString(searchString);
            controller.ViewBag.SortBy = Convert.ToString(sortBy);
            controller.ViewBag.SortOrder = Convert.ToString(sortOrder);

            _actionArguments = null;
        }
    }
}
