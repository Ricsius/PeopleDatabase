using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace PeopleDatabase.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select a file";

                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";

                return View();
            }

            try
            {
                int insertedCountries = await _countriesService.UploadCountriesFromExcel(excelFile);
                ViewBag.Message = $"Inserted {insertedCountries} countries";
            }
            catch (Exception e) 
            {
                ViewBag.ErrorMessage = e.Message;

                return View();
            }

            return View();
        }
    }
}
