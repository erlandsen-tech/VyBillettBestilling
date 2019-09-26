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
        [HttpGet]
        public JsonResult Stasjonsliste()
        {
            VyDbTilgang Context = new VyDbTilgang();
            return Json(Context.HentAlleStasjoner(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReiseRute(int stasjA, int stasjB)
        {
            var repo = new VyDbTilgang();
            return Json(repo.stierMellomStasjoner(stasjA, stasjB), JsonRequestBehavior.AllowGet);
        }
    }
}