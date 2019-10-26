using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VyBillettBestilling.Model;
using VyBillettBestilling.BLL;
using VyBillettBestilling.BLL.Methods;
using VyBillettBestilling.ViewModels;

namespace VyBillettBestilling.Controllers
{
    public class HomeController : Controller
    {
        KonverterModel konverter = new KonverterModel();
        public ActionResult Index()
        {
            if (Session["Bestilling"] == null)
            {
                Session["Bestilling"] = new Bestilling();
            }
            if (Session["Handlekurv"] == null)
            {
                Session["Handlekurv"] = new Model.Handlekurv();
            }
            //Eksempeldata 
            //VyDbTilgang dbt = new VyDbTilgang();
            //dbt.addPassasjertyper();
            //dbt.ByggBanedata();
            //dbt.addPris();
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
            List<Model.RuteView> ruteView = new List<Model.RuteView>();
            var ruter = HomeMethods.RuteTabell(InnBestilling);
            foreach (Model.Rute rute in ruter)
            {
                ruteView.Add(HomeMethods.GetRuteView(rute, InnBestilling));
            }
            var returView = konverter.ruteView(ruteView);
            return View(returView);
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