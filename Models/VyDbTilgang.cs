using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class VyDbTilgang
    {
        /**
         *  Generelle hentemetoder for (konverterte) poster i basen
         *  (Blir utvidet med metoder for de fleste resterende tabellene)
         */
        public List<Passasjer> HentPassasjerTyper()
        {
            var passasjerer = new List<Passasjer>();
            using (var db = new VyDbContext())
            {
                var dbpassasjer = db.Passasjertyper.ToList();
                foreach(DbPassasjertype pass in dbpassasjer)
                {
                    var passasjer = KonverterPassasjer(pass);
                    passasjerer.Add(passasjer);
                }
            }
            return passasjerer;
        }
        public Passasjer HentPassasjer(int id)
        {
            using(var db = new VyDbContext())
            {
                var dbpassasjer = db.Passasjertyper.Find(id);
                var passasjer = KonverterPassasjer(dbpassasjer);
                return passasjer;
            }
        }
        public void OppdaterPassasjer(Passasjer passasjer)
        {
            using (var db = new VyDbContext())
            {
                var dbPassasjer = db.Passasjertyper.Find(passasjer.ptypId);
                dbPassasjer.Rabatt = passasjer.rabatt;
                dbPassasjer.TypeNavn = passasjer.typenavn;
                dbPassasjer.OvreAldersgrense = passasjer.ovreAlder;
                dbPassasjer.NedreAldersgrense = passasjer.nedreAlder;
                db.Entry(dbPassasjer).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private static Passasjer KonverterPassasjer(DbPassasjertype pass)
        {
            var passasjer = new Passasjer
            {
                ptypId = pass.ptypId,
                rabatt = pass.Rabatt,
                typenavn = pass.TypeNavn,
                ovreAlder = pass.OvreAldersgrense,
                nedreAlder = pass.NedreAldersgrense
            };
            return passasjer;
        }
        private static DbPassasjertype KonverterTilDbPassasjer(Passasjer pass)
        {
            var dbpassasjer = new DbPassasjertype
            {
                ptypId = pass.ptypId,
                Rabatt = pass.rabatt,
                TypeNavn = pass.typenavn,
                OvreAldersgrense = pass.ovreAlder,
                NedreAldersgrense = pass.nedreAlder
            };
            return dbpassasjer;
        }
        public Stasjon HentStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : konverterStasjon(funnet);
            }
        }
        public Pris HentPris()
        {
            using (var db = new VyDbContext())
            {
                var dbPris = db.Pris.FirstOrDefault();
                var pris = new Pris
                {
                    prisPrKm = dbPris.prisPrKm
                };
                return pris;
            }
        }
        public void SettPris(double nyPris)
        {
            using (var db = new VyDbContext())
            {
                var dbPris = db.Pris.FirstOrDefault();
                db.Pris.Remove(dbPris);
                DbPris pris = new DbPris()
                {
                    Id = 1,
                    prisPrKm = nyPris
                };
                db.Pris.Add(pris);
                db.SaveChanges();
            }
        }
        public List<Stasjon> HentAlleStasjoner()
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Select(dbst => konverterStasjon(dbst)).ToList();
            }
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
        public List<Stasjon> HentStasjonerEtterBegNavn(String begNavn)
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Where(st => st.StasjNavn.StartsWith(begNavn)).Select(st => konverterStasjon(st)).ToList();
            }
        }
        public List<Stasjon> HentStasjonerPaNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                return (funnet == null) ? null : funnet.Stasjoner.Select(dbst => konverterStasjon(dbst)).ToList();

                //Demonstrasjon pa hvordan gjore det pa en annen mate:
                //return db.Stasjoner.Where(dbst => dbst.NettId == nettId).Select(dbst => konverterStasjon(dbst));
            }
        }
        public List<Stasjon> HentStasjonerPaHovedstrekning(int hovstrId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Hovedstrekninger.Find(hovstrId);
                return (funnet == null) ? null : funnet.Stasjoner.ToList().Select(dbst => konverterStasjon(dbst)).ToList();
            }
        }
        public List<Stasjon> HentStasjoner(String stasjNavn, String optSted = "")
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Where(st => st.StasjNavn.Equals(stasjNavn) && (optSted.Length == 0 || st.StasjSted.Equals(optSted)))
                    .Select(st => konverterStasjon(st)).ToList();
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
                // Droppe denne?:

                hovedstrekning_Ider = dbst.Hovedstrekninger.Select(hs => hs.Id).ToList(),
                nett_id = (dbst.Nett != null)? dbst.Nett.Id : -1

            };
        }

        public Hovedstrekning HentHovedstrekning(int hovstrId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Hovedstrekninger.Find(hovstrId);
                return (funnet == null) ? null : konverterHovedstrekning(funnet);
            }
        }
        public List<Hovedstrekning> HentAlleHovedstrekninger()
        {
            using (var db = new VyDbContext())
            {
                return db.Hovedstrekninger.ToList().Select(dbho => konverterHovedstrekning(dbho)).ToList();
            }
        }
        public List<Hovedstrekning> HentHovedstrekningerEtterBegNavn(String begNavn)
        {
            using (var db = new VyDbContext())
            {
                return db.Hovedstrekninger.ToList().Where(ho => ho.HovstrNavn.StartsWith(begNavn)).Select(ho => konverterHovedstrekning(ho)).ToList();
            }
        }
        public List<Hovedstrekning> HentHovedstrekningerEtterBegKortNavn(String begNavn)
        {
            using (var db = new VyDbContext())
            {
                return db.Hovedstrekninger.ToList().Where(ho => ho.HovstrKortNavn.StartsWith(begNavn)).Select(ho => konverterHovedstrekning(ho)).ToList();
            }
        }
        public List<Hovedstrekning> HentHovedstrekningerPaNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                return (funnet == null) ? null : funnet.Hovedstrekninger.Select(dbho => konverterHovedstrekning(dbho)).ToList();
            }
        }
        public List<Hovedstrekning> HentHovedstrekningerTilStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : funnet.Hovedstrekninger.Select(dbho => konverterHovedstrekning(dbho)).ToList();
            }
        }
        private Hovedstrekning konverterHovedstrekning(DbHovedstrekning dbho)
        {
            return new Hovedstrekning
            {
                id = dbho.Id,
                hovstr_navn = dbho.HovstrNavn,
                hovstr_kortnavn = dbho.HovstrKortNavn,
                // Droppe denne?:
                stasjon_Ider = dbho.Stasjoner.ToList().Select(st => st.Id).ToList(),
                nett_id = (dbho.Nett != null) ? dbho.Nett.Id : -1
            };
        }

        public Nett HentNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                return (funnet == null) ? null : konverterNett(funnet);
            }
        }
        public List<Nett> HentAlleNett()
        {
            using (var db = new VyDbContext())
            {
                return db.Nett.ToList().Select(dbne => konverterNett(dbne)).ToList();
            }
        }
        public List<Nett> HentNettEtterBegNavn(string begNavn)
        {
            using (var db = new VyDbContext())
            {
                return db.Nett.ToList().Where(ne => ne.Nettnavn.StartsWith(begNavn)).Select(ne => konverterNett(ne)).ToList();
            }
        }
        private Nett konverterNett(DbNett dbne)
        {
            return new Nett
            {
                id = dbne.Id,
                nett_navn = dbne.Nettnavn,
                // Droppe disse?:
                hovedstrekning_Ider = dbne.Hovedstrekninger.Select(hs => hs.Id).ToList(),
                stasjon_Ider = dbne.Stasjoner.Select(st => st.Id).ToList()
            };
        }

        /**
         *  Slutt generelle hentemetoder for poster i basen
         */

        /**
         *  Metoder for a legge til og fjerne poster i basen
         */
        public int leggTilNett(string navn) // returverdien er det nye nettets id, eller -1 hvis innlegging feilet.
        {
            using (var db = new VyDbContext())
            {
                if (db.Nett.FirstOrDefault(ne => navn.Equals(ne.Nettnavn)) == null)
                {
                    DbNett detnye = new DbNett(navn);
                    db.Nett.Add(detnye);
                    db.SaveChanges();
                    return detnye.Id;
                }
            }
            return -1; // Nettnavnet er brukt fra for, (eller baseoppdatering feilet(?))
        }
        public int leggTilNett(Nett nett) // returverdien er det nye nettets id, eller -1 hvis innlegging feilet. Exception ved ugyldige attributter
        {
            using (var db = new VyDbContext())
            {
                if (db.Nett.FirstOrDefault(ne => nett.nett_navn.Equals(ne.Nettnavn)) == null)
                {
                    // Ma sjekke at det tillagte nettet ikke viser til hovedstrekninger 
                    // eller stasjoner som ikke finnes, eller horer til andre nett, eller
                    // har duplikater av hovedstrekninger eller stasjoner:
                    bool feil = false;
                    DbHovedstrekning tmpHov;
                    DbStasjon tmpSta;
                    DbNett detnye = new DbNett(nett.nett_navn);
                    if (nett.hovedstrekning_Ider != null)
                    {   // Feil ved duplikathovedstrekning:
                        if (!(feil = nett.hovedstrekning_Ider.Count() != nett.hovedstrekning_Ider.Distinct().Count()))
                            foreach (int i in nett.hovedstrekning_Ider) // Feil hvis h.strekninger ikke finnes, eller horer til annet nett:
                                if (!(feil |= (tmpHov = db.Hovedstrekninger.Find(i)) == null || tmpHov.Nett != null))
                                {
                                    detnye.Hovedstrekninger.Add(tmpHov);
                                    tmpHov.Nett = detnye;
                                }
                    }
                    if (!feil && nett.stasjon_Ider != null)
                    {   // Feil ved duplikatstasjon:
                        if (!(feil = nett.stasjon_Ider.Count() != nett.stasjon_Ider.Distinct().Count()))
                            foreach (int i in nett.stasjon_Ider) // Feil hvis stasjoner ikke finnes, eller horer til annet nett:
                                if (!(feil |= (tmpSta = db.Stasjoner.Find(i)) == null || tmpSta.Nett != null))
                                {
                                    detnye.Stasjoner.Add(tmpSta);
                                    tmpSta.Nett = detnye;
                                }
                    }
                    if (!feil)
                    {
                        db.Nett.Add(detnye);
                        db.SaveChanges();
                        return detnye.Id;
                    }
                    throw new ArgumentException("nett-objektet har ugyldige data; duplikat(er), ikke-eksisterende stasjon eller hovedstrekning, eller opptatt peker fra stasjon eller hovedstrekning");
                }
                return -1; // Nettnavnet er brukt fra for, (eller baseoppdatering feilet(?))
            }
        }
        public bool fjernNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                if (funnet != null)
                {
                    // Tror det funker bra uten dette bortkommenterte. Hvis det virker uten (dvs. referansene blir borte); fjern det:
                    //foreach (var str in funnet.Hovedstrekninger)
                    //    str.Nett = null; // Ma fjerne referansene til DbNett-et som skal fjernes (Eller ordner EF det? Sjekk)
                    //funnet.Hovedstrekninger.Clear();
                    //foreach (var sta in funnet.Stasjoner)
                    //    sta.Nett = null; // Ma fjerne referansene til DbNett-et som skal fjernes (Eller ordner EF det? Sjekk)
                    //funnet.Stasjoner.Clear();
                    db.Nett.Remove(funnet);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool settNyttNettnavn(int nettId, string nyttNavn)
        {
            using (var db = new VyDbContext())
            {
                DbNett funnet = db.Nett.Find(nettId);
                if (funnet != null && nyttNavn != null && !db.Nett.Any(n => nyttNavn.Equals(n.Nettnavn)))
                {
                    // Tror det skal funke sa enkelt som dette:
                    funnet.Nettnavn = nyttNavn;
                    // Men ellers kan nok dette brukes:
                    //db.Entry(funnet).Property(p => p.Nettnavn).CurrentValue = nyttNavn;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public int leggTilHovedstrekning(string navn, string kortnavn) // returverdien er den nye strekningens id, eller -1 hvis innlegging feilet.
        {
            using (var db = new VyDbContext())
            {
                if (db.Hovedstrekninger.FirstOrDefault(ho => navn.Equals(ho.HovstrNavn)
                    || kortnavn.Equals(ho.HovstrKortNavn)) == null)
                {
                    DbHovedstrekning dennye = new DbHovedstrekning(navn, null, kortnavn);
                    db.Hovedstrekninger.Add(dennye);
                    db.SaveChanges();
                    return dennye.Id;
                }
            }
            return -1; // Navnet eller kortnavnet er brukt fra for, (eller baseoppdatering feilet(?))
        }
        public int leggTilHovedstrekning(Hovedstrekning hovst) // returverdien er den nye strekningens id, eller -1 hvis innlegging feilet.
        {
            using (var db = new VyDbContext())
            {
                if (db.Hovedstrekninger.FirstOrDefault(ho => hovst.hovstr_navn.Equals(ho.HovstrNavn)
                        || hovst.hovstr_kortnavn.Equals(ho.HovstrKortNavn)) == null)
                {
                    // Ma sjekke at den tillagte hovedstrekningen ikke viser til nett 
                    // eller stasjoner som ikke finnes, eller
                    // har ugyldige duplikater av stasjoner:
                    bool feil = false;
                    DbNett tmpNet = null;
                    DbStasjon tmpSta;
                    // Feil hvis angitt nett ikke finnes (men det er lov a angi ikke-nett med nett_id <= 0):
                    // Ev. ikke-nett overstyres av stasjonenes nett, hvis det er entydig (ikke-entydig er en feil, og gir unntak).
                    feil = (hovst.nett_id > 0 && (tmpNet = db.Nett.Find(hovst.nett_id)) == null);
                    if (feil)
                        throw new ArgumentException("hovedstrekning-objektet har ugyldige data; ikke-eksisterende nett angitt");

                    // Sjekker feil mht. stasjoner:
                    if (!feil && hovst.stasjon_Ider != null)
                    {
                        int c = hovst.stasjon_Ider.Count();
                        // Sjekker duplikater, og at hovedstrekningen har minst to stasjoner (hvis den har noen):
                        if (c > 0) // Godtar forste og siste stasjon lik (ringbane), derfor to distinct-sjekker:
                            feil = c < 2 || hovst.stasjon_Ider.Skip(1).Distinct().Count() != c - 1
                                || hovst.stasjon_Ider.Take(c - 1).Distinct().Count() != c - 1;
                        if (feil)
                            throw new ArgumentException("hovedstrekning-objektet har ugyldige data; ulovlige duplikatstasjoner eller ulovlig lengde (1) på ny hovedstrekning");
                        if (!feil && c > 0)
                        {
                            // Godtar som endestasjoner: Utilknyttede og motepunkt-stasjoner. Skjoting med annen strekning tillates ikke
                            // Merk: A knytte en ny ringbane til en endestajon eller motepunkt blir tillatt. Sann skal det vaere.
                            // Ogsa feil hvis stasjoner er pa annet nett enn angitt for denne hovedstrekningen (hvis det er angitt, da),
                            // eller stasjonslista inneholder stasjoner fra forskjellige nett

                            feil = (tmpSta = db.Stasjoner.Find(hovst.stasjon_Ider.First())) == null // Angitt stasjon skal eksistere
                                // Angitt nett skal ikke vaere ulikt annet angitt nett:
                                || (tmpSta.Nett != null && !tmpSta.Nett.Equals((tmpNet != null) ? tmpNet : tmpNet = tmpSta.Nett))
                                || (tmpSta.Hovedstrekninger.Count() == 1 // Feil a tilknytte annet enn ende av andre hovedstrekninger
                                        && !tmpSta.Equals(tmpSta.Hovedstrekninger.First().Stasjoner.First())
                                        && !tmpSta.Equals(tmpSta.Hovedstrekninger.First().Stasjoner.Last()))
                                // Ikke tillatt a skjote ender, unntatt a sette pa en ringbane:
                                || (tmpSta.Hovedstrekninger.Count() == 1 && !hovst.stasjon_Ider.First().Equals(hovst.stasjon_Ider.Last()));

                            feil |= (tmpSta = db.Stasjoner.Find(hovst.stasjon_Ider.Last())) == null
                                || (tmpSta.Nett != null && !tmpSta.Nett.Equals((tmpNet != null) ? tmpNet : tmpNet = tmpSta.Nett))
                                || (tmpSta.Hovedstrekninger.Count() == 1
                                        && !tmpSta.Equals(tmpSta.Hovedstrekninger.First().Stasjoner.First())
                                        && !tmpSta.Equals(tmpSta.Hovedstrekninger.First().Stasjoner.Last()))
                                || (tmpSta.Hovedstrekninger.Count() == 1 && !hovst.stasjon_Ider.First().Equals(hovst.stasjon_Ider.Last()));
                            // Ikke-endestasjoner skal vaere utilknyttede:
                            for (int i = 1; i < c - 1; ++i)
                                feil |= (tmpSta = db.Stasjoner.Find(hovst.stasjon_Ider[i])) == null
                                    || (tmpSta.Nett != null && !tmpSta.Nett.Equals((tmpNet != null) ? tmpNet : tmpNet = tmpSta.Nett))
                                    || tmpSta.Hovedstrekninger.Count() > 0;
                        }
                        if (feil)
                            throw new ArgumentException("hovedstrekning-objektet har ugyldige data; ikke-eksisterende stasjon angitt, tvetydig nett-angivelse eller ulovlig kopling mellom hovedstrekninger");
                    }
                    if (!feil) // Da er alt i orden!
                    {       // Her er tmpNet enten null, angitt av argumentet eller oppfanget under feilsjekkene. Under enhver omstendighet utvetydig:
                        DbHovedstrekning dennye = new DbHovedstrekning(hovst.hovstr_navn, tmpNet, hovst.hovstr_kortnavn);
                        if (hovst.stasjon_Ider != null)
                            foreach (int i in hovst.stasjon_Ider)
                            {
                                tmpSta = db.Stasjoner.Find(i);
                                tmpSta.Nett = tmpNet; // Setter nett pa stasjonene, i tilfelle det ikke er der fra for
                                dennye.Stasjoner.Add(tmpSta);
                                tmpSta.Hovedstrekninger.Add(dennye);
                            }
                        if (tmpNet != null)
                            tmpNet.Hovedstrekninger.Add(dennye);
                        db.Hovedstrekninger.Add(dennye);
                        db.SaveChanges();
                        return dennye.Id;
                    }
                    throw new Exception("Hvis denne kastes er noe feil i koden");
                }
            }
            return -1; // Navnet eller kortnavnet er brukt fra for
        }
        public bool fjernHovedstrekning(int hovstrId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Hovedstrekninger.Find(hovstrId);
                if (funnet != null)
                {
                    // Dette trengs ikke, EF ordner det:
                    //if (funnet.Nett != null)
                    //{
                    //    funnet.Nett.Hovedstrekninger.Remove(funnet); // Ma fjerne referansen til DbHovedstrekning-en som skal fjernes
                    //    funnet.Nett = null; // Fjerner ogsa referansen andre veien
                    //}
                    List<DbStasjon> funnetsStasjoner = funnet.Stasjoner.ToList();
                    foreach (var sta in funnetsStasjoner)
                    {
                        // Ma denne vaere med? Ikke for basen, men ma gjores for at ev. motestasjoner skal fa redusert til to. Det forenkler metoden nedenfor.
                        sta.Hovedstrekninger.Remove(funnet); // Fjerner referansene til DbHovedstrekning-en som skal fjernes
                        // Dette ma vaere med; ef fjerner (heldigvis) ikke disse av seg selv:
                        if (sta.Hovedstrekninger.Count() == 0 && sta.Nett != null) // Fjerner ogsa referansene til/fra nett hvis stasjonen ikke lenger er tilknyttet noe:
                        {
                            sta.Nett.Stasjoner.Remove(sta);
                            sta.Nett = null;
                        }
                    }

                    // Ma sjekke om fjerning av strekning har etterlatt "motepunkt" med to hovedstrekninger. Da ma hovedstrekningene slas sammen
                    
                    // Sjekker forst den ene enden:
                    var skjotes = funnetsStasjoner.First().Hovedstrekninger;
                    if (skjotes.Count() == 2 && skjotes.First() != skjotes.Last()) // skjotes.First() == skjotes.Last() innebaerer en losrevet ringbane
                    {
                        DbStasjon skjotestasjon = funnetsStasjoner.First();
                        DbHovedstrekning astr = skjotes.First();
                        DbHovedstrekning bstr = skjotes.Last();
                        List<DbStasjon> aStList = astr.Stasjoner.ToList();
                        List<DbStasjon> bStList = bstr.Stasjoner.ToList();
                        DbHovedstrekning beholdes;
                        DbHovedstrekning fjernes;
                        List<DbStasjon> nyStList;
                        // Ordner dem slik at siste stasjon i forste == skjotestasjon == forste stasjon i siste:
                        if (aStList.Count > bStList.Count) // Eller en annen mekanisme for a velge hvilken som beholdes?
                        {
                            if (aStList.First().Equals(skjotestasjon))
                                astr.Stasjoner.Reverse();
                            if (bStList.Last().Equals(skjotestasjon))
                                bStList.Reverse();
                            nyStList = bStList;
                            beholdes = astr;
                            fjernes = bstr;
                        }
                        else
                        {
                            if (bStList.First().Equals(skjotestasjon))
                                bstr.Stasjoner.Reverse();
                            if (aStList.Last().Equals(skjotestasjon))
                                aStList.Reverse();
                            nyStList = aStList;
                            beholdes = bstr;
                            fjernes = astr;
                        }
                        // I skjotestasjonen fjernes utgaende hovedstrekning, den gjenvaerende er allerede registrert i stasjonens liste (og tilbake):
                        // Trengs removene her? Sjekk med utskrift. 
                        // Trengs add-ene, eller lages det pekere automatisk etter AddRange? Ja, de trengs.
                        nyStList[0].Hovedstrekninger.Remove(fjernes);
                        for (int i = 1; i < nyStList.Count; ++i)
                        {   // Erstatter utgaende hovedstrekning fra stasjonenes hovedstrekningliste med den nye felles i resten av stasjonene
                            nyStList[i].Hovedstrekninger.Remove(fjernes);
                            nyStList[i].Hovedstrekninger.Add(beholdes);
                        }
                        beholdes.Stasjoner.AddRange(nyStList.Skip(1).ToList()); // Legger stasjoner fra utgaende inn i gjenvaerende (untatt skjotestasjonen, den er der fra for)
                        // Dette trengs ikke,EF ordner det:
                        //if (fjernes.Nett != null)
                        //{
                        //    fjernes.Nett.Hovedstrekninger.Remove(fjernes); // Ma fjerne referansen til DbHovedstrekning-en som utgar
                        //    fjernes.Nett = null; // Fjerner ogsa referansen andre veien
                        //}
                        db.Hovedstrekninger.Remove(fjernes); // fjerner utgaende hovedstrekning fra basen
                    }

                    // Sjekker sa den andre enden:
                    skjotes = funnetsStasjoner.Last().Hovedstrekninger;
                    if (skjotes.Count() == 2 && skjotes.First() != skjotes.Last()) // skjotes.First() == skjotes.Last() innebaerer en losrevet ringbane
                    {
                        DbStasjon skjotestasjon = funnetsStasjoner.Last();
                        DbHovedstrekning astr = skjotes.First();
                        DbHovedstrekning bstr = skjotes.Last();
                        List<DbStasjon> aStList = astr.Stasjoner.ToList();
                        List<DbStasjon> bStList = bstr.Stasjoner.ToList();
                        DbHovedstrekning beholdes;
                        DbHovedstrekning fjernes;
                        List<DbStasjon> nyStList;
                        // Ordner dem slik at siste stasjon i forste == skjotestasjon == forste stasjon i siste:
                        if (aStList.Count > bStList.Count) // Eller en annen mekanisme for a velge hvilken som beholdes?
                        {
                            if (aStList.First().Equals(skjotestasjon))
                                astr.Stasjoner.Reverse();
                            if (bStList.Last().Equals(skjotestasjon))
                                bStList.Reverse();
                            nyStList = bStList;
                            beholdes = astr;
                            fjernes = bstr;
                        }
                        else
                        {
                            if (bStList.First().Equals(skjotestasjon))
                                bstr.Stasjoner.Reverse();
                            if (aStList.Last().Equals(skjotestasjon))
                                aStList.Reverse();
                            nyStList = aStList;
                            beholdes = bstr;
                            fjernes = astr;
                        }
                        // I skjotestasjonen fjernes utgaende hovedstrekning, den gjenvaerende er allerede registrert i stasjonens liste (og tilbake):
                        nyStList[0].Hovedstrekninger.Remove(fjernes);
                        for (int i = 1; i < nyStList.Count; ++i)
                        {   // Erstatter utgaende hovedstrekning fra stasjonenes hovedstrekningliste med den nye felles i resten av stasjonene
                            nyStList[i].Hovedstrekninger.Remove(fjernes);
                            nyStList[i].Hovedstrekninger.Add(beholdes);
                        }
                        beholdes.Stasjoner.AddRange(nyStList.Skip(1).ToList()); // Legger stasjoner fra utgaende inn i gjenvaerende (untatt skjotestasjonen, den er der fra for)
                        db.Hovedstrekninger.Remove(fjernes); // fjerner utgaende hovedstrekning fra basen
                    }

                    // NBNBNB!!! Kan ha blitt splittet til flere nett. Ma gjore noe med det. Bruk stiermellomstasjoner for a sjekke(?)
                    db.Hovedstrekninger.Remove(funnet);
                    db.SaveChanges();

                    return true;
                }
            }
            return false;
        }
        public bool settNyeHovedstrekningNavn(int hovstrId, string nyttNavn, string nyttKortnavn)
        {   // Null-verdier pa navnene betyr a beholde gammelt navn
            using (var db = new VyDbContext())
            {
                DbHovedstrekning funnet = db.Hovedstrekninger.Find(hovstrId);
                if (funnet != null && (nyttNavn != null || nyttKortnavn != null))
                    if (!db.Hovedstrekninger.Any(h => h.HovstrNavn.Equals(nyttNavn) || h.HovstrKortNavn.Equals(nyttKortnavn)))
                    {
                        // Tror det skal funke sa enkelt som dette:
                        if (nyttNavn != null)
                            funnet.HovstrNavn = nyttNavn;
                        if (nyttKortnavn != null)
                            funnet.HovstrKortNavn = nyttKortnavn;
                        // Men ellers kan nok dette brukes:
                        //if (nyttNavn != null)
                        //    db.Entry(funnet).Property(p => p.HovstrNavn).CurrentValue = nyttNavn;
                        //if (nyttKortnavn != null)
                        //    db.Entry(funnet).Property(p => p.HovstrKortNavn).CurrentValue = nyttKortnavn;
                        db.SaveChanges();
                        return true;
                    }
            }
            return false;
        }

        // Merk: disse metodene legger ikke stasjonen inn i en hovedstrekning, men bare registrerer dem i basen.
        // Ev. angitte hovedstrekninger ignoreres.
        // A legge stasjonen inn i en hovedstrekning gjores med andre metoder
        public int leggTilStasjon(string navn, string optSted = "", double optBreddegrad = 0, double optLengdegrad = 0) // returverdien er den nye stasjonens id, eller -1 hvis innlegging feilet.
        {
            using (var db = new VyDbContext())
            {
                if (db.Stasjoner.FirstOrDefault(st => navn.Equals(st.StasjNavn)
                    && st.StasjSted.Equals((optSted == null) ? "" : optSted)) == null)
                {
                    // Koordinatene ma vaere riktige:
                    if (optBreddegrad < -90 | optBreddegrad > 90 | optLengdegrad < -180 | optLengdegrad > 180)
                        throw new ArgumentException("stasjon-objektet har ugyldige data; koordinatene er feil");
                    DbStasjon dennye = new DbStasjon(navn, null, (optSted == null) ? "" : optSted);
                    dennye.Breddegrad = optBreddegrad;
                    dennye.Lengdegrad = optLengdegrad;
                    db.Stasjoner.Add(dennye);
                    db.SaveChanges();
                    return dennye.Id;
                }
            }
            return -1; // Navn- og stedkombinasjonen er brukt fra for
        }
        public int leggTilStasjon(Stasjon stas) // returverdien er den nye stasjonens id, eller -1 hvis innlegging feilet.
        {
            using (var db = new VyDbContext())
            {
                if (db.Stasjoner.FirstOrDefault(st => stas.stasjon_navn.Equals(st.StasjNavn)
                    && st.StasjSted.Equals(stas.stasjon_sted) && ((st.Nett == null) ? stas.nett_id <= 0 : stas.nett_id == st.Nett.Id)) == null)
                {
                    // Ma sjekke at den tillagte stasjonen ikke viser til nett som ikke finnes, og at koordinatene er gyldige:
                    bool feil = false;
                    DbNett tmpNet = null;
                    // Koordinatene ma vaere riktige:
                    feil = stas.breddegrad < -90 | stas.breddegrad > 90 | stas.lengdegrad < -180 | stas.lengdegrad > 180;
                    if (feil)
                        throw new ArgumentException("stasjon-objektet har ugyldige data; koordinatene er feil");
                    // Feil hvis angitt nett ikke finnes (men det er lov, og kanskje lurt, a angi ikke-nett med nett_id <= 0):
                    feil = (stas.nett_id > 0 && (tmpNet = db.Nett.Find(stas.nett_id)) == null);
                    if (feil)
                        throw new ArgumentException("stasjon-objektet har ugyldige data; ikke-eksisterende nett angitt");
                    
                    DbStasjon dennye = new DbStasjon(stas.stasjon_navn, tmpNet, (stas.stasjon_sted == null) ? "" : stas.stasjon_sted);
                    dennye.Breddegrad = stas.breddegrad;
                    dennye.Lengdegrad = stas.lengdegrad;
                    db.Stasjoner.Add(dennye);
                    db.SaveChanges();
                    return dennye.Id;
                }
            }
            return -1; // Navn-, sted- og nettkombinasjonen er brukt fra for
        }
        public bool fjernStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                if (funnet != null)
                {
                    // NBNBNB!!! Kan ha blitt splittet til flere nett. Ma gjore noe med det. Bruk stiermellomstasjoner for a sjekke(?)
                    db.Stasjoner.Remove(funnet);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool settNyeStasjonNavnOgSted(int stasjId, string nyttNavn, string nyttSted)
        {// Null-verdier pa navnene betyr a beholde gammelt navn
            using (var db = new VyDbContext())
            {
                DbStasjon funnet = db.Stasjoner.Find(stasjId);
                if (funnet != null && (nyttNavn != null || nyttSted != null))
                    if (!db.Stasjoner.Any(h => ((nyttNavn == null) || h.StasjNavn.Equals(nyttNavn)) 
                            && ((nyttSted == null) || h.StasjSted.Equals(nyttSted))))
                    {
                        // Tror det skal funke sa enkelt som dette:
                        if (nyttNavn != null)
                            funnet.StasjNavn = nyttNavn;
                        if (nyttSted != null)
                            funnet.StasjSted = nyttSted;
                        // Men ellers kan nok dette brukes:
                        //if (nyttNavn != null)
                            //db.Entry(funnet).Property(p => p.StasjNavn).CurrentValue = nyttNavn;
                        //if (nyttKortnavn != null)
                            //db.Entry(funnet).Property(p => p.StasjSted).CurrentValue = nyttSted;
                        db.SaveChanges();
                        return true;
                    }
            }
            return false;
        }
        public bool settNyeStasjonKoordinater(int stasjId, double breddegrad, double lengdegrad)
        {
            using (var db = new VyDbContext())
            {
                DbStasjon funnet = db.Stasjoner.Find(stasjId);
                if (funnet != null & breddegrad >= -90 & breddegrad <= 90 & lengdegrad >= -180 | lengdegrad <= 180)
                {
                    // Tror det skal funke sa enkelt som dette:
                    funnet.Breddegrad = breddegrad;
                    funnet.Lengdegrad = lengdegrad;
                    // Men ellers kan nok dette brukes:
                    //db.Entry(funnet).Property(p => p.Breddegrad).CurrentValue = breddegrad;
                    //db.Entry(funnet).Property(p => p.Lengdegrad).CurrentValue = lengdegrad;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /**
         *  Slutt metoder for a legge til og fjerne poster i basen
         */


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

        /**
         * Spesialiserte hentemetoder for (lister av) verdier.
         * Bare for hyppige, spesielle eller tidsviktige foresporsler, bruk ellers de generelle
         */
        public List<String> HentAlleStasjonNavn()
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Select(dbst => dbst.StasjNavn).ToList();
            }
        }

        //public List<List<Stasjon>> stierMellomStasjoner(int ida, int idb)
        //{
        //    List<List<Stasjon>> retur = new List<List<Stasjon>>();
        //    // Bruker et Set for raskere oppslag:
        //    ISet<DbHovedstrekning> blinde = new HashSet<DbHovedstrekning>();
        //    List<List<DbHovedstrekning>> strekninger = new List<List<DbHovedstrekning>>(); // ruter fra a til b
        //    IList<DbHovedstrekning> stitilna = new List<DbHovedstrekning>();
        //    DbStasjon a;
        //    DbStasjon b;
        //    //double bbredde, blengde; // For beregning av naermeste sti. Bruk implementeres senere.
        //    IEnumerable<DbHovedstrekning> ahs; // Hovedstrekningen(e) som stasjon a er tilknyttet
        //    IEnumerable<DbHovedstrekning> bhs; // Hovedstrekningen(e) som stasjon a er tilknyttet
        //    IEnumerable<DbHovedstrekning> felleshs; //Hovedstrekningen(e) som a og b begge er tilknyttet

        //    using (var db = new VyDbContext())
        //    {
        //        bool ikkeferdig = true; // hjelpeverdi til metodeflyt

        //        a = db.Stasjoner.Find(ida);
        //        b = db.Stasjoner.Find(idb);
        //        if (a == null || b == null)
        //            return null;
        //        //bbredde = b.Breddegrad; blengde = b.Lengdegrad;

        //        if (a.Id != b.Id) // Ingen sti finnes.
        //            return new List<List<Stasjon>>(); // Returnere null i stedet?

        //        ahs = a.Hovedstrekninger;
        //        bhs = b.Hovedstrekninger;
        //        felleshs = ahs.Intersect(bhs);

        //        // Start- og stoppstasjon er den samme. Det er bare tull, men returnerer en/flere "stier" likevel
        //        if (ida == idb)
        //        {
        //            // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
        //            //foreach (DbHovedstrekning hs in ahs)
        //            //    strekninger.Add(new List<DbHovedstrekning> { hs });
        //            ikkeferdig = false;
        //            return new List<List<Stasjon>> { new List<Stasjon> { konverterStasjon(a) } };
        //        }

        //        // Spesialtilfelle: A og B er pa samme blindbane eller ringbane
        //        else if ((ahs.Count() == 1 || bhs.Count() == 1) && felleshs.Count() == 1)
        //        {
        //            IList<DbStasjon> stasj = felleshs.First().Stasjoner;
        //            if (stasj.First().Hovedstrekninger.Count() == 1 || stasj.Last().Hovedstrekninger.Count() == 1)
        //            {    // Er blindbane. Bare en sti.
        //                 // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
        //                 //strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });

        //                ikkeferdig = false;
        //                return new List<List<Stasjon>> { stasjAtilB(a, b, stasj).Select(st => konverterStasjon(st)).ToList() };
        //            }
        //            else if (stasj.First() == stasj.Last())
        //            {   // Er ringbane. Legger inn de to stiene, en med en hovedstrekning og en med to,
        //                // for a vise at "enden" ma krysses. Gjore det pa annen mate?
        //                //strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });
        //                //strekninger.Add(new List<DbHovedstrekning> { felleshs.First(), felleshs.First() });

        //                int aidx = stasj.IndexOf(a);
        //                int bidx = stasj.IndexOf(b);
        //                int inkr = (aidx < bidx) ? 1 : -1;
        //                int stopp;
        //                List<Stasjon> stListe = new List<Stasjon>((bidx - aidx) * inkr + 1);
        //                for (stopp = bidx + inkr; aidx != stopp; aidx += inkr)
        //                    stListe.Add(konverterStasjon(stasj[aidx]));
        //                List<Stasjon> revListe = new List<Stasjon>(2 + stasj.Count - stListe.Count);
        //                aidx -= stListe.Count * inkr; // setter aidx tilbake
        //                inkr = -inkr; // negerer, for na skal det telles andre veien
        //                for (stopp = (inkr == 1) ? stasj.Count - 1 : 0; aidx != stopp; aidx += inkr)
        //                    revListe.Add(konverterStasjon(stasj[aidx]));
        //                aidx = (inkr == 1) ? 0 : stasj.Count - 1;
        //                for (stopp = bidx + inkr; aidx != stopp; aidx += inkr)
        //                    revListe.Add(konverterStasjon(stasj[aidx]));
        //                ikkeferdig = false;
        //                return new List<List<Stasjon>> { stListe, revListe };
        //            }
        //            //else // ingenting, det er ikke en blind- eller ringbane
        //        }

        //        // 1) Hvis noen av nabohovedstrekningene finnes tidligere i grenen; bitt i halen, ikke ga videre pa noen av hovedstrekningene
        //        // 2) Hvis noen av nabohovedstrekningene har stasjon b er det funnet en sti (av hovedstrekninger). Lagre den, og ikke ga videre pa den hovedstrekningen
        //        // 2b) Hvis noen av nabohovedstrekningene ender blindt (har endestasjon uten forbindelse, eller er ringbaner), ikke ga videre pa de hovedstrekningene.
        //        // 3) Resterende hovedstrekninger traverseres videre.
        //        // Hvis alle nabohovedstrekninger er blinde eller traverseringer returnerer (blind == true) er ogsa denne blind
        //        // Blinde hovedstrekninger markeres i liste og besokes ikke igjen
        //        // NB: traverseringer som ikke har funnet noen sti OG ikke har bitt seg i halen er ogsa a betrakte som blinde.
        //        //  Har ikke lagt inn slik avskjaering enna. Hmm.. dette ma skje allerede.
        //        //  Kan ogsa avskjaere nar stien har vokst betydelig lenger enn allerede funnet sti.
        //        // 

        //        IEnumerable<DbHovedstrekning> nabohs;
        //        List<DbHovedstrekning> traverserfraForste = new List<DbHovedstrekning>();
        //        List<DbHovedstrekning> traverserfraSiste = new List<DbHovedstrekning>();

        //        // A ikke pa knutepunkt. A har da bare en hovedstrekning:
        //        if (ikkeferdig && ahs.Count() == 1)
        //        {
        //            // Litt triksing nedenfor for a hoppe over kode nar a og b pa samme strekning (sammeHs == true)
        //            DbStasjon aforsteSt = null;
        //            DbStasjon asisteSt = null;
        //            bool sammeHs = (felleshs.Count() == 1);
        //            stitilna.Add(ahs.First());
        //            if (sammeHs)
        //            {   // Finner hvilken ende av hovedstrekningen som lista skal bygges fra.
        //                asisteSt = (ahs.First().Stasjoner.IndexOf(a) < ahs.First().Stasjoner.IndexOf(b)) ?
        //                    ahs.First().Stasjoner.First() : ahs.First().Stasjoner.Last();
        //                strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Legger inn fellesstrekningen
        //            }
        //            if (!sammeHs)
        //            {
        //                aforsteSt = ahs.First().Stasjoner.First();
        //                asisteSt = ahs.First().Stasjoner.Last();
        //                nabohs = aforsteSt.Hovedstrekninger.Where(h => h != ahs.First());
        //                foreach (DbHovedstrekning hs in nabohs)
        //                {
        //                    if (bhs.Contains(hs))
        //                    {
        //                        stitilna.Add(hs); // Legger til siste delstrekning for kopiering
        //                        strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
        //                        stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
        //                    }
        //                    else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
        //                                || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
        //                        blinde.Add(hs);
        //                    else
        //                        traverserfraForste.Add(hs);
        //                }
        //            }

        //            nabohs = asisteSt.Hovedstrekninger.Where(h => h != ahs.First());
        //            foreach (DbHovedstrekning hs in nabohs)
        //            {
        //                if (bhs.Contains(hs))
        //                {
        //                    stitilna.Add(hs); // Legger til siste delstrekning for kopiering
        //                    strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
        //                    stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
        //                }
        //                else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
        //                            || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
        //                    blinde.Add(hs);
        //                else
        //                    traverserfraSiste.Add(hs);
        //            }
        //            // Legge inn en prioritering av hva som skal startes med av traverserfraForste eller traverserfraSiste
        //            //  utfra om det er forste eller siste som ligger geografisk naermest b?
        //            // .. og sortere de to med hensyn pa hvilke hovedstrekninger som ligger geografisk naermest B?
        //            if (!sammeHs)
        //            {
        //                nabohs = traverserfraForste;
        //                foreach (DbHovedstrekning hs in nabohs)
        //                {
        //                    stitilna.Add(hs);
        //                    traverser(hs, aforsteSt);
        //                    stitilna.RemoveAt(stitilna.Count - 1);
        //                }
        //                // HVIS EN AV DISSE DROPPES, FUNGERER DEN UTEN A GJORE REKURSJONEN TO GANGER NAR A OG B ER PA SAMME HOVEDSTREKNING
        //            }
        //            // Kan na sette disse blinde. NB! kan ikke gjores for den tilsvarende traverserfraForste, det blir tull
        //            blinde.UnionWith(asisteSt.Hovedstrekninger.Except(bhs));
        //            nabohs = traverserfraSiste;
        //            foreach (DbHovedstrekning hs in nabohs)
        //            {
        //                stitilna.Add(hs);
        //                traverser(hs, asisteSt);
        //                stitilna.RemoveAt(stitilna.Count - 1);
        //            }
        //            ikkeferdig = false;
        //        }

        //        // A pa knutepunkt, b pa samme hovedstrekning fungerer fint:
        //        else if (ikkeferdig)
        //        {
        //            nabohs = ahs;
        //            foreach (DbHovedstrekning hs in nabohs)
        //            {
        //                if (bhs.Contains(hs))
        //                    strekninger.Add(new List<DbHovedstrekning> { hs });
        //                else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
        //                        || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
        //                    blinde.Add(hs);
        //                else
        //                {
        //                    traverserfraForste.Add(hs);
        //                    blinde.Add(hs); // Alle strekninger rundt a som ikke inneholder b kan settes blinde.
        //                }
        //            }
        //            nabohs = traverserfraForste;
        //            foreach (DbHovedstrekning hs in nabohs)
        //            {
        //                stitilna.Add(hs);
        //                traverser(hs, a);
        //                stitilna.RemoveAt(stitilna.Count - 1);
        //            }
        //            ikkeferdig = false;
        //        }

        //        var stierEtterLengde = strekninger.Distinct().GroupBy(li => li.Count).OrderBy(ig => ig.Key).ToList();
        //        int antgrp = stierEtterLengde.Count, g = 0;
        //        List<DbStasjon>[] starter;  // Alltid lengde == 1 eller 2 
        //        List<DbStasjon>[] stopper;  // Alltid lengde == 1 eller 2

        //        if (g < antgrp && stierEtterLengde[g].Key == 0) // Det skal ikke vaere noen med lengde 0 her. I sa fall er noe feil
        //        {
        //            throw new Exception("Feil i stifinningen");
        //        }
        //        if (g < antgrp && stierEtterLengde[g].Key == 1) // Det skal vaere maks 1 med lengde 1 her. Blir det flere er det noe feil
        //        {
        //            foreach (IList<DbHovedstrekning> lenen in stierEtterLengde[g]) // Droppe lokka nar det er konstatert at dette virker riktig
        //                retur.Add(stasjAtilB(a, b, lenen[0].Stasjoner).Select(st => konverterStasjon(st)).ToList());
        //            retur.Add(stasjAtilB(a, b, stierEtterLengde[g].First()[0].Stasjoner).Select(st => konverterStasjon(st)).ToList());
        //            ++g;
        //        }
        //        if (g < antgrp && stierEtterLengde[g].Key == 2) // Det skal vaere maks 4 med lengde 2 her. Blir det flere er det noe feil
        //        {
        //            IList<DbStasjon> forste, siste;
        //            foreach (IList<DbHovedstrekning> lento in stierEtterLengde[g])
        //            {
        //                forste = lento[0].Stasjoner;
        //                siste = lento[1].Stasjoner;
        //                if (erEndelike(forste, siste))
        //                { // To stier
        //                    starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
        //                    stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
        //                    starter[0].RemoveAt(starter[0].Count - 1);
        //                    starter[1].RemoveAt(starter[1].Count - 1);
        //                    if (forste.First() == siste.First())
        //                    {
        //                        retur.Add(starter[0].Concat(stopper[0]).Select(st => konverterStasjon(st)).ToList());
        //                        retur.Add(starter[1].Concat(stopper[1]).Select(st => konverterStasjon(st)).ToList());
        //                    }
        //                    else
        //                    {
        //                        retur.Add(starter[0].Concat(stopper[1]).Select(st => konverterStasjon(st)).ToList());
        //                        retur.Add(starter[1].Concat(stopper[0]).Select(st => konverterStasjon(st)).ToList());
        //                    }
        //                }
        //                else
        //                {
        //                    if (erRingbane(forste))
        //                        starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
        //                    else
        //                        starter = new List<DbStasjon>[] { stasjAtilB(a, fellesende(forste, siste), forste) };
        //                    if (erRingbane(siste))
        //                        stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
        //                    else
        //                        stopper = new List<DbStasjon>[] { stasjAtilB(fellesende(forste, siste), b, siste) };
        //                    foreach (List<DbStasjon> stli in starter)
        //                    {
        //                        stli.RemoveAt(stli.Count - 1);
        //                        foreach (List<DbStasjon> stlii in stopper)
        //                            retur.Add(stli.Concat(stlii).Select(st => konverterStasjon(st)).ToList());
        //                    };
        //                }
        //            }
        //            ++g;
        //        }
        //        if (g < antgrp && stierEtterLengde[g].Key == 3)
        //        {
        //            IList<DbStasjon> forste, andre, siste;
        //            DbStasjon veimerke;
        //            List<DbStasjon> midtstykke;
        //            List<DbStasjon> ret = new List<DbStasjon>();
        //            foreach (IList<DbHovedstrekning> lentre in stierEtterLengde[g])
        //            {
        //                forste = lentre[0].Stasjoner;
        //                andre = lentre[1].Stasjoner;
        //                siste = lentre[2].Stasjoner;
        //                if (erEndelike(forste, andre) && erEndelike(andre, siste)) // To stier; "sikk-sakk"
        //                {
        //                    starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
        //                    stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
        //                    starter[0].RemoveAt(starter[0].Count - 1);
        //                    starter[1].RemoveAt(starter[1].Count - 1);

        //                    ret.Clear();
        //                    ret.AddRange(starter[0]);
        //                    veimerke = (forste.First() == andre.First()) ? andre.First() : andre.Last();
        //                    ret.AddRange(heleFraEndeUSiste(veimerke, andre));
        //                    veimerke = motsattEnde(veimerke, andre);
        //                    ret.AddRange((veimerke == siste.First()) ? stopper[0] : stopper[1]);
        //                    retur.Add(ret.Select(st => konverterStasjon(st)).ToList());

        //                    ret.Clear();
        //                    ret.AddRange(starter[1]);
        //                    veimerke = (forste.Last() == andre.First()) ? andre.First() : andre.Last();
        //                    ret.AddRange(heleFraEndeUSiste(veimerke, andre));
        //                    veimerke = motsattEnde(veimerke, andre);
        //                    ret.AddRange((veimerke == siste.First()) ? stopper[0] : stopper[1]);
        //                    retur.Add(ret.Select(st => konverterStasjon(st)).ToList());
        //                }
        //                else
        //                {
        //                    if (erRingbane(forste))
        //                        starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
        //                    else
        //                    {
        //                        veimerke = (erEndelike(forste, andre)) ? motsattEnde(fellesende(andre, siste), andre) : fellesende(forste, andre);
        //                        starter = new List<DbStasjon>[] { stasjAtilB(a, veimerke, forste) };
        //                    }
        //                    if (erRingbane(siste))
        //                        stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
        //                    else
        //                    {
        //                        veimerke = (erEndelike(siste, andre)) ? motsattEnde(fellesende(andre, forste), andre) : fellesende(siste, andre);
        //                        stopper = new List<DbStasjon>[] { stasjAtilB(veimerke, b, siste) };
        //                    }

        //                    veimerke = (erEndelike(forste, andre)) ? motsattEnde(fellesende(andre, siste), andre) : fellesende(forste, andre);
        //                    midtstykke = heleFraEndeUSiste(veimerke, andre);

        //                    foreach (List<DbStasjon> stli in starter)
        //                    {
        //                        stli.RemoveAt(stli.Count - 1);
        //                        foreach (List<DbStasjon> stlii in stopper)
        //                            retur.Add(stli.Concat(midtstykke).Concat(stlii).Select(st => konverterStasjon(st)).ToList());
        //                    }
        //                }
        //            }
        //            ++g;
        //        }
        //        while (g < antgrp) // Key >= 4
        //        {
        //            IList<DbStasjon> forste, andre, nestsiste, siste;
        //            DbStasjon veimerke;
        //            List<DbStasjon> midtstykke;
        //            foreach (IList<DbHovedstrekning> lenx in stierEtterLengde[g])
        //            {
        //                forste = lenx[0].Stasjoner;
        //                andre = lenx[1].Stasjoner;
        //                nestsiste = lenx[lenx.Count - 2].Stasjoner;
        //                siste = lenx[lenx.Count - 1].Stasjoner;

        //                if (erRingbane(forste))
        //                    starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
        //                else
        //                {
        //                    veimerke = motsattEnde(fellesende(andre, lenx[2].Stasjoner), andre);
        //                    starter = new List<DbStasjon>[] { stasjAtilB(a, veimerke, forste) };
        //                }
        //                if (erRingbane(siste))
        //                    stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
        //                else
        //                {
        //                    veimerke = motsattEnde(fellesende(nestsiste, lenx[lenx.Count - 3].Stasjoner), nestsiste);
        //                    stopper = new List<DbStasjon>[] { stasjAtilB(veimerke, b, siste) };
        //                }

        //                veimerke = motsattEnde(fellesende(andre, lenx[2].Stasjoner), andre);
        //                midtstykke = new List<DbStasjon>();
        //                for (int i = 1; i < lenx.Count - 1; ++i)
        //                {
        //                    midtstykke.AddRange(heleFraEndeUSiste(veimerke, lenx[i].Stasjoner));
        //                    veimerke = motsattEnde(veimerke, lenx[i].Stasjoner);
        //                }

        //                foreach (List<DbStasjon> stli in starter)
        //                {
        //                    stli.RemoveAt(stli.Count - 1);
        //                    foreach (List<DbStasjon> stlii in stopper)
        //                        retur.Add(stli.Concat(midtstykke).Concat(stlii).Select(st => konverterStasjon(st)).ToList());
        //                }
        //            }
        //            ++g;
        //        }
        //    }
        //    return retur;

        //    bool traverser(DbHovedstrekning hovstr, DbStasjon feilEnde)
        //    {
        //        DbStasjon rettEnde = ((rettEnde = hovstr.Stasjoner.First()) == feilEnde) ? hovstr.Stasjoner.Last() : rettEnde;
        //        if (rettEnde == a || rettEnde == feilEnde) // Da har soket bitt seg i halen (starten), eller er en ringbane.
        //        {
        //            blinde.Add(hovstr); // Merk den som blind
        //            return true;
        //        }
        //        IEnumerable<DbHovedstrekning> nabohs = rettEnde.Hovedstrekninger.Where(hs => hs != hovstr);
        //        if (nabohs.Count() == 0) // Strekningen er et blindspor.
        //        {
        //            blinde.Add(hovstr); // Merk den som blind
        //            return true;
        //        }
        //        if (nabohs.Count(hs => stitilna.Contains(hs)) >= 2) // Da har soket bitt seg i halen, og er ikke blind/ring
        //            return false;

        //        bool blind = true;
        //        List<DbHovedstrekning> traverserVidere = new List<DbHovedstrekning>();
        //        DbHovedstrekning funnetStr = null;
        //        foreach (DbHovedstrekning hs in nabohs)
        //        {
        //            if (bhs.Contains(hs)) // Da er det funnet en sti
        //            {
        //                stitilna.Add(hs); // Legger til siste delstrekning for kopiering
        //                strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
        //                stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
        //                funnetStr = hs;
        //                blind = false;
        //            }
        //            else if (!blinde.Contains(hs))
        //                traverserVidere.Add(hs);
        //        }

        //        // Sortere traverserVidere med hensyn pa hvilke hovedstrekninger som ligger geografisk naermest B?
        //        nabohs = traverserVidere;
        //        if (funnetStr == null || !ahs.Contains(funnetStr))
        //        {   // Dropper traversering nar det er umulig a na fram
        //            foreach (DbHovedstrekning hs in nabohs)
        //            {
        //                stitilna.Add(hs);
        //                if (!traverser(hs, rettEnde))
        //                    blind = false;
        //                stitilna.RemoveAt(stitilna.Count - 1);
        //            }
        //        }

        //        // Hvis ingenting har fatt denne til a ikke vaere blind, merk den som blind
        //        if (blind)
        //            blinde.Add(hovstr);
        //        return blind;
        //    } // Slutt traverser()

        //    bool erRingbane(IEnumerable<DbStasjon> erring)
        //    {
        //        return erring.First() == erring.Last();
        //    }
        //    bool erEndelike(IEnumerable<DbStasjon> lia, IEnumerable<DbStasjon> lib)
        //    {
        //        return (lia.First() == lib.First() && lia.Last() == lib.Last())
        //            || (lia.First() == lib.Last() && lia.Last() == lib.First());
        //    }
        //    List<DbStasjon>[] fraTilBeggeEnder(DbStasjon fra, IList<DbStasjon> dbStasjList)
        //    {
        //        int i, idx = dbStasjList.IndexOf(fra);
        //        List<DbStasjon> stListe = new List<DbStasjon>(idx + 1);
        //        List<DbStasjon> stListe2 = new List<DbStasjon>(dbStasjList.Count - idx);
        //        for (i = idx; i > -1; --i)
        //            stListe.Add(dbStasjList[i]);
        //        for (i = dbStasjList.Count; idx < i; ++idx)
        //            stListe2.Add(dbStasjList[idx]);
        //        return new List<DbStasjon>[] { stListe, stListe2 };
        //    }
        //    List<DbStasjon>[] fraBeggeEnderTil(IList<DbStasjon> dbStasjList, DbStasjon til)
        //    {
        //        int i, idx = dbStasjList.IndexOf(til);
        //        List<DbStasjon> stListe = new List<DbStasjon>(idx + 1);
        //        List<DbStasjon> stListe2 = new List<DbStasjon>(dbStasjList.Count - idx);
        //        for (i = 0; i <= idx; ++i)
        //            stListe.Add(dbStasjList[i]);
        //        for (i = dbStasjList.Count - 1; i >= idx; --i)
        //            stListe2.Add(dbStasjList[i]);
        //        return new List<DbStasjon>[] { stListe, stListe2 };
        //    }
        //    List<DbStasjon> stasjAtilB(DbStasjon aDbSt, DbStasjon bDbSt, IList<DbStasjon> dbStasjList)
        //    {
        //        int aidx = dbStasjList.IndexOf(aDbSt);
        //        int bidx = dbStasjList.IndexOf(bDbSt);
        //        int inkr = (aidx < bidx) ? 1 : -1;
        //        List<DbStasjon> stListe = new List<DbStasjon>(((inkr == 1) ? bidx - aidx : aidx - bidx) + 1);
        //        for (bidx += inkr; aidx != bidx; aidx += inkr)
        //            stListe.Add(dbStasjList[aidx]);
        //        return stListe;
        //    }
        //    List<DbStasjon> heleFraEndeUSiste(DbStasjon ende, IList<DbStasjon> dbStasjList)
        //    {
        //        if (ende == dbStasjList.First())
        //            return dbStasjList.Take(dbStasjList.Count - 1).ToList();
        //        else if (ende == dbStasjList.Last())
        //            return dbStasjList.Skip(1).Reverse().ToList();
        //        return null;
        //    }
        //    DbStasjon motsattEnde(DbStasjon ende, IEnumerable<DbStasjon> dbStasjList)
        //    {
        //        return (ende == dbStasjList.First()) ? dbStasjList.Last() : (ende == dbStasjList.Last()) ? dbStasjList.First() : null;
        //    }
        //    DbStasjon fellesende(IEnumerable<DbStasjon> lia, IEnumerable<DbStasjon> lib)
        //    {
        //        return (lia.First() == lib.First() || lia.First() == lib.Last()) ? lia.First()
        //            : (lia.Last() == lib.Last() || lia.Last() == lib.First()) ? lia.Last() : null;
        //    }
        //    List<DbStasjon> heleFraEnde(DbStasjon ende, IList<DbStasjon> dbStasjList)
        //    {
        //        if (ende == dbStasjList.First())
        //            return dbStasjList.Take(dbStasjList.Count).ToList();
        //        else if (ende == dbStasjList.Last())
        //            return dbStasjList.Skip(0).Reverse().ToList();
        //        return null;
        //    }
        //    List<Stasjon> konvHeleFraEnde(DbStasjon ende, IList<DbStasjon> dbStasjList)
        //    {
        //        if (ende == dbStasjList.First())
        //            return dbStasjList.Select(st => konverterStasjon(st)).ToList();
        //        else if (ende == dbStasjList.Last())
        //            return dbStasjList.Select(st => konverterStasjon(st)).Reverse().ToList();
        //        return null;
        //    }
        //    List<Stasjon> KonvHeleFraEndeUSiste(DbStasjon ende, IList<DbStasjon> dbStasjList)
        //    {
        //        List<Stasjon> ret = null;
        //        if (ende == dbStasjList.First())
        //        {
        //            ret = dbStasjList.Select(st => konverterStasjon(st)).ToList();
        //            ret.RemoveAt(ret.Count - 1);
        //        }
        //        else if (ende == dbStasjList.Last())
        //        {
        //            ret = dbStasjList.Select(st => konverterStasjon(st)).Reverse().ToList();
        //            ret.RemoveAt(ret.Count - 1);
        //        }
        //        return ret;
        //    }

        //}

        /**
         * Slutt spesialiserte hentemetoder
         */

        /** 
         * Metoder for å legge til eksempeldata
         */
        public void ByggBanedata()
        {
            using (var db = new VyDbContext())
            {
                //try
                {
                    Dictionary<string, DbStasjon> stasjDict = new Dictionary<string, DbStasjon>();
                    List<CSVstasjon> stasjliste = CSVstasjon.convertEngine();
                    IEnumerable<string> nettnavner = stasjliste.Select(st => st.nettnavn).Distinct();
                    List<DbNett> netter = new List<DbNett>(nettnavner.Count());
                    foreach (string n in nettnavner)
                        netter.Add(new DbNett(n));
                    db.Nett.AddRange(netter);
                    var grupper = stasjliste.GroupBy(st => st.ns2banekortnavn);
                    foreach (var grp in grupper)
                    {
                        DbNett grpNett = netter.Single(nt => nt.Nettnavn.Equals(grp.First().nettnavn));
                        DbHovedstrekning hovst = new DbHovedstrekning(grp.First().ns2banenavn, null, grp.First().ns2banekortnavn);
                        db.Hovedstrekninger.Add(hovst);
                        grpNett.Hovedstrekninger.Add(hovst);
                        List<CSVstasjon> csvstasjList = grp.OrderBy(csv => csv.ns2sporkilometer).ToList();

                        foreach (CSVstasjon st in csvstasjList)
                        {
                            DbStasjon stasj;
                            if (!stasjDict.TryGetValue(st.ns1id2, out stasj))
                            {
                                stasj = new DbStasjon(st.ns2stasjonsnavn, null);
                                stasj.Breddegrad = st.breddegrad;
                                stasj.Lengdegrad = st.lengdegrad;
                                stasjDict.Add(st.ns1id2, stasj);
                                db.Stasjoner.Add(stasj);
                                grpNett.Stasjoner.Add(stasj);
                            }
                            stasj.Hovedstrekninger.Add(hovst);
                            hovst.Stasjoner.Add(stasj);
                            //db.HovstrStasj.Add(new DbHovedstrekningStasjon(hovst, stasj, stasjlopnr += 100));
                        }
                    }
                    db.SaveChanges();
                }
                ;
                //catch (Exception ex)
                //{
                //    throw ex; // Ma nesten kaste den videre.
                //}
            }
            ;
            using (var db = new VyDbContext())
            {
                var hshs = db.Hovedstrekninger.ToList();
                ;
                foreach (DbHovedstrekning h in hshs)
                {
                    var sli = h.Stasjoner.ToList();
                    //Debug.WriteLine(sli.First().StasjNavn + "  " + sli.Last().StasjNavn);
                    //for (int i = 0; i < sli.Count; ++i)
                    //    Debug.WriteLine("Hovstr: " + h.HovstrNavn + "  stasjon: " + sli[i].Id + "  " + sli[i].StasjNavn);
                    Debug.WriteLine(sli.First().StasjNavn + "  " + sli.Last().StasjNavn);
                    for (int i = 0; i < sli.Count; ++i)
                        Debug.WriteLine("Hovstr: " + h.HovstrNavn + "  stasjon: " + sli[i].Id + "  " + sli[i].StasjNavn);
                }
            }
            ;
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
        
        public void addPris()
        {
            using (var db = new VyDbContext())
            {
                DbPris dbp = new DbPris
                {
                    Id = 1,
                    prisPrKm = 3
                };
                db.Pris.Add(dbp);
                db.SaveChanges();
            }
        }

        /** 
         * Slutt metoder for å legge til eksempeldata
         */
    }
}