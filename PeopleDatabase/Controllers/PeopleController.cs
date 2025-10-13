using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

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
        public IActionResult Index()
        {
            IEnumerable<PersonResponse> people = _peopleService.GetAllPersons();

            return View(people);
        }
    }
}
