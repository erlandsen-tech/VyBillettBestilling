using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VyBillettBestilling.Models;
using VyBillettBestilling.Methods;
using System.Diagnostics;

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

            VyDbTilgang dbt = new VyDbTilgang();
            //dbt.addPassasjertyper();
            //dbt.ByggBanedata();
            //dbt.addPris();
            //Debug.WriteLine(dbt.leggTilNett("tullenett"));
            //Debug.WriteLine("HERERJEG!!!");
            //dbt.fjernNett(2);

            HomeMethods hmt = new HomeMethods();
            ViewBag.Stasjoner = hmt.StasjonsNavn();
            return View();
        }

        public ActionResult Handlekurv()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Bestilling(Bestilling InnBestilling)
        {
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
            return Json(Context.HentAlleStasjoner().OrderBy(n => n.stasjon_navn), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string HentStasjonsnavnMedId(int id)
        {
            VyDbTilgang context = new VyDbTilgang();
            return context.HentStasjon(id).stasjon_navn;
        }
        [HttpGet]
        public int HentNettForStasjon(int id)
        {
            VyDbTilgang context = new VyDbTilgang();
            return context.HentStasjon(id).nett_id;
        }
    }
}