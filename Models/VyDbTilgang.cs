using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class VyDbTilgang
    {
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


        public List<Stasjon> HentAlleStasjoner()
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Select(dbst => konverterStasjon(dbst)).ToList();
            }
        }

        public Stasjon HentStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : konverterStasjon(funnet);
            }
        }


        public List<String> HentAlleStasjonNavn()
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Select(dbst => dbst.StasjNavn).ToList();
            }
        }



        /** 
         * Under har vi metoder for å manipulere databasen og legge til eksempeldata
         * **/
         public void ByggBanedata()
        {
            using (var db = new VyDbContext())
            {
                using (var dbtransaction = db.Database.BeginTransaction()) // Har to SaveChanges, bruker derfor transaction
                {
                    try
                    {
                        Dictionary<string, DbStasjon> stasjDict = new Dictionary<string, DbStasjon>();
                        List<CSVstasjon> stasjliste = CSVstasjon.convertEngine();
                        IEnumerable<string> nettnavner = stasjliste.Select(st => st.nettnavn).Distinct();
                        List<DbNett> netter = new List<DbNett>(nettnavner.Count());
                        foreach (string n in nettnavner)
                            netter.Add(new DbNett(n));
                        foreach (DbNett n in netter)
                            db.Nett.Add(n);
                        db.SaveChanges(); // Ma ha denne for at DbNett-ene skal fa tildelt Id som brukes ved konstruksjon av DbHovedstrekning-er og DbStasjon-er
                        var grupper = stasjliste.GroupBy(st => st.ns2banekortnavn);
                        foreach (var grp in grupper)
                        {
                            DbNett grpNett = netter.Single(nt => nt.Nettnavn.Equals(grp.First().nettnavn));
                            DbHovedstrekning hovst = new DbHovedstrekning(grp.First().ns2banenavn, grpNett, grp.First().ns2banekortnavn);
                            grpNett.Hovedstrekninger.Add(hovst);
                            List<CSVstasjon> csvstasjList = grp.OrderBy(csv => csv.ns2sporkilometer).ToList();
                            foreach (CSVstasjon st in csvstasjList)
                            {
                                if (!stasjDict.TryGetValue(st.ns1id2, out DbStasjon stasj))
                                {
                                    stasj = new DbStasjon(st.ns2stasjonsnavn, grpNett);
                                    grpNett.Stasjoner.Add(stasj);
                                    stasj.Breddegrad = st.breddegrad;
                                    stasj.Lengdegrad = st.lengdegrad;
                                    stasjDict.Add(st.ns1id2, stasj);
                                }
                                stasj.Hovedstrekninger.Add(hovst);
                                hovst.Stasjoner.Add(stasj);
                            }
                        }
                        db.SaveChanges();
                        dbtransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbtransaction.Rollback();
                        throw ex; // Ma nesten kaste den videre.
                    }
                }
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

        private Stasjon konverterStasjon(DbStasjon dbst)
        {
            return new Stasjon
            {
                id = dbst.Id,
                stasjon_navn = dbst.StasjNavn,
                stasjon_sted = dbst.StasjSted,
                breddegrad = dbst.Breddegrad,
                lengdegrad = dbst.Lengdegrad,
            };
        }

    }
}