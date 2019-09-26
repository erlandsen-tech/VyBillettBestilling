using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class VyDbTilgang
    {


        VyDbContext db = new VyDbContext();
        public IEnumerable<Stasjon> HentAlleStasjoner()
        {
            var alleStasjoner = db.Stasjoner.Select(dbst => new Stasjon
            {
                id = dbst.StasjonId,
                stasjon_navn = dbst.StasjNavn,
                stasjon_sted = dbst.StasjSted,
                breddegrad = dbst.Breddegrad,
                lengdegrad = dbst.Lengdegrad,
                hovedstrekninger = dbst.Hovedstrekninger.Select(hs => hs.HovstrId),
                nett_id = dbst.NettId,
                nett_navn = dbst.Nett.Nettnavn
            });
            return alleStasjoner;

            // Demonstrasjon pa hvordan gjore det pa en annen mate:
            // Vet ikke om denne Distinct-en gjor susen. SJEKK! Da kan slutt-Distinct-en droppes:
            //return db.Hovedstrekninger.SelectMany(hs => hs.Stasjoner//.Distinct()
            //,
            //    (dbhs, dbst) => new Stasjon
            //    {
            //        stasjon_navn = dbst.StasjNavn
            //    }
            //).Distinct().OrderBy(n => n.stasjon_navn+n.stasjon_sted);
        }

        public IEnumerable<Stasjon> HentStasjonerPaNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.Where(dbst => dbst.NettId == nettId).Select(dbst => new Stasjon
                {
                    id = dbst.StasjonId,
                    stasjon_navn = dbst.StasjNavn,
                    stasjon_sted = dbst.StasjSted,
                    breddegrad = dbst.Breddegrad,
                    lengdegrad = dbst.Lengdegrad,
                    hovedstrekninger = dbst.Hovedstrekninger.Select(hs => hs.HovstrId).ToArray(),
                    nett_id = dbst.NettId,
                    nett_navn = dbst.Nett.Nettnavn
                });

                // Demonstrasjon pa hvordan gjore det pa en annen mate:
                //return db.Nett.Find(nettId).Stasjoner.Select(dbst => new Stasjon
                //{
                //    id = dbst.StasjonId,
                //    stasjon_navn = dbst.StasjNavn,
                //    stasjon_sted = dbst.StasjSted,
                //    breddegrad = dbst.Breddegrad,
                //    lengdegrad = dbst.Lengdegrad,
                //    hovedstrekninger = dbst.Hovedstrekninger.Select(hs => hs.HovstrId).ToArray(),
                //    nett_id = dbst.NettId,
                //    nett_navn = dbst.Nett.Nettnavn
                //});
            }
        }

        public IEnumerable<IEnumerable<Stasjon>> stierMellomStasjoner(int ida, int idb)
        {
            using (var db = new VyDbContext())
            {
                DbStasjon a = db.Stasjoner.Find(ida);
                DbStasjon b = db.Stasjoner.Find(idb);
                if (ida == idb) // Start- og stoppstasjon er den samme. Det er bare tull, men returnerer en "sti" likevel
                    return new[]{new []{ new Stasjon
                    {
                        id = a.StasjonId,
                        stasjon_navn = a.StasjNavn,
                        stasjon_sted = a.StasjSted,
                        breddegrad = a.Breddegrad,
                        lengdegrad = a.Lengdegrad,
                        hovedstrekninger = a.Hovedstrekninger.Select(hs => hs.HovstrId).ToArray(),
                        nett_id = a.NettId,
                        nett_navn = a.Nett.Nettnavn
                    } } };
                if (a.NettId != b.NettId)
                    return new IEnumerable<Stasjon>[] { }; // Ingen sti finnes

                List<DbHovedstrekning> bhs = b.Hovedstrekninger;

                List<DbHovedstrekning> ahs = a.Hovedstrekninger;

                // Finn nabohovedstrekninger

                //List<DbStasjon> aep = ahs.
                // Forste hovedstrekning(er)
                // Hvis noen av nabohovedstrekningene har stasjon b er det funnet en sti (av hovedstrekninger). Lagre den, og ikke ga videre pa den hovedstrekningen
                // Hvis noen av nabohovedstrekningene finnes tidligere i grenen; blindvei, ikke ga videre pa de hovedstrekningene



            }


            return null;
        }

    }
}