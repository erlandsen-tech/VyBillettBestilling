using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VyBillettBestilling.Models;
using VyBillettBestilling.Methods;
namespace VyBillettBestilling.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Bestilling"] == null)
            {
                Session["Bestilling"] = new Bestilling();
            }
            if (Session["Handlekurv"] == null)
            {
                Session["Handlekurv"] = new Handlekurv();
            }
//Eksempeldata 
/**
            VyDbTilgang dbt = new VyDbTilgang();

            dbt.addPassasjertyper();
            dbt.addNett();
            dbt.AddStasjoner();
            Random r = new Random();
            for (int i = 1; i < 188; i++)
            {
                int rInt = r.Next(1, 188);
                int boole = r.Next(0, 1);
                dbt.addRute(i, rInt);
                if (boole != 0)
                {
                    rInt = r.Next(2, 188);
                    dbt.addRute(i, rInt);
                }
            }
    **/
// ferdig

            return View();
        }

        public ActionResult Handlekurv()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Bestilling(Bestilling InnBestilling)
        {
            Session["RuteView"] = new List<RuteView>();
            List<RuteView> ruteView = new List<RuteView>();
            var ruter = HomeMethods.RuteTabell(InnBestilling);
            foreach (Rute rute in ruter)
            {
                ruteView.Add(HomeMethods.GetRuteView(rute, InnBestilling));
            }
            return View(ruteView);
        }

        [HttpGet]
        public JsonResult Stasjonsliste()
        {
            VyDbTilgang Context = new VyDbTilgang();
            return Json(Context.HentAlleStasjoner(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public string HentStasjonsnavnMedId(int id)
        {
            VyDbTilgang context = new VyDbTilgang();
            return context.HentStasjonsnavnMedId(id);
        }

        [HttpGet]
        public JsonResult ReiseRute(int stasjA, int stasjB)
        {
            var repo = new VyDbTilgang();
            return Json(repo.stierMellomStasjoner(stasjA, stasjB), JsonRequestBehavior.AllowGet);
        }
    }
}