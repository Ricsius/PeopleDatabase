using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace PeopleDatabase.Controllers
{
    [Route("[controller]")]
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;

        public PeopleController(IPeopleService peopleService, ICountriesService countriesService)
        {
            _peopleService = peopleService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
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

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            SetupCreate();

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                SetupCreate();

                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return View();
            }

            _peopleService.AddPerson(request);

            return RedirectToAction(nameof(Index));
        }

        private void SetupCreate()
        {
            IEnumerable<SelectListItem> countries = _countriesService
                .GetAllCountries()
                .OrderBy(c => c.CountryName)
                .Select(c => new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.CountryId.ToString()
                });
            ViewBag.Countries = countries;
        }
    }
}
