using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using PeopleDatabase.Controllers;
using ServiceContracts;
using ServiceContracts.DTO;

namespace PeopleDatabase.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PeopleController controller)
            {
                IEnumerable<CountryResponse> countries = await _countriesService
                        .GetAllCountries();

                IEnumerable<SelectListItem> items = countries
                    .OrderBy(c => c.CountryName)
                    .Select(c => new SelectListItem()
                    {
                        Text = c.CountryName,
                        Value = c.CountryId.ToString()
                    });

                controller.ViewBag.Countries = items;

                if (!controller.ModelState.IsValid)
                {
                    controller.ViewBag.Errors = controller.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                    object? request = context.ActionArguments["request"];

                    if (request is PersonAddRequest addRequest)
                    {
                        context.Result = controller.View(addRequest);
                    }
                    else if (request is PersonUpdateRequest updateRequest)
                    {
                        context.Result = controller.View(updateRequest);
                    }
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
