using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Controllers
{
    public class HomeController : Controller
    {
        VyDbContext Context = new VyDbContext();
        public ActionResult Index()
        {
            Session["Bestilling"] = new Bestilling();
            return View();
        }
        public ActionResult Handlekurv()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Bestilling(Bestilling InnBestilling)
        {
            return View(InnBestilling);
        }
    }
}