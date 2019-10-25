using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VyBillettBestilling.Models;
using VyBillettBestilling.Models.View;

namespace VyBillettBestilling.Methods
{
    public class ManageMethods
    {
        public List<Stasjon> FinnStasjonerUtenHovedStrekning()
        {
            var StasjonerUtenHovedStrekning = new List<Stasjon>();
            var dbt = new VyDbTilgang();
            var alleStasjoner = dbt.HentAlleStasjoner();
            foreach (Stasjon stasjon in alleStasjoner)
            {
                if (stasjon.hovedstrekning_Ider.Count <= 0)
                {
                    StasjonerUtenHovedStrekning.Add(stasjon);
                }
            }
            return StasjonerUtenHovedStrekning;
        }
        public Hovedstrekning LagHovedstrekning(HovedstrekningCreateView hvstcv)
        {
            var dbt = new VyDbTilgang();
            var idList = new List<int>();
            var stasjoner = dbt.HentAlleStasjoner();
            foreach (string stasjonsnavn in hvstcv.stasjonsliste)
            {
                foreach (Stasjon stasjon in stasjoner)
                {
                    if (stasjon.stasjon_navn == stasjonsnavn)
                    {
                        idList.Add(stasjon.id);
                    }
                }
            }
            var hvst = new Hovedstrekning
            {
                hovstr_navn = hvstcv.hovstr_navn,
                hovstr_kortnavn = hvstcv.hovstr_kortnavn,
                nett_id = parseNettId(hvstcv.nettid),
                stasjon_Ider = idList
            };
            return hvst;
        }
        private static int parseNettId(String nettid)
        {
            int i = 0;
            if (!Int32.TryParse(nettid, out i))
            {
                i = -1;
            }
            return i;
        }
    }
}