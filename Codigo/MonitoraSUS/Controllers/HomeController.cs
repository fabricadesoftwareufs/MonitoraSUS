using Microsoft.AspNetCore.Mvc;

namespace MonitoraSUS.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            Response.StatusCode = 404;
            return View("PaginaNaoEncontrada");

        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
