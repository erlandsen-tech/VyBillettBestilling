using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Methods
{
    public class HandlekurvMethods
    {
         //Henter billettpris fra database og genererer et kjop
        public Billett LagBillett(int type, int antall, int startId, int stoppId)
        {
            var dbt = new VyDbTilgang();
            var startStasjon = dbt.HentStasjon(startId);
            var stoppStasjon = dbt.HentStasjon(stoppId);
            //Henter km i luftlinje via lengde og breddegrad
            var avstand = Klodeavstander.overflateDistanseJorda(startStasjon.breddegrad, startStasjon.lengdegrad, stoppStasjon.breddegrad, stoppStasjon.lengdegrad);
            Billett billett = new Billett(); ;
            int sisteid = 1;
            //Må lage unik ID da posisjonen kan endre seg i tabell ved sletting
            if (HttpContext.Current.Session["Handlekurv"] is Handlekurv kurv
                && kurv.Billetter != null
                && kurv.Billetter.Count > 0)
            {
                sisteid = kurv.Billetter[kurv.Billetter.Count - 1].Id;
                sisteid += 1;
            }
            billett.Passasjertype = dbt.Passasjertype(type);
            billett.Antall = antall;
            billett.Pris = dbt.HentPris().prisPrKm * avstand;
            if (billett.Passasjertype.rabatt != 0)
            {
                billett.Pris *= (billett.Passasjertype.rabatt / 100);
            }
            billett.Id = sisteid;
            billett.Pris = Math.Round(billett.Pris, 2);
            return billett;
        }
        public Handlekurv OppdaterHandlekurv(int StartId, int StoppId, int Voksen, int Barn, int Student, int Honnor, long avreise)
        {

            var dbt = new VyDbTilgang();
            DateTime Avreise = new DateTime(avreise);
            var StartNavn = dbt.HentStasjon(StartId).stasjon_navn;
            var StoppNavn = dbt.HentStasjon(StoppId).stasjon_navn;

            var CurrentKorg = (HttpContext.Current.Session["Handlekurv"] as Handlekurv ?? new Handlekurv());
            if (CurrentKorg.Billetter == null)
            {
                CurrentKorg.Billetter = new List<Billett>();
            }
            int[] Billetter = { Voksen, Barn, Student, Honnor };
            for (int i = 0; i < Billetter.Count(); i++)
            {

                if (Billetter[i] > 0)
                {
                    var hkm = new HandlekurvMethods();
                    var bill = hkm.LagBillett(i + 1, Billetter[i], StartId, StoppId);
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
            return CurrentKorg;
        }

        public void SlettFraKurv(int id)
        {
            if (HttpContext.Current.Session["Handlekurv"] is Handlekurv kurv)
            {
                foreach (Billett billett in kurv.Billetter)
                {
                    if (billett.Id == id)
                    {
                        try
                        {
                            kurv.Billetter.Remove(billett);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error " + e + " when deleting billett with id " + id);
                        }
                        //Hopp ut av loop etter slett da ID skal være unik
                        break;
                    }
                }
            }
        }
    }
}