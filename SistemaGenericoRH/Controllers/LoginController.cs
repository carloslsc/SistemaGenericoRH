using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaGenericoRH.Controllers
{
    public class LoginController : Controller
    {
        private string _route;

        public LoginController(IConfiguration configuration)
        {
            _route = configuration.GetSection("ExternalRoutes").GetSection("ApiRoute").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetExternalRoutes()
        {
            string route = _route;
            return Json(new JsonResult(new { data = route }));
        }
    }
}
