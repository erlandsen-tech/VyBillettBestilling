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
        public List<Rute> HentRute(int start, int stopp, DateTime starttid)
        {
            //TODO
            //Må oppdateres med stifinning.
            VyDbContext db = new VyDbContext();
            {
                return db.Ruter.Where(dbrt => dbrt.Start_id == start && dbrt.DateTime > starttid).Select(dbrt => new Rute
                {
                    Id = dbrt.RuteID,
                    Start_id = dbrt.Start_id,
                    Stopp_id = stopp,
                    DateTime = dbrt.DateTime
                }).ToList();
            }
        }

        // Generelle hentemetoder for (konverterte) poster i basen
        // (Blir utvidet med metoder for de fleste resterende tabellene)
        public Stasjon HentStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : konverterStasjon(funnet);
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
        public List<Stasjon> HentStasjonerPaNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                return (funnet == null) ? null : funnet.Stasjoner.ToList().Select(dbst => konverterStasjon(dbst)).ToList();

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
        public List<Stasjon> HentStasjonerEtterBegNavn(String begNavn)
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Where(st => st.StasjNavn.StartsWith(begNavn)).Select(st => konverterStasjon(st)).ToList();
            }
        }
        private Stasjon konverterStasjon(DbStasjon dbst)
        {
            return new Stasjon
            {
                id = dbst.StasjonId,
                stasjon_navn = dbst.StasjNavn,
                stasjon_sted = dbst.StasjSted,
                breddegrad = dbst.Breddegrad,
                lengdegrad = dbst.Lengdegrad,
                // Droppe denne?:
                hovedstrekninger = dbst.Hovedstrekninger.ToList().Select(hs => hs.HovstrId).ToList(),
                nett_id = dbst.NettId,
                nett_navn = dbst.Nett.Nettnavn
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
        public List<Hovedstrekning> HentHovedstrekningerPaNett(int nettId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Nett.Find(nettId);
                return (funnet == null) ? null : funnet.Hovedstrekninger.ToList().Select(dbho => konverterHovedstrekning(dbho)).ToList();
            }
        }
        public List<Hovedstrekning> HentHovedstrekningerTilStasjon(int stasjId)
        {
            using (var db = new VyDbContext())
            {
                var funnet = db.Stasjoner.Find(stasjId);
                return (funnet == null) ? null : funnet.Hovedstrekninger.ToList().Select(dbho => konverterHovedstrekning(dbho)).ToList();
            }
        }
        private Hovedstrekning konverterHovedstrekning(DbHovedstrekning dbho)
        {
            return new Hovedstrekning
            {
                id = dbho.HovstrId,
                // Droppe denne?:
                stasjoner = dbho.Stasjoner.ToList().Select(st => st.StasjonId).ToList(),
                nett_id = dbho.NettId,
                nett_navn = dbho.Nett.Nettnavn,
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
        private Nett konverterNett(DbNett dbne)
        {
            return new Nett
            {
                id = dbne.NettId,
                nett_navn = dbne.Nettnavn,
                // Droppe disse?:
                hovedstrekninger = dbne.Hovedstrekninger.ToList().Select(hs => hs.HovstrId).ToList(),
                stasjoner = dbne.Stasjoner.ToList().Select(st => st.StasjonId).ToList(),
            };
        }
        // Slutt generelle hentemetoder for poster i basen

        // Spesialiserte hentemetoder for (lister av) verdier.
        // Bare for hyppige eller spesielt tidsviktige foresporsler, bruk ellers de generelle

        public List<String> HentAlleStasjonNavn()
        {
            using (var db = new VyDbContext())
            {
                return db.Stasjoner.ToList().Select(dbst => dbst.StasjNavn).ToList();
            }
        }
        
        public List<List<Stasjon>> stierMellomStasjoner(int ida, int idb)
        {
            List<List<Stasjon>> retur = new List<List<Stasjon>>();
            // Bruker et Set for raskere oppslag:
            ISet<DbHovedstrekning> blinde = new HashSet<DbHovedstrekning>();
            List<List<DbHovedstrekning>> strekninger = new List<List<DbHovedstrekning>>(); // ruter fra a til b
            IList<DbHovedstrekning> stitilna = new List<DbHovedstrekning>();
            DbStasjon a;
            DbStasjon b;
            //double bbredde, blengde; // For beregning av naermeste sti. Bruk implementeres senere.
            IEnumerable<DbHovedstrekning> ahs; // Hovedstrekningen(e) som stasjon a er tilknyttet
            IEnumerable<DbHovedstrekning> bhs; // Hovedstrekningen(e) som stasjon a er tilknyttet
            IEnumerable<DbHovedstrekning> felleshs; //Hovedstrekningen(e) som a og b begge er tilknyttet

            using (var db = new VyDbContext())
            {
                bool ikkeferdig = true; // hjelpeverdi til metodeflyt

                a = db.Stasjoner.Find(ida);
                b = db.Stasjoner.Find(idb);
                if (a == null || b == null)
                    return null;
                //bbredde = b.Breddegrad; blengde = b.Lengdegrad;

                if (a.NettId != b.NettId) // Ingen sti finnes.
                    return new List<List<Stasjon>>(); // Returnere null i stedet?

                ahs = a.Hovedstrekninger;
                bhs = b.Hovedstrekninger;
                felleshs = ahs.Intersect(bhs);

                // Start- og stoppstasjon er den samme. Det er bare tull, men returnerer en/flere "stier" likevel
                if (ida == idb)
                {
                    // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
                    //foreach (DbHovedstrekning hs in ahs)
                    //    strekninger.Add(new List<DbHovedstrekning> { hs });
                    ikkeferdig = false;
                    return new List<List<Stasjon>> { new List<Stasjon> { konverterStasjon(a) } };
                }

                // Spesialtilfelle: A og B er pa samme blindbane eller ringbane
                else if ((ahs.Count() == 1 || bhs.Count() == 1) && felleshs.Count() == 1)
                {
                    IList<DbStasjon> stasj = felleshs.First().Stasjoner;
                    if (stasj.First().Hovedstrekninger.Count() == 1 || stasj.Last().Hovedstrekninger.Count() == 1)
                    {    // Er blindbane. Bare en sti.
                         // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
                         //strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });

                        ikkeferdig = false;
                        return new List<List<Stasjon>> { stasjAtilB(a, b, stasj).Select(st => konverterStasjon(st)).ToList() };
                    }
                    else if (stasj.First() == stasj.Last())
                    {   // Er ringbane. Legger inn de to stiene, en med en hovedstrekning og en med to,
                        // for a vise at "enden" ma krysses. Gjore det pa annen mate?
                        //strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });
                        //strekninger.Add(new List<DbHovedstrekning> { felleshs.First(), felleshs.First() });

                        int aidx = stasj.IndexOf(a);
                        int bidx = stasj.IndexOf(b);
                        int inkr = (aidx < bidx) ? 1 : -1;
                        int stopp;
                        List<Stasjon> stListe = new List<Stasjon>((bidx - aidx) * inkr + 1);
                        for (stopp = bidx + inkr; aidx != stopp; aidx += inkr)
                            stListe.Add(konverterStasjon(stasj[aidx]));
                        List<Stasjon> revListe = new List<Stasjon>(2 + stasj.Count - stListe.Count);
                        aidx -= stListe.Count * inkr; // setter aidx tilbake
                        inkr = -inkr; // negerer, for na skal det telles andre veien
                        for (stopp = (inkr == 1) ? stasj.Count - 1 : 0; aidx != stopp; aidx += inkr)
                            revListe.Add(konverterStasjon(stasj[aidx]));
                        aidx = (inkr == 1) ? 0 : stasj.Count - 1;
                        for (stopp = bidx + inkr; aidx != stopp; aidx += inkr)
                            revListe.Add(konverterStasjon(stasj[aidx]));
                        ikkeferdig = false;
                        return new List<List<Stasjon>> { stListe, revListe };
                    }
                    //else // ingenting, det er ikke en blind- eller ringbane
                }

                // 1) Hvis noen av nabohovedstrekningene finnes tidligere i grenen; bitt i halen, ikke ga videre pa noen av hovedstrekningene
                // 2) Hvis noen av nabohovedstrekningene har stasjon b er det funnet en sti (av hovedstrekninger). Lagre den, og ikke ga videre pa den hovedstrekningen
                // 2b) Hvis noen av nabohovedstrekningene ender blindt (har endestasjon uten forbindelse, eller er ringbaner), ikke ga videre pa de hovedstrekningene.
                // 3) Resterende hovedstrekninger traverseres videre.
                // Hvis alle nabohovedstrekninger er blinde eller traverseringer returnerer (blind == true) er ogsa denne blind
                // Blinde hovedstrekninger markeres i liste og besokes ikke igjen
                // NB: traverseringer som ikke har funnet noen sti OG ikke har bitt seg i halen er ogsa a betrakte som blinde.
                //  Har ikke lagt inn slik avskjaering enna. Hmm.. dette ma skje allerede.
                //  Kan ogsa avskjaere nar stien har vokst betydelig lenger enn allerede funnet sti.
                // 

                IEnumerable<DbHovedstrekning> nabohs;
                List<DbHovedstrekning> traverserfraForste = new List<DbHovedstrekning>();
                List<DbHovedstrekning> traverserfraSiste = new List<DbHovedstrekning>();

                // A ikke pa knutepunkt. A har da bare en hovedstrekning:
                if (ikkeferdig && ahs.Count() == 1)
                {
                    // Litt triksing nedenfor for a hoppe over kode nar a og b pa samme strekning (sammeHs == true)
                    DbStasjon aforsteSt = null;
                    DbStasjon asisteSt = null;
                    bool sammeHs = (felleshs.Count() == 1);
                    stitilna.Add(ahs.First());
                    if (sammeHs)
                    {   // Finner hvilken ende av hovedstrekningen som lista skal bygges fra.
                        asisteSt = (ahs.First().Stasjoner.IndexOf(a) < ahs.First().Stasjoner.IndexOf(b)) ?
                            ahs.First().Stasjoner.First() : ahs.First().Stasjoner.Last();
                        strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Legger inn fellesstrekningen
                    }
                    if (!sammeHs)
                    {
                        aforsteSt = ahs.First().Stasjoner.First();
                        asisteSt = ahs.First().Stasjoner.Last();
                        nabohs = aforsteSt.Hovedstrekninger.Where(h => h != ahs.First());
                        foreach (DbHovedstrekning hs in nabohs)
                        {
                            if (bhs.Contains(hs))
                            {
                                stitilna.Add(hs); // Legger til siste delstrekning for kopiering
                                strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
                                stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
                            }
                            else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
                                        || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
                                blinde.Add(hs);
                            else
                                traverserfraForste.Add(hs);
                        }
                    }

                    nabohs = asisteSt.Hovedstrekninger.Where(h => h != ahs.First());
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        if (bhs.Contains(hs))
                        {
                            stitilna.Add(hs); // Legger til siste delstrekning for kopiering
                            strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
                            stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
                        }
                        else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
                                    || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
                            blinde.Add(hs);
                        else
                            traverserfraSiste.Add(hs);
                    }
                    // Legge inn en prioritering av hva som skal startes med av traverserfraForste eller traverserfraSiste
                    //  utfra om det er forste eller siste som ligger geografisk naermest b?
                    // .. og sortere de to med hensyn pa hvilke hovedstrekninger som ligger geografisk naermest B?
                    if (!sammeHs)
                    {
                        nabohs = traverserfraForste;
                        foreach (DbHovedstrekning hs in nabohs)
                        {
                            stitilna.Add(hs);
                            traverser(hs, aforsteSt);
                            stitilna.RemoveAt(stitilna.Count - 1);
                        }
                        // HVIS EN AV DISSE DROPPES, FUNGERER DEN UTEN A GJORE REKURSJONEN TO GANGER NAR A OG B ER PA SAMME HOVEDSTREKNING
                    }
                    // Kan na sette disse blinde. NB! kan ikke gjores for den tilsvarende traverserfraForste, det blir tull
                    blinde.UnionWith(asisteSt.Hovedstrekninger.Except(bhs));
                    nabohs = traverserfraSiste;
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        stitilna.Add(hs);
                        traverser(hs, asisteSt);
                        stitilna.RemoveAt(stitilna.Count - 1);
                    }
                    ikkeferdig = false;
                }

                // A pa knutepunkt, b pa samme hovedstrekning fungerer fint:
                else if (ikkeferdig)
                {
                    nabohs = ahs;
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        if (bhs.Contains(hs))
                            strekninger.Add(new List<DbHovedstrekning> { hs });
                        else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
                                || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
                            blinde.Add(hs);
                        else
                        {
                            traverserfraForste.Add(hs);
                            blinde.Add(hs); // Alle strekninger rundt a som ikke inneholder b kan settes blinde.
                        }
                    }
                    nabohs = traverserfraForste;
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        stitilna.Add(hs);
                        traverser(hs, a);
                        stitilna.RemoveAt(stitilna.Count - 1);
                    }
                    ikkeferdig = false;
                }

                var stierEtterLengde = strekninger.Distinct().GroupBy(li => li.Count).OrderBy(ig => ig.Key).ToList();
                int antgrp = stierEtterLengde.Count, g = 0;
                List<DbStasjon>[] starter;  // Alltid lengde == 1 eller 2 
                List<DbStasjon>[] stopper;  // Alltid lengde == 1 eller 2

                if (g < antgrp && stierEtterLengde[g].Key == 0) // Det skal ikke vaere noen med lengde 0 her. I sa fall er noe feil
                {
                    throw new Exception("Feil i stifinningen");
                }
                if (g < antgrp && stierEtterLengde[g].Key == 1) // Det skal vaere maks 1 med lengde 1 her. Blir det flere er det noe feil
                {
                    foreach (IList<DbHovedstrekning> lenen in stierEtterLengde[g]) // Droppe lokka nar det er konstatert at dette virker riktig
                        retur.Add(stasjAtilB(a, b, lenen[0].Stasjoner).Select(st => konverterStasjon(st)).ToList());
                    retur.Add(stasjAtilB(a, b, stierEtterLengde[g].First()[0].Stasjoner).Select(st => konverterStasjon(st)).ToList());
                    ++g;
                }
                if (g < antgrp && stierEtterLengde[g].Key == 2) // Det skal vaere maks 4 med lengde 2 her. Blir det flere er det noe feil
                {
                    IList<DbStasjon> forste, siste;
                    foreach (IList<DbHovedstrekning> lento in stierEtterLengde[g])
                    {
                        forste = lento[0].Stasjoner;
                        siste = lento[1].Stasjoner;
                        if (erEndelike(forste, siste))
                        { // To stier
                            starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
                            stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
                            starter[0].RemoveAt(starter[0].Count - 1);
                            starter[1].RemoveAt(starter[1].Count - 1);
                            if (forste.First() == siste.First())
                            {
                                retur.Add(starter[0].Concat(stopper[0]).Select(st => konverterStasjon(st)).ToList());
                                retur.Add(starter[1].Concat(stopper[1]).Select(st => konverterStasjon(st)).ToList());
                            }
                            else
                            {
                                retur.Add(starter[0].Concat(stopper[1]).Select(st => konverterStasjon(st)).ToList());
                                retur.Add(starter[1].Concat(stopper[0]).Select(st => konverterStasjon(st)).ToList());
                            }
                        }
                        else
                        {
                            if (erRingbane(forste))
                                starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
                            else
                                starter = new List<DbStasjon>[] { stasjAtilB(a, fellesende(forste, siste), forste) };
                            if (erRingbane(siste))
                                stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
                            else
                                stopper = new List<DbStasjon>[] { stasjAtilB(fellesende(forste, siste), b, siste) };
                            foreach (List<DbStasjon> stli in starter)
                            {
                                stli.RemoveAt(stli.Count - 1);
                                foreach (List<DbStasjon> stlii in stopper)
                                    retur.Add(stli.Concat(stlii).Select(st => konverterStasjon(st)).ToList());
                            };
                        }
                    }
                    ++g;
                }
                if (g < antgrp && stierEtterLengde[g].Key == 3)
                {
                    IList<DbStasjon> forste, andre, siste;
                    DbStasjon veimerke;
                    List<DbStasjon> midtstykke;
                    List<DbStasjon> ret = new List<DbStasjon>();
                    foreach (IList<DbHovedstrekning> lentre in stierEtterLengde[g])
                    {
                        forste = lentre[0].Stasjoner;
                        andre = lentre[1].Stasjoner;
                        siste = lentre[2].Stasjoner;
                        if (erEndelike(forste, andre) && erEndelike(andre, siste)) // To stier; "sikk-sakk"
                        {
                            starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
                            stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
                            starter[0].RemoveAt(starter[0].Count - 1);
                            starter[1].RemoveAt(starter[1].Count - 1);

                            ret.Clear();
                            ret.AddRange(starter[0]);
                            veimerke = (forste.First() == andre.First()) ? andre.First() : andre.Last();
                            ret.AddRange(heleFraEndeUSiste(veimerke, andre));
                            veimerke = motsattEnde(veimerke, andre);
                            ret.AddRange((veimerke == siste.First()) ? stopper[0] : stopper[1]);
                            retur.Add(ret.Select(st => konverterStasjon(st)).ToList());

                            ret.Clear();
                            ret.AddRange(starter[1]);
                            veimerke = (forste.Last() == andre.First()) ? andre.First() : andre.Last();
                            ret.AddRange(heleFraEndeUSiste(veimerke, andre));
                            veimerke = motsattEnde(veimerke, andre);
                            ret.AddRange((veimerke == siste.First()) ? stopper[0] : stopper[1]);
                            retur.Add(ret.Select(st => konverterStasjon(st)).ToList());
                        }
                        else
                        {
                            if (erRingbane(forste))
                                starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
                            else
                            {
                                veimerke = (erEndelike(forste, andre)) ? motsattEnde(fellesende(andre, siste), andre) : fellesende(forste, andre);
                                starter = new List<DbStasjon>[] { stasjAtilB(a, veimerke, forste) };
                            }
                            if (erRingbane(siste))
                                stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
                            else
                            {
                                veimerke = (erEndelike(siste, andre)) ? motsattEnde(fellesende(andre, forste), andre) : fellesende(siste, andre);
                                stopper = new List<DbStasjon>[] { stasjAtilB(veimerke, b, siste) };
                            }

                            veimerke = (erEndelike(forste, andre)) ? motsattEnde(fellesende(andre, siste), andre) : fellesende(forste, andre);
                            midtstykke = heleFraEndeUSiste(veimerke, andre);

                            foreach (List<DbStasjon> stli in starter)
                            {
                                stli.RemoveAt(stli.Count - 1);
                                foreach (List<DbStasjon> stlii in stopper)
                                    retur.Add(stli.Concat(midtstykke).Concat(stlii).Select(st => konverterStasjon(st)).ToList());
                            }
                        }
                    }
                    ++g;
                }
                while (g < antgrp) // Key >= 4
                {
                    IList<DbStasjon> forste, andre, nestsiste, siste;
                    DbStasjon veimerke;
                    List<DbStasjon> midtstykke;
                    foreach (IList<DbHovedstrekning> lenx in stierEtterLengde[g])
                    {
                        forste = lenx[0].Stasjoner;
                        andre = lenx[1].Stasjoner;
                        nestsiste = lenx[lenx.Count - 2].Stasjoner;
                        siste = lenx[lenx.Count - 1].Stasjoner;

                        if (erRingbane(forste))
                            starter = fraTilBeggeEnder(a, forste); // Alltid lengde == 2
                        else
                        {
                            veimerke = motsattEnde(fellesende(andre, lenx[2].Stasjoner), andre);
                            starter = new List<DbStasjon>[] { stasjAtilB(a, veimerke, forste) };
                        }
                        if (erRingbane(siste))
                            stopper = fraBeggeEnderTil(siste, b); // Alltid lengde == 2
                        else
                        {
                            veimerke = motsattEnde(fellesende(nestsiste, lenx[lenx.Count - 3].Stasjoner), nestsiste);
                            stopper = new List<DbStasjon>[] { stasjAtilB(veimerke, b, siste) };
                        }

                        veimerke = motsattEnde(fellesende(andre, lenx[2].Stasjoner), andre);
                        midtstykke = new List<DbStasjon>();
                        for (int i = 1; i < lenx.Count - 1; ++i)
                        {
                            midtstykke.AddRange(heleFraEndeUSiste(veimerke, lenx[i].Stasjoner));
                            veimerke = motsattEnde(veimerke, lenx[i].Stasjoner);
                        }

                        foreach (List<DbStasjon> stli in starter)
                        {
                            stli.RemoveAt(stli.Count - 1);
                            foreach (List<DbStasjon> stlii in stopper)
                                retur.Add(stli.Concat(midtstykke).Concat(stlii).Select(st => konverterStasjon(st)).ToList());
                        }
                    }
                    ++g;
                }
            }
            return retur;

            bool traverser(DbHovedstrekning hovstr, DbStasjon feilEnde)
            {
                DbStasjon rettEnde = ((rettEnde = hovstr.Stasjoner.First()) == feilEnde) ? hovstr.Stasjoner.Last() : rettEnde;
                if (rettEnde == a || rettEnde == feilEnde) // Da har soket bitt seg i halen (starten), eller er en ringbane.
                {
                    blinde.Add(hovstr); // Merk den som blind
                    return true;
                }
                IEnumerable<DbHovedstrekning> nabohs = rettEnde.Hovedstrekninger.Where(hs => hs != hovstr);
                if (nabohs.Count() == 0) // Strekningen er et blindspor.
                {
                    blinde.Add(hovstr); // Merk den som blind
                    return true;
                }
                if (nabohs.Count(hs => stitilna.Contains(hs)) >= 2) // Da har soket bitt seg i halen, og er ikke blind/ring
                    return false;

                bool blind = true;
                List<DbHovedstrekning> traverserVidere = new List<DbHovedstrekning>();
                DbHovedstrekning funnetStr = null;
                foreach (DbHovedstrekning hs in nabohs)
                {
                    if (bhs.Contains(hs)) // Da er det funnet en sti
                    {
                        stitilna.Add(hs); // Legger til siste delstrekning for kopiering
                        strekninger.Add(new List<DbHovedstrekning>(stitilna)); // Kopierer
                        stitilna.RemoveAt(stitilna.Count - 1); // Fjerner den midlertidig tillagte
                        funnetStr = hs;
                        blind = false;
                    }
                    else if (!blinde.Contains(hs))
                        traverserVidere.Add(hs);
                }

                // Sortere traverserVidere med hensyn pa hvilke hovedstrekninger som ligger geografisk naermest B?
                nabohs = traverserVidere;
                if (funnetStr == null || !ahs.Contains(funnetStr))
                {   // Dropper traversering nar det er umulig a na fram
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        stitilna.Add(hs);
                        if (!traverser(hs, rettEnde))
                            blind = false;
                        stitilna.RemoveAt(stitilna.Count - 1);
                    }
                }

                // Hvis ingenting har fatt denne til a ikke vaere blind, merk den som blind
                if (blind)
                    blinde.Add(hovstr);
                return blind;
            } // Slutt traverser()

            bool erRingbane(IEnumerable<DbStasjon> erring)
            {
                return erring.First() == erring.Last();
            }
            bool erEndelike(IEnumerable<DbStasjon> lia, IEnumerable<DbStasjon> lib)
            {
                return (lia.First() == lib.First() && lia.Last() == lib.Last())
                    || (lia.First() == lib.Last() && lia.Last() == lib.First());
            }
            List<DbStasjon>[] fraTilBeggeEnder(DbStasjon fra, IList<DbStasjon> dbStasjList)
            {
                int i, idx = dbStasjList.IndexOf(fra);
                List<DbStasjon> stListe = new List<DbStasjon>(idx + 1);
                List<DbStasjon> stListe2 = new List<DbStasjon>(dbStasjList.Count - idx);
                for (i = idx; i > -1; --i)
                    stListe.Add(dbStasjList[i]);
                for (i = dbStasjList.Count; idx < i; ++idx)
                    stListe2.Add(dbStasjList[idx]);
                return new List<DbStasjon>[] { stListe, stListe2 };
            }
            List<DbStasjon>[] fraBeggeEnderTil(IList<DbStasjon> dbStasjList, DbStasjon til)
            {
                int i, idx = dbStasjList.IndexOf(til);
                List<DbStasjon> stListe = new List<DbStasjon>(idx + 1);
                List<DbStasjon> stListe2 = new List<DbStasjon>(dbStasjList.Count - idx);
                for (i = 0; i <= idx; ++i)
                    stListe.Add(dbStasjList[i]);
                for (i = dbStasjList.Count - 1; i >= idx; --i)
                    stListe2.Add(dbStasjList[i]);
                return new List<DbStasjon>[] { stListe, stListe2 };
            }
            List<DbStasjon> stasjAtilB(DbStasjon aDbSt, DbStasjon bDbSt, IList<DbStasjon> dbStasjList)
            {
                int aidx = dbStasjList.IndexOf(aDbSt);
                int bidx = dbStasjList.IndexOf(bDbSt);
                int inkr = (aidx < bidx) ? 1 : -1;
                List<DbStasjon> stListe = new List<DbStasjon>(((inkr == 1) ? bidx - aidx : aidx - bidx) + 1);
                for (bidx += inkr; aidx != bidx; aidx += inkr)
                    stListe.Add(dbStasjList[aidx]);
                return stListe;
            }
            List<DbStasjon> heleFraEndeUSiste(DbStasjon ende, IList<DbStasjon> dbStasjList)
            {
                if (ende == dbStasjList.First())
                    return dbStasjList.Take(dbStasjList.Count - 1).ToList();
                else if (ende == dbStasjList.Last())
                    return dbStasjList.Skip(1).Reverse().ToList();
                return null;
            }
            DbStasjon motsattEnde(DbStasjon ende, IEnumerable<DbStasjon> dbStasjList)
            {
                return (ende == dbStasjList.First()) ? dbStasjList.Last() : (ende == dbStasjList.Last()) ? dbStasjList.First() : null;
            }
            DbStasjon fellesende(IEnumerable<DbStasjon> lia, IEnumerable<DbStasjon> lib)
            {
                return (lia.First() == lib.First() || lia.First() == lib.Last()) ? lia.First()
                    : (lia.Last() == lib.Last() || lia.Last() == lib.First()) ? lia.Last() : null;
            }
            List<DbStasjon> heleFraEnde(DbStasjon ende, IList<DbStasjon> dbStasjList)
            {
                if (ende == dbStasjList.First())
                    return dbStasjList.Take(dbStasjList.Count).ToList();
                else if (ende == dbStasjList.Last())
                    return dbStasjList.Skip(0).Reverse().ToList();
                return null;
            }
            List<Stasjon> konvHeleFraEnde(DbStasjon ende, IList<DbStasjon> dbStasjList)
            {
                if (ende == dbStasjList.First())
                    return dbStasjList.Select(st => konverterStasjon(st)).ToList();
                else if (ende == dbStasjList.Last())
                    return dbStasjList.Select(st => konverterStasjon(st)).Reverse().ToList();
                return null;
            }
            List<Stasjon> KonvHeleFraEndeUSiste(DbStasjon ende, IList<DbStasjon> dbStasjList)
            {
                List<Stasjon> ret = null;
                if (ende == dbStasjList.First())
                {
                    ret = dbStasjList.Select(st => konverterStasjon(st)).ToList();
                    ret.RemoveAt(ret.Count - 1);
                }
                else if (ende == dbStasjList.Last())
                {
                    ret = dbStasjList.Select(st => konverterStasjon(st)).Reverse().ToList();
                    ret.RemoveAt(ret.Count - 1);
                }
                return ret;
            }

        }

        // Slutt spesialiserte hentemetoder

        /** 
         * Under har vi metoder for å manipulere databasen og legge til eksempeldata
         * **/

        Rute konverterRute(DbRute dbrt)
        {
            return new Rute
            {
                DateTime = dbrt.DateTime,
                Id = dbrt.RuteID,
                Start_id = dbrt.Start_id,
                Stopp_id = dbrt.Stopp_id
            };
        }

        DbRute konverterDbRute(Rute dbrt)
        {
            return new DbRute
            {
                DateTime = dbrt.DateTime,
                Start_id = dbrt.Start_id,
                Stopp_id = dbrt.Stopp_id
            };
        }
        public void addRute(int start, int stopp)
        {
            DateTime now = DateTime.Now;
            using (var db = new VyDbContext())
            {
                for (int i = 0; i < 24; i++)
                {
                    now = now.AddHours(3);
                    Rute rute = new Rute();
                    rute.Start_id = start;
                    rute.Stopp_id = stopp;
                    rute.DateTime = now;
                    db.Ruter.Add(konverterDbRute(rute));
                }
                db.SaveChanges();
            }
        }
        DbStasjon KonverterDbStasjon(Stasjon st)
        {

            return new DbStasjon
            {
                StasjonId = st.id,
                StasjNavn = st.stasjon_navn,
                StasjSted = st.stasjon_sted,
                Breddegrad = st.breddegrad,
                Lengdegrad = st.lengdegrad,
                Hovedstrekninger = st.hovedstrekninger.Select(hs => GetDbHovedstrekning(hs)).ToList(),
                //dette ser rart ut, men det er bare et nett i vårt eksempel
                NettId = st.nett_id,
                Nett = GetDbNett(st.nett_id)
            };
        }
        DbHovedstrekning GetDbHovedstrekning(int id)
        {
            using (var db = new VyDbContext())
            {
                return db.Hovedstrekninger.Find(id);
            }
        }

        DbNett GetDbNett(int id)
        {
            using (var db = new VyDbContext())
            {
                return db.Nett.Find(id);
            }
        }
        public void AddStasjoner()
        {
            using (var db = new VyDbContext())
            {
                VyDbTilgang dbt = new VyDbTilgang();
                List<CSVstasjon> liste = CSVstasjon.convertEngine();
                foreach (CSVstasjon stasjon in liste)
                {
                    List<DbHovedstrekning> hvdstr = new List<DbHovedstrekning>();
                    DbHovedstrekning dbhvst = GetDbHovedstrekning(1);
                    hvdstr.Add(dbhvst);
                    DbStasjon st = new DbStasjon
                    {
                        StasjSted = stasjon.name,
                        StasjNavn = stasjon.name,
                        Hovedstrekninger = hvdstr,
                        Nett = dbt.GetDbNett(1),
                        Lengdegrad = stasjon.longitude,
                        Breddegrad = stasjon.latitude
                    };
                    db.Stasjoner.Add(st);
                }
                db.SaveChanges();
            }
        }
        IEnumerable<int> Rnd()
        {
            List<int> randList = new List<int>();
            Random r = new Random();
            Random rndAntall = new Random();
            for (int i = 0; i < rndAntall.Next(1, 7); i++)
            {
                int rInt = r.Next(0, 60); //for ints
                randList.Add(rInt);
            }
            return randList;
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
        public void addNett()
        {
            using (var db = new VyDbContext())
            {
                DbNett nett = new DbNett();
                nett.NettId = 1;
                nett.Nettnavn = "Norge";
                db.Nett.Add(nett);
                db.SaveChanges();
            }

        }
        
    }
}