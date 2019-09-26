using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class VyDbTilgang
    {

        public IQueryable<Stasjon> HentAlleStasjoner()
        {
            VyDbContext db = new VyDbContext();
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
        }

        public IQueryable<Stasjon> HentStasjonerPaNett(int nettId)
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
                //bbredde = b.Breddegrad; blengde = b.Lengdegrad;

                if (a.NettId != b.NettId) // Ingen sti finnes.
                    return new List<List<Stasjon>>(); // Returnere null i stedet?

                ahs = a.Hovedstrekninger;
                bhs = b.Hovedstrekninger;
                felleshs = ahs.Intersect(bhs);

                if (ida == idb) // Start- og stoppstasjon er den samme. Det er bare tull, men returnerer en/flere "stier" likevel
                {
                    // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
                    foreach (DbHovedstrekning hs in ahs)
                        strekninger.Add(new List<DbHovedstrekning> { hs });
                    ikkeferdig = false;
                    return new List<List<Stasjon>> { new List<Stasjon> { new Stasjon

                    {
                        id = a.StasjonId,
                        stasjon_navn = a.StasjNavn,
                        stasjon_sted = a.StasjSted,
                        breddegrad = a.Breddegrad,
                        lengdegrad = a.Lengdegrad,
                        hovedstrekninger = ahs.Select(hs => hs.HovstrId).ToArray(),
                        nett_id = a.NettId,
                        nett_navn = a.Nett.Nettnavn
                    } } };
                }

                // Spesialtilfelle: A og B er pa samme blindbane eller ringbane
                else if ((ahs.Count() == 1 || bhs.Count() == 1) && felleshs.Count() == 1)
                {
                    IList<DbStasjon> stasj = felleshs.First().Stasjoner;
                    if (stasj.First().Hovedstrekninger.Count() == 1 || stasj.Last().Hovedstrekninger.Count() == 1)
                    {    // Er blindbane. Bare en sti.
                         // Putter tilknyttede hovedstrekning(er) i strekninger forst, for ev. senere bruk:
                        strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });

                        ikkeferdig = false;
                        return new List<List<Stasjon>> { stasjAtilB(a, b, stasj) };
                    }
                    else if (stasj.First() == stasj.Last())
                    {   // Er ringbane. Legger inn de to stiene, en med en hovedstrekning og en med to,
                        // for a vise at "enden" ma krysses. Gjore det pa annen mate?
                        strekninger.Add(new List<DbHovedstrekning> { felleshs.First() });
                        strekninger.Add(new List<DbHovedstrekning> { felleshs.First(), felleshs.First() });

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
                        for (stopp = (inkr == 1) ? stasj.Count : -1; aidx != stopp; aidx += inkr)
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

                if (ikkeferdig && ahs.Count() == 1)
                {   // A ikke pa knutepunkt. A har da bare en hovedstrekning.
                    // Litt triksing nedenfor for a hoppe over kode nar a og b pa samme strekning
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
                            {
                                blinde.Add(hs);
                            }
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
                        {
                            blinde.Add(hs);
                        }
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

                else if (ikkeferdig)  // A pa knutepunkt, b pa samme hovedstrekning fungerer fint:
                {
                    nabohs = ahs;
                    foreach (DbHovedstrekning hs in nabohs)
                    {
                        if (bhs.Contains(hs))
                        {
                            strekninger.Add(new List<DbHovedstrekning> { hs });
                        }
                        else if (hs.Stasjoner.First().Hovedstrekninger.Count() < 2 || hs.Stasjoner.Last().Hovedstrekninger.Count() < 2
                                || hs.Stasjoner.First() == hs.Stasjoner.Last()) // Er blind eller ringbane
                        {
                            blinde.Add(hs);
                        }
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

                {
                    //IList<DbStasjon> tmp, tmq;
                    //List<List<DbHovedstrekning>> trygge = strekninger.Where(li => (li.Count <= 1) || ( // Lengde 1 er alltid trygg
                    //    (
                    //        (tmp = li[0].Stasjoner)[0] == a || tmp.Last() == a // Starter i knutepunkt
                    //        || tmp[0] == tmp.Last() // Starter i ring
                    //        || tmp[0].Hovedstrekninger.Count() == 1 || tmp.Last().Hovedstrekninger.Count() == 1 // Starter i blindbane
                    //    )&&(
                    //        (tmp = li.Last().Stasjoner)[0] == b || tmp.Last() == b // Slutter i knutepunkt
                    //        || tmp[0] == tmp.Last() // Slutter i ring
                    //        || tmp[0].Hovedstrekninger.Count() == 1 || tmp.Last().Hovedstrekninger.Count() == 1 // Slutter i blindbane
                    //    )
                    //    )).Distinct().ToList();
                    //strekninger.RemoveAll(li => trygge.Contains(li)); // Fjerner de utplukkede trygge

                    //IEnumerable<List<DbHovedstrekning>> tmpenli = strekninger.Where(li => li.Count <= 1 || // her finnes flere trygge

                    //        (!endelike(li[0].Stasjoner, li[1].Stasjoner)
                    //        && !endelike(li[li.Count - 1].Stasjoner, li[li.Count - 2].Stasjoner))
                    //    ).Distinct();
                    //strekninger.RemoveAll(li => tmpenli.Contains(li)); // Fjerner de utplukkede trygge
                    //trygge.AddRange(tmpenli); // ..og legger dem til i trygge-lista
                    //// Uproblematiske: begge ender ring eller endestasjon, len == 1,

                    //tmpenli = strekninger.Where(li =>  // Finner de som er utrygge bare i starten
                    //        ((tmp = li[0].Stasjoner).First() == (tmq = li[1].Stasjoner).First() || tmp.First() != tmq.Last()
                    //        && tmp.Last() != tmq.First() && tmp.Last() != tmq.Last())
                    //    &&
                    //        ((tmp = li[li.Count - 1].Stasjoner).First() != (tmq = li[li.Count - 2].Stasjoner).First() && tmp.First() != tmq.Last()
                    //        && tmp.Last() != tmq.First() && tmp.Last() != tmq.Last())
                    //    );
                }

                var stierEtterLengde = strekninger.GroupBy(li => li.Count).OrderBy(ig => ig.Key).ToArray();
                int antgrp = stierEtterLengde.Length, g = 0;
                List<Stasjon>[] ringstarter;  // Alltid lengde == 2
                List<Stasjon>[] ringstopper;  // Alltid lengde == 2
                List<DbHovedstrekning> liho;
                List<List<DbHovedstrekning>> liliho;
                if (g < antgrp && stierEtterLengde[g].Key == 0) // Det skal ikke vaere noen med lengde 0 her. I sa fall er noe feil
                {
                    throw new Exception("Feil i stifinningen");
                    ++g;
                }
                if (g < antgrp && stierEtterLengde[g].Key == 1) // Det skal vaere maks 1 med lengde 1 her. Blir det flere er det noe feil
                {
                    foreach (var lenen in stierEtterLengde[g]) // Droppe lokka nar det er konstatert at dette virker riktig
                        retur.Add(stasjAtilB(a, b, lenen[0].Stasjoner));
                    ++g;
                }
                if (g < antgrp && stierEtterLengde[g].Key == 2)
                {
                    foreach (var lento in stierEtterLengde[g])
                    {
                        IList<DbStasjon> forste = lento[0].Stasjoner;
                        IList<DbStasjon> siste = lento[1].Stasjoner;
                        if (erEndelike(forste, siste))
                        {
                            ringstarter = fratilBeggeEnder(a, forste); // Alltid lengde == 2
                            ringstarter[0].Reverse();
                            ringstopper = fratilBeggeEnder(b, siste); // Alltid lengde == 2
                            ringstopper[1].Reverse();
                        }
                        else
                        {
                            if (erRingbane(forste))
                            {
                                ringstarter = fratilBeggeEnder(a, forste); // Alltid lengde == 2
                                ringstarter[0].Reverse();
                            }
                            else
                                ringstarter = new List<Stasjon>[] { stasjAtilB(a, fellesende(forste, siste), forste) };
                            if (erRingbane(siste))
                            {
                                ringstopper = fratilBeggeEnder(b, siste); // Alltid lengde == 2
                                ringstopper[1].Reverse();
                            }
                            else
                                ringstopper = new List<Stasjon>[] { stasjAtilB(fellesende(forste, siste), b, siste) };
                        }
                        if (erRingbane(forste) && erRingbane(siste)) // "Attetall"; fire mulige stier
                        {
                            ringstarter[0].RemoveAt(ringstarter[0].Count - 1);
                            ringstarter[1].RemoveAt(ringstarter[1].Count - 1);
                            retur.Add(ringstarter[1].Concat(ringstopper[0]).ToList());
                            retur.Add(ringstarter[1].Concat(ringstopper[1]).ToList());
                            retur.Add(ringstarter[0].Concat(ringstopper[0]).ToList());
                            retur.Add(ringstarter[0].Concat(ringstopper[1]).ToList());
                        }
                        else if (erEndelike(forste, siste))
                        { // To mulige stier
                            ringstarter[0].RemoveAt(ringstarter[0].Count - 1);
                            ringstarter[1].RemoveAt(ringstarter[1].Count - 1);
                            if (forste.Last() == siste.First())
                            {
                                retur.Add(ringstarter[1].Concat(ringstopper[0]).ToList());
                                retur.Add(ringstarter[0].Concat(ringstopper[1]).ToList());
                            }
                            else
                            {
                                retur.Add(ringstarter[1].Concat(ringstopper[1]).ToList());
                                retur.Add(ringstarter[0].Concat(ringstopper[0]).ToList());
                            }
                        }
                        else
                        {

                            foreach (var li in ringstarter) ;
                        }
                    }
                    ++g;
                }




            }


            return null;
            List<Stasjon>[] fratilBeggeEnder(DbStasjon aDbSt, IList<DbStasjon> dbStasjList)
            {
                int aidx = dbStasjList.IndexOf(aDbSt);
                List<Stasjon> stListe = new List<Stasjon>(aidx + 1);
                for (int i = 0; i <= aidx; ++i)
                    stListe.Add(konverterStasjon(dbStasjList[i]));
                List<Stasjon> stListe2 = new List<Stasjon>(dbStasjList.Count - aidx);
                for (; aidx < dbStasjList.Count; ++aidx)
                    stListe2.Add(konverterStasjon(dbStasjList[aidx]));
                return new List<Stasjon>[] { stListe, stListe2 };
            }
            List<Stasjon> stasjAtilB(DbStasjon aDbSt, DbStasjon bDbSt, IList<DbStasjon> dbStasjList)
            {

                int aidx = dbStasjList.IndexOf(aDbSt);
                int bidx = dbStasjList.IndexOf(bDbSt);
                int inkr = (aidx < bidx) ? 1 : -1;
                List<Stasjon> stListe = new List<Stasjon>((bidx - aidx) * inkr + 1);
                for (bidx += inkr; aidx != bidx; aidx += inkr)
                    stListe.Add(konverterStasjon(dbStasjList[aidx]));
                return stListe;
            }

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
                    {
                        traverserVidere.Add(hs);
                    }
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

            bool erRingbane(IList<DbStasjon> erring)
            {
                return erring.First() == erring.Last();
            }
            bool erEndelike(IList<DbStasjon> lia, IList<DbStasjon> lib)
            {
                return (lia.First() == lib.First() & lia.Last() == lib.Last())
                    | (lia.First() == lib.Last() & lia.Last() == lib.First());
            }
            DbStasjon fellesende(IList<DbStasjon> lia, IList<DbStasjon> lib)
            {
                return (lia.First() == lib.First() || lia.First() == lib.Last()) ? lia.First()
                    : (lia.Last() == lib.Last() || lia.Last() == lib.First()) ? lia.Last() : null;
            }
        }

        Stasjon konverterStasjon(DbStasjon dbst)
        {
            return new Stasjon
            {
                id = dbst.StasjonId,
                stasjon_navn = dbst.StasjNavn,
                stasjon_sted = dbst.StasjSted,
                breddegrad = dbst.Breddegrad,
                lengdegrad = dbst.Lengdegrad,
                hovedstrekninger = dbst.Hovedstrekninger.Select(hs => hs.HovstrId).ToArray(),
                nett_id = dbst.NettId,
                nett_navn = dbst.Nett.Nettnavn
            };
        }

    }
}