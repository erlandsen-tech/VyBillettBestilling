using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
            ruteView.StartNavn = dbt.HentStasjon(rute.Start_id).stasjon_navn;
            ruteView.StoppNavn = dbt.HentStasjon(rute.Stopp_id).stasjon_navn;

            ruteView.StartId = rute.Start_id;
            ruteView.StoppId = rute.Stopp_id;

            ruteView.AntallBarn = innBestilling.AntallBarn;
            ruteView.AntallHonnor = innBestilling.AntallHonnor;
            ruteView.AntallStudent = innBestilling.AntallStudent;
            ruteView.AntallVoksne = innBestilling.AntallVoksne;
            return ruteView;
        }
        public static List<Rute> RuteTabell(Bestilling bestilling)
        {
            var tid = bestilling.StartTid;
            var ruter = new List<Rute>();
            for (int i = 0; i < 10; i++)
            {
                tid = tid.AddHours(1);
                var rute = new Rute
                {
                    Start_id = bestilling.ReiseFra,
                    Stopp_id = bestilling.ReiseTil,
                    DateTime = tid
                };
                ruter.Add(rute);
            }
            return ruter;
        }
        public IEnumerable<SelectListItem> StasjonsNavn()
        {
            VyDbTilgang dbTilgang = new VyDbTilgang();
            {
                IEnumerable<SelectListItem> items = dbTilgang.HentAlleStasjoner().Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.stasjon_navn
                });
                items = items.OrderBy(n => n.Text);
                return items;
            }
        }
    }
}