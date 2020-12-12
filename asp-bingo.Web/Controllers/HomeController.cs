using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using asp_bingo.Web.Models;
using asp_bingo.Web.Services;
using Microsoft.AspNetCore.Http;

namespace asp_bingo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
			if (HttpContext.Session.GetString("bingo") != "banko") return Redirect("~/Home/SignUp");
            return View();
        }

		[HttpGet]
		public IActionResult SignUp() => View();

		[HttpPost]
        public IActionResult SignUp(string name, string className, string color)
        {
            HttpContext.Session.SetString("name", name);
            HttpContext.Session.SetString("class", className);
            HttpContext.Session.SetString("color", color);
            HttpContext.Session.SetString("bingo", "banko");
            return Redirect("~/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
