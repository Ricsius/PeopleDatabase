using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PeopleDatabase.Controllers
{
    public class HomeController : Controller
    {
        private const string DefaultErrorMessage = "Error occured during execution";
        [Route("Error")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? f = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ErrorMessage = f?.Error != null 
                ? f.Error.Message 
                : DefaultErrorMessage;

            return View();
        }
    }
}
