using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaGenericoRH.Controllers
{
    public class UsuariosController : Controller
    {
        private string _route;

        public UsuariosController(IConfiguration configuration)
        {
            _route = configuration.GetSection("ExternalRoutes").GetSection("ApiRoute").Value;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
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
