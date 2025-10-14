using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace PeopleDatabase.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;

        public PeopleController(IPeopleService peopleService)
        {
            _peopleService = peopleService;
        }

        [Route("/people/index")]
        [Route("")]
        public IActionResult Index(
            string? searchBy, 
            string? searchString, 
            string sortby = nameof(PersonResponse.Name), 
            SortOrderOptions sortOrder = SortOrderOptions.Ascending)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name), "Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };

            IEnumerable<PersonResponse> people = _peopleService.SearchPeople(searchBy, searchString);

            ViewBag.SearchBy = searchBy;
            ViewBag.SearchString = searchString;

            people = _peopleService.GetSortedPeople(people, sortby, sortOrder);

            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortOrder.ToString();

            return View(people);
        }
    }
}
