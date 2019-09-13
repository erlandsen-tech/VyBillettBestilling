using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VyBillettBestilling.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home Tullekommentar
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Bestilling()
        {
            return View();
        }
    }
}