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
            int Id, int Voksen, int Barn, int Student, int Honnor)
        {
            // voksne barn student honnør
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
                    Billett bill = new Billett();
                    bill = lagBillett(i + 1, Billetter[i]);
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

        //Midlertidig, her skal hentes fra billetbase
        //og regne pris basert på distanse
        public Billett lagBillett(int type, int antall)
        {
            Billett billett = new Billett();
            billett.Type = type;
            billett.Antall = antall;
            //billett.ReiseFra = fra;
            //billett.ReiseTil = til;
            return billett;
        }

        public int Pris(List<Billett> billetter)
        {
            return 0;
        }
    }
}