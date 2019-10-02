using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Methods
{
    public class HomeMethods
    {
        public static RuteView GetRuteView(Rute rute, Bestilling innBestilling)
        {
            VyDbTilgang dbt = new VyDbTilgang();
            RuteView ruteView = new RuteView();
            ruteView.DateTime = rute.DateTime;
            ruteView.Id = rute.Id;
            ruteView.StartNavn = dbt.HentStasjonsnavnMedId(rute.Start_id);
            ruteView.StartId = rute.Start_id;
            ruteView.StoppId = rute.Stopp_id;
            ruteView.StoppNavn = dbt.HentStasjonsnavnMedId(rute.Stopp_id);
            ruteView.AntallBarn = innBestilling.AntallBarn;
            ruteView.AntallHonnor = innBestilling.AntallHonnor;
            ruteView.AntallStudent = innBestilling.AntallStudent;
            ruteView.AntallVoksne = innBestilling.AntallVoksne;
            return ruteView;
        }
        public static List<Rute> RuteTabell(Bestilling bestilling)
        {
            VyDbTilgang vyDb = new VyDbTilgang();
            var ruter = vyDb.HentRute(bestilling.ReiseFra, bestilling.ReiseTil, bestilling.StartTid).ToList();
            return ruter;
        }
        //Tanken bak metoden under er å lage en reise
        //start, stopp, mellomstasjoner, priser
        //TODO
        //Implementere det ovenstående. Nå lager den bare en halv rute
        public static Reise ReiseRute(List<Rute> ruter)
        {
            VyDbTilgang vyDb = new VyDbTilgang();
            var reiser = new Reise();
            var startstasjon = vyDb.HentStasjon(ruter[0].Start_id);
            var stoppstasjon = vyDb.HentStasjon(ruter[1].Stopp_id);
            foreach (Rute rute in ruter)
            {

            }
            return reiser;
        }

    }
}