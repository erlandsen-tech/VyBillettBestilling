using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Controllers
{
    public class HandlekurvController : Controller
    {
        // GET: Handlekurv
        public ActionResult Handlekurv()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LeggOppi(
            int StartId, int StoppId, int Voksen, int Barn, int Student, int Honnor, long avreise)
        {
            var dbt = new VyDbTilgang();
            DateTime Avreise = new DateTime(avreise);
            var StartNavn = dbt.HentStasjon(StartId).stasjon_navn;
            var StoppNavn = dbt.HentStasjon(StoppId).stasjon_navn;

            var CurrentKorg = (Session["Handlekurv"] as Handlekurv ?? new Handlekurv());
            if (CurrentKorg.Billetter == null)
            {
                CurrentKorg.Billetter = new List<Billett>();
            }
            int[] Billetter = { Voksen, Barn, Student, Honnor };
            for (int i = 0; i < Billetter.Count(); i++)
            {

                if (Billetter[i] > 0)
                {
                    var bill = LagBillett(i + 1, Billetter[i]);
                    bill.StartStasjon = StartNavn;
                    bill.StoppStasjon = StoppNavn;
                    bill.Avreise = Avreise;//avreise;
                    CurrentKorg.Billetter.Add(bill);
                }
                else
                {
                    Billetter[i] = 0;
                }
            }
            Session["Handlekurv"] = CurrentKorg;
            return View("Handlekurv");
        }

        //Lager billetter. Etter at vi implementerer kunde, legger vi også
        // til mulighet for implementasjon av billettid mot database
        public Billett LagBillett(int type, int antall)
        {
            var dbt = new VyDbTilgang();
            Billett billett = new Billett();
            billett.Passasjertype = dbt.Passasjertype(type);
            billett.Antall = antall;
            billett.Pris = 300;
            if (billett.Passasjertype.rabatt != 0)
            {
                billett.Pris *= (billett.Passasjertype.rabatt / 100);
            }
            return billett;
        }

        public int EnheterIKurv()
        {
            int antallIKurv = 0;
            var kurv = Session["Handlekurv"] as Handlekurv;
            if (kurv != null && kurv.Billetter != null)
            {
                foreach (var enhet in kurv.Billetter)
                {
                    antallIKurv += enhet.Antall;
                }
            }
            return antallIKurv;
        }
    }
}