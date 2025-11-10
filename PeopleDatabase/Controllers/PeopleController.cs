using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
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
        public async Task<IActionResult> Index(
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

            IEnumerable<PersonResponse> people = await _peopleService.SearchPeople(searchBy, searchString);

            ViewBag.SearchBy = searchBy;
            ViewBag.SearchString = searchString;

            people = await _peopleService.GetSortedPeople(people, sortby, sortOrder);

            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortOrder.ToString();

            return View(people);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CountriesDropdownSetup();

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                await CountriesDropdownSetup();

                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return View(request);
            }

            await _peopleService.AddPerson(request);

            return RedirectToAction(nameof(Index));
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? response = await _peopleService.GetPersonById(personId);

            if (response == null)
            {
                return RedirectToAction(nameof(Index));
            }

            PersonUpdateRequest request = response.ToPersonUpdateRequest();

            await CountriesDropdownSetup();

            return View(request);
        }

        [Route("[action]/{personId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonUpdateRequest request)
        {

            PersonResponse? personResponse = await _peopleService.GetPersonById(request.PersonId);

            if (personResponse == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CountriesDropdownSetup();

                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return View(request);
            }

            await _peopleService.UpdatePerson(request);

            return RedirectToAction(nameof(Index));
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? response = await _peopleService.GetPersonById(personId);

            if (response == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(response);
        }

        [Route("[action]/{personId}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonResponse person)
        {
            PersonResponse? response = await _peopleService.GetPersonById(person.PersonId);

            if (response == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _peopleService.DeletePerson(person.PersonId);

            return RedirectToAction(nameof(Index));
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> PeoplePdf() 
        {
            IEnumerable<PersonResponse> people = await _peopleService.GetAllPersons();
            ViewAsPdf view = new ViewAsPdf(nameof(PeoplePdf), people, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Right = 20,
                    Left = 20,
                    Bottom = 20,
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };

            return view;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> PeopleCsv()
        {
            MemoryStream stream = await _peopleService.GetPeopleCsv();

            return File(stream, "application/octet-stream", "people.csv");
        }

        private async Task CountriesDropdownSetup()
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
            ViewBag.Countries = items;
        }
    }
}
