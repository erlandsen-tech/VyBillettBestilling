using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public void endreStrekning(Hovedstrekning str, List<bool> ulikheter)
        {
            var dbt = new VyDbTilgang();
            for(int i = 0; i < ulikheter.Count; i++)
            {
                if (!ulikheter[i])
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                            dbt.settNyeHovedstrekningNavn(str.id, str.hovstr_navn, str.hovstr_kortnavn);
                            break;
                        case 2:
                            dbt.fjernStasjonerFraHovedstrekning(str.id, fjernedeStasjoner(str));
                            dbt.settInnStasjonerIHovedstrekning(str.id, nyeStasjoner(str), finnIndex(dbt.HentHovedstrekning(str.id).stasjon_Ider.ToList()));
                            break;
                        default: break;
                    }
                }
            }
        }
        //Her burde vi legge inn mulighet for å finne ny index til hver stasjon, men tiden strekker ikke til
        private static int finnIndex(List<int> stasjoner)
        {
            return (stasjoner.Count / 2);
        }
        private static List<int> fjernedeStasjoner(Hovedstrekning nyHvst)
        {
            var dbt = new VyDbTilgang();
            var gammelHvst = dbt.HentHovedstrekning(nyHvst.id);
            return gammelHvst.stasjon_Ider.Except(nyHvst.stasjon_Ider).ToList();
        }
        private static List<int> nyeStasjoner(Hovedstrekning nyHvst)
        {
            var dbt = new VyDbTilgang();
            var gammelHvst = dbt.HentHovedstrekning(nyHvst.id);
            return nyHvst.stasjon_Ider.Except(gammelHvst.stasjon_Ider).ToList();
        }
        public List<bool> likeStrekninger(Hovedstrekning nyHvst)
        {
            var dbt = new VyDbTilgang();
            var gammelHvst = dbt.HentHovedstrekning(nyHvst.id);
            var navn = gammelHvst.hovstr_navn == nyHvst.hovstr_navn;
            var kortnavn = gammelHvst.hovstr_kortnavn == nyHvst.hovstr_kortnavn;
            var stasjoner = true;
            var fjerneStasj = fjernedeStasjoner(nyHvst);
            var nyeStasj = nyeStasjoner(nyHvst);
            if(fjerneStasj.Count > 0 || nyeStasj.Count > 0)
            {
                stasjoner = false;
            }
            var nettid = gammelHvst.nett_id == nyHvst.nett_id;
            bool[] likheter =  { navn, kortnavn, stasjoner, nettid };
            return likheter.ToList();
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