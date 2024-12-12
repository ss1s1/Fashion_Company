using Microsoft.AspNetCore.Mvc;

namespace Fashion_Company.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
            return View();
		}
	}

}
