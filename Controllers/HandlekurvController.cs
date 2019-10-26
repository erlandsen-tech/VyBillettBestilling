using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VyBillettBestilling.BLL.Methods;
using VyBillettBestilling.Model;

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
            var hkm = new HandlekurvMethods();
            Session["Handlekurv"] = hkm.OppdaterHandlekurv(StartId,StoppId,Voksen,Barn,Student,Honnor,avreise); 
            return View("Handlekurv");
        }
        public int EnheterIKurv()
        {
            int antallIKurv = 0;
            if (Session["Handlekurv"] is Handlekurv kurv && kurv.Billetter != null)
            {
                foreach (var enhet in kurv.Billetter)
                {
                    antallIKurv += enhet.Antall;
                }
            }
            return antallIKurv;
        }
        [HttpDelete]
        public void Slett(int id)
        {
            HandlekurvMethods hkm = new HandlekurvMethods();
            hkm.SlettFraKurv(id);
        }
    }
}