using Microsoft.AspNetCore.Mvc;
using PeopleDatabase.Filters.ActionFilters;
using PeopleDatabase.Filters.AuthorizationFilters;
using PeopleDatabase.Filters.ExceptionFilters;
using PeopleDatabase.Filters.ResourceFilters;
using PeopleDatabase.Filters.ResultFilters;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace PeopleDatabase.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(ResponseHeaderActionFilter),
            Arguments = new object[] { "X-Key-FromController", "Custom-Value-FromController", 1 }, 
            Order = 1)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    public class PeopleController : Controller
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PeopleController> _logger;

        public PeopleController(IPeopleService peopleService, ICountriesService countriesService, ILogger<PeopleController> logger)
        {
            _peopleService = peopleService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PeopleListActionFilter), 
            Order = 4)]
        [TypeFilter(typeof(ResponseHeaderActionFilter), 
            Arguments = new object[] { "X-Custom-Key-FromIndex", "Custom-Value-FromIndex", 3 }, 
            Order = 3)]
        [TypeFilter(typeof(PeopleListResultFilter))]
        public async Task<IActionResult> Index(
            string? searchBy,
            string? searchString,
            string sortby = nameof(PersonResponse.Name),
            SortOrderOptions sortOrder = SortOrderOptions.Ascending)
        {
            _logger.LogInformation($"{nameof(Index)} action of {nameof(PeopleController)}");
            _logger.LogDebug(@$"{nameof(searchBy)}: {searchBy}, {nameof(searchString)}: {searchString}, {nameof(sortby)}: {sortby}, {nameof(sortOrder)}: {sortOrder}");

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

            people = await _peopleService.GetSortedPeople(people, sortby, sortOrder);

            return View(people);
        }

        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation($"{nameof(Create)} GET action of {nameof(PeopleController)}");

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest request)
        {
            _logger.LogInformation($"{nameof(Create)} POST action of {nameof(PeopleController)}");

            await _peopleService.AddPerson(request);

            return RedirectToAction(nameof(Index));
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            _logger.LogInformation($"{nameof(Edit)} GET action of {nameof(PeopleController)}");

            PersonResponse? response = await _peopleService.GetPersonById(personId);

            if (response == null)
            {
                return RedirectToAction(nameof(Index));
            }

            PersonUpdateRequest request = response.ToPersonUpdateRequest();

            return View(request);
        }

        [Route("[action]/{personId}")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest request)
        {
            _logger.LogInformation($"{nameof(Edit)} POST action of {nameof(PeopleController)}");

            PersonResponse? personResponse = await _peopleService.GetPersonById(request.PersonId);

            if (personResponse == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await _peopleService.UpdatePerson(request);

            return RedirectToAction(nameof(Index));
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personId)
        {
            _logger.LogInformation($"{nameof(Delete)} GET action of {nameof(PeopleController)}");

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
            _logger.LogInformation($"{nameof(Delete)} POST action of {nameof(PeopleController)}");

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
            _logger.LogInformation($"{nameof(PeoplePdf)} action of {nameof(PeopleController)}");

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
            _logger.LogInformation($"{nameof(PeopleCsv)} action of {nameof(PeopleController)}");

            MemoryStream stream = await _peopleService.GetPeopleCsv();

            return File(stream, "application/octet-stream", "people.csv");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> PeopleExcel()
        {
            _logger.LogInformation($"{nameof(PeopleExcel)} action of {nameof(PeopleController)}");

            MemoryStream stream = await _peopleService.GetPeopleExcel();

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "people.xlsx");
        }
    }
}
