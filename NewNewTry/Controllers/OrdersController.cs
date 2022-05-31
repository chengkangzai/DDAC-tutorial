using Microsoft.AspNetCore.Mvc;

namespace NewNewTry.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
