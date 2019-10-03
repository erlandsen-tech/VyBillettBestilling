using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class VyDbTilgang
    {
        public Stasjon HentStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : konverterStasjon(funnet);
            }
        }
        public Passasjer Passasjertype(int typeId)
        {
            using (var db = new VyDbContext())
            {
                var dbpass = db.Passasjertyper.Find(typeId);
                var pass = new Passasjer
                {
                    rabatt = dbpass.Rabatt,
                    typenavn = dbpass.TypeNavn
                };
                return pass;
            }
        }
        public IEnumerable<Stasjon> HentAlleStasjoner()
        {
            var db = new VyDbContext();
            var alle = db.Stasjoner.ToList().Select(dbst => konverterStasjon(dbst)).ToList();
            db.Dispose();
            return alle;
        }

        /** 
         * Under har vi metoder for å manipulere databasen og legge til eksempeldata
         * **/

        private Stasjon konverterStasjon(DbStasjon dbst)
        {
            return new Stasjon
            {
                id = dbst.StasjonId,
                stasjon_navn = dbst.StasjNavn,
                stasjon_sted = dbst.StasjSted,
                breddegrad = dbst.Breddegrad,
                lengdegrad = dbst.Lengdegrad,
            };
        }

        public void AddStasjoner()
        {
            using (var db = new VyDbContext())
            {
                VyDbTilgang dbt = new VyDbTilgang();
                List<CSVstasjon> liste = CSVstasjon.convertEngine();
                foreach (CSVstasjon stasjon in liste)
                {
                    DbStasjon st = new DbStasjon
                    {
                        StasjSted = stasjon.name,
                        StasjNavn = stasjon.name,
                        Lengdegrad = stasjon.longitude,
                        Breddegrad = stasjon.latitude
                    };
                    db.Stasjoner.Add(st);
                }
                db.SaveChanges();
            }
        }

        public void addPassasjertyper()
        {
            using (var db = new VyDbContext())
            {
                string[] typeNavn = { "Voksen", "Barn", "Student", "Honnor" };
                int[] rabatt = { 0, 60, 50, 60 };
                int[] ovrealder = { 67, 17, 35, 999 };
                int[] nedrealder = { 18, 0, 0, 68 };
                for (int i = 0; i < typeNavn.Length; i++)
                {
                    DbPassasjertype dbp = new DbPassasjertype
                    {
                        Rabatt = rabatt[i],
                        TypeNavn = typeNavn[i],
                        OvreAldersgrense = ovrealder[i],
                        NedreAldersgrense = nedrealder[i]
                    };
                    db.Passasjertyper.Add(dbp);
                }
                db.SaveChanges();
            }
        }
    }
}