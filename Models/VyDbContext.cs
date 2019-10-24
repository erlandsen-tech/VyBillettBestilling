using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class VyDbContext : DbContext
    {
        public VyDbContext() : base("name=BillettBase")
        {
            Database.CreateIfNotExists();
        }

        // Tabeller for linjenettet:
        public DbSet<DbStasjon> Stasjoner { get; set; }
        public DbSet<DbPris> Pris { get; set; }
        // Opplisting av stasjoner, ev. med data om plattformer, geografisk plassering og annet (betjent?, toalett? osv.)
        // Ogsa data om Hovedstrekning(-er) den tilhorer. (Geo.pos. gir opplosning pa <= 1,1 m med 5 desimaler, det er nok.)
        public DbSet<DbHovedstrekningStasjon> HovstrStasj { get; set; }
        //public DbSet<DbDelstrekning> Delstrekninger { get; set; } // Avstand (og pris?) mellom to nabostasjoner. Forelopig ikke i bruk.
        public DbSet<DbHovedstrekning> Hovedstrekninger { get; set; }
        // En DbHovedstrekning (Eks. Oslo-Bergen) har en _ordnet_ liste av STASJONER
        // Den tilhorer et NETT av DbHovedstrekning-er som er forbundet med hverandre (sann at tog
        // kan kjore fra ett sted pa nettet til et annet). DbHovedstrekning-er pa forskjellige nett har ingen forbindelse
        // Den har to endestasjoner, som hver er tilknyttet 0 til flere andre DbHovedstrekning-er.
        // Den er ikke tiknyttet andre Dbhovedstrekning-er unntatt i endestasjonene.
        // Enhver stasjon med linjer i mer eller mindre enn to retninger blir dermed en endestajon pa en eller flere DbHovedstrekning-er
        // ("Endestasjon" her referer til DbHovedstrekning-en, ikke til ruten toget kjorer;
        // tog kan kjore fra en DbHovedstrekning til en annen gjennom en endestasjon.)
        // DbHovedstrekningen har en DISTANSE. Inntil videre regnes alle distanser mellom stasjoner
        // pa DbHovedstrekning-en a vaere like, som en del av hele distansen pa DbHovedstrekning-en,
        // nar DbStrekning-er tas i bruk genereres DbHovedstrekning-ens distanse som sum av avstandene til DbStrekning-ene.
        // Spesialtilfelle: En DbHovedstrekning som går i ring vil ha samme stasjon i begge ender. Hva gjor vi med det?
        // I modellen her forutsettes at det ikke finnes mer enn to (en, kanskje?) sti(er) mellom
        // to stasjoner pa hele linjenettet, dette for a unnga for kompliserte grafberegninger
        public DbSet<DbNett> Nett { get; set; }
        // Hvert Nett er en samling av DbHovedstrekning-er som er forbundet med hverandre.

        // Klasser for linjenettet:
        
        public class DbNett
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public String Nettnavn { get; set; } // Sor-Norge, Ofoten, Japan osv.
            public virtual List<DbHovedstrekning> Hovedstrekninger { get; set; } // IEnumerable eller ICollection?
            public virtual List<DbStasjon> Stasjoner { get; set; } // IEnumerable eller ICollection? // Droppe denne?
            public DbNett(string navn)
            {
                Nettnavn = navn;
                Hovedstrekninger = new List<DbHovedstrekning>();
                Stasjoner = new List<DbStasjon>();
            }
            public DbNett() { } // Ma ogsa ha en parameterlos konstruktor. Vet ikke hvorfor, men sann er det.
        }
        public class DbHovedstrekning
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public String HovstrNavn { get; set; }
            [Required]
            public String HovstrKortNavn { get; set; }
            //[Required] // Kan ikke vaere required, uansett hvordan man gjor det med virtual og forskjellige navn osv.. Lager da kaskederende delete m.v.
            public virtual DbNett Nett { get; set; } // DbHovedstrekning-er pa samme Nett er forbundet med hverandre med et antall DbHovedstrekninger
                                                     //public virtual List<DbDelstrekning> Delstrekninger { get; set; }

            // Denne skal ikke brukes, er her bare for a tilfredsstille EFs basekrav:
            public virtual List<DbStasjon> StasjonerIkkeBruk { get; set; }

            public virtual List<DbHovedstrekningStasjon> StasjonerNummerert { get; set; }

            // Ingenting virker uten konstruktor i nullparam

            //public virtual StasjonListeHjelper Stasjoner { get; set; } // Virker ikke (no key defined)
            //internal virtual StasjonListeHjelper Stasjoner { get; set; } // Gir mange exceptions (50) (32)
            //public  StasjonListeHjelper Stasjoner { get; set; } // Virker ikke (no key defined)
            //internal StasjonListeHjelper Stasjoner { get; set; } // Gir mange exceptions (50) (32)
            //public StasjonListeHjelper Stasjoner; // Gir mange exceptions (50) (32)
            internal StasjonListeHjelper Stasjoner; // Gir mange exceptions (50) (32)
            //public DbHovedstrekning Stasjoner => this;  // Gir ogsa mange exceptions! (50)

            public class StasjonListeHjelper
            {
                public List<DbHovedstrekningStasjon> StasjonerNummerert => Eier.StasjonerNummerert;
                public DbHovedstrekning Eier { get; set; }
                public StasjonListeHjelper(DbHovedstrekning eierStrekning)
                {
                    Eier = eierStrekning;
                }
                public DbHovedstrekningStasjon faaDbElement(DbStasjon stas)
                {
                    return StasjonerNummerert.First(hosta => hosta.Stasjon.Equals(stas));
                }
                public List<DbHovedstrekningStasjon> faaDbElementer(List<DbStasjon> stasjer)
                {
                    List<DbHovedstrekningStasjon> ret = new List<DbHovedstrekningStasjon>(stasjer.Count());
                    foreach (var stas in stasjer)
                        ret.Add(StasjonerNummerert.FirstOrDefault(hosta => stas.Equals(hosta.Stasjon)));
                    return ret;
                }
                public List<DbHovedstrekningStasjon> faaAlleDbElementer()
                {
                    return StasjonerNummerert.ToList();
                }
               private List<DbStasjon> Stasjjjoner()
                {
                    return StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).Select(st => st.Stasjon).ToList();
                }
                public List<DbStasjon> ToList() => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).Select(st => st.Stasjon).ToList();
                public DbStasjon[] ToArray() => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).Select(st => st.Stasjon).ToArray();
                public int Count() => StasjonerNummerert.Count();
                public bool Contains(DbStasjon stas) => StasjonerNummerert.Any(hosta => hosta.Stasjon.Equals(stas));
                public DbStasjon ElementAt(int index) => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).ElementAt(index).Stasjon;
                public DbStasjon First() => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).First().Stasjon;
                public DbStasjon FirstOrDefault() => (StasjonerNummerert.Count() == 0) ? null
                    : StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).First().Stasjon;
                public int IndexOf(DbStasjon stas) => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr)
                    .ToList().FindIndex(hosta => hosta.Stasjon.Equals(stas));
                public int LastIndexOf(DbStasjon stas) => StasjonerNummerert.OrderBy(hosta => hosta.rekkenr)
                    .ToList().FindLastIndex(hosta => hosta.Stasjon.Equals(stas));
                public DbStasjon Last() => StasjonerNummerert.OrderByDescending(hosta => hosta.rekkenr).First().Stasjon;
                public DbStasjon LastOrDefault() => (StasjonerNummerert.Count() == 0) ? null
                    : StasjonerNummerert.OrderByDescending(hosta => hosta.rekkenr).First().Stasjon;
                
                public void Add(DbStasjon stas)
                {
                    double rekkenr = (StasjonerNummerert.Count() == 0) ? 0 : StasjonerNummerert.Max(d => d.rekkenr);
                    StasjonerNummerert.Add(new DbHovedstrekningStasjon(Eier, stas, rekkenr + 100));
                }
                public void AddRange(List<DbStasjon> stasjer)
                {
                    double rekkenr = (StasjonerNummerert.Count() == 0) ? 0 : StasjonerNummerert.Max(d => d.rekkenr);
                    foreach (DbStasjon stas in stasjer)
                        StasjonerNummerert.Add(new DbHovedstrekningStasjon(Eier, stas, rekkenr += 100));
                }
                public void Insert(int index, DbStasjon stas)
                {
                    if (index < 0 | index > StasjonerNummerert.Count())
                        throw new ArgumentOutOfRangeException("index < 0 || index > Stasjoner_Count()");
                    double nr = 100;
                    if (StasjonerNummerert.Count() == 0) ; // Da er initialverdien riktig
                    else if (index == StasjonerNummerert.Count())
                        nr = StasjonerNummerert.Max(d => d.rekkenr) + 100; // feiler nar count == 0
                    else if (index == 0)
                        nr = StasjonerNummerert.Min(d => d.rekkenr) / 2; // feiler nar count == 0
                    else
                    {
                        var ordnet = StasjonerNummerert.OrderBy(hosta => hosta.rekkenr);
                        nr = (ordnet.ElementAt(index - 1).rekkenr + ordnet.ElementAt(index).rekkenr) / 2;
                    }
                    StasjonerNummerert.Add(new DbHovedstrekningStasjon(Eier, stas, nr));
                }
                public void InsertRange(int index, List<DbStasjon> stasjer)
                {
                    if (index < 0 | index > StasjonerNummerert.Count())
                        throw new ArgumentOutOfRangeException("index < 0 || index > Stasjoner_Count()");
                    double teller = 0, inkr = 100;
                    if (StasjonerNummerert.Count() == 0) ; // Da er initialverdiene riktige
                    else if (index == StasjonerNummerert.Count())
                        teller = StasjonerNummerert.Max(d => d.rekkenr); // feiler nar count == 0 // max , 100
                    else if (index == 0)
                        inkr = StasjonerNummerert.Min(d => d.rekkenr) / (stasjer.Count() + 1); // feiler nar count == 0  // 0 , min/(s.count + 1)
                    else
                    {
                        var ordnet = StasjonerNummerert.OrderBy(hosta => hosta.rekkenr);
                        teller = ordnet.ElementAt(index - 1).rekkenr;
                        inkr = (ordnet.ElementAt(index).rekkenr - teller) / (stasjer.Count() + 1);
                    }
                    foreach (DbStasjon stas in stasjer)
                        StasjonerNummerert.Add(new DbHovedstrekningStasjon(Eier, stas, teller += inkr));
                }
                public void Reverse()
                {
                    var ordnet = StasjonerNummerert.OrderByDescending(hosta => hosta.rekkenr);
                    double nr = 0;
                    foreach (var hosta in ordnet)
                        hosta.rekkenr = nr += 100;
                }
            /* Clear() og Remove-metodene fungerer ikke, lager bare kroll.
             * Kraesjer nar Stasjon og Hovedstrekning er Required i DbHovedstrekningStasjon,
             * og setter null-verdier i tabellen nar de ikke Required, uten at postene blir borte */
            //public void Clear() { StasjonerNummerert.Clear(); }
            //public bool Remove(DbStasjon dbst)
            //{
            //    int idx = StasjonerNummerert.FindIndex(st => st.Stasjon.Equals(dbst));
            //    if (idx < 0)
            //        return false;
            //    StasjonerNummerert.RemoveAt(idx);
            //return true;
            //}
            //public void RemoveAt(int index)
            //{
            //    StasjonerNummerert.Remove(StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).ElementAt(index));
            //}
            //public void RemoveRange(int index, int count)
            //{
            //    if (index < 0 | count < 0 | index + count > StasjonerNummerert.Count())
            //        throw new ArgumentOutOfRangeException("index < 0 || count < 0 || index+count > Stasjoner_Count()");
            //    var ordnet = StasjonerNummerert.OrderBy(hosta => hosta.rekkenr).Skip(index).Take(count);
            //    StasjonerNummerert.RemoveAll(hosta => ordnet.Contains(hosta));
            //}
            }

            public DbHovedstrekning(string navn, DbNett nett, string kortnavn)
            {
                HovstrNavn = navn;
                HovstrKortNavn = kortnavn;
                Nett = nett;
                StasjonerIkkeBruk = new List<DbStasjon>();
                StasjonerNummerert = new List<DbHovedstrekningStasjon>();
                Stasjoner = new StasjonListeHjelper(this);
            }
            public DbHovedstrekning() {
                Stasjoner = new StasjonListeHjelper(this);
            } // Ma ogsa ha en parameterlos konstruktor.
        }
        //public class DbDelstrekning
        //{
        //    [Key]
        //    public int Id { get; set; }
        //    //[Required]
        //    public int HovstrId { get; set; }
        //    //[Required]
        //    public DbHovedstrekning Hovedstrekning { get; set; }

        //    [Required]
        //    public DbStasjon Start { get; set; }
        //    [Required]
        //    public int StartStasjonId { get; set; }
        //    [Required]
        //    public DbStasjon Stopp { get; set; }
        //    [Required]
        //    public int StoppStasjonId { get; set; }

        //    [Required]
        //    public double Distanse { get; set; }
        //}


        public class DbHovedstrekningStasjon
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public virtual DbHovedstrekning Hovedstrekning { get; set; }
            [Required]
            public virtual DbStasjon Stasjon { get; set; }
            [Required]
            public double rekkenr { get; set; }

            public DbHovedstrekningStasjon(DbHovedstrekning hovstr, DbStasjon stasj, double rekkenr)
            {
                Hovedstrekning = hovstr; Stasjon = stasj; this.rekkenr = rekkenr;
            }
            public DbHovedstrekningStasjon() { }
        }
        public class DbStasjon
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public String StasjNavn { get; set; }
            // Kombinasjonen av stasjonnavn og stasjonsted forutsettes unik pa hvert nett.
            // Ved like StasjNavn pa et nett ma det tilfoyes StasjSted pa alle (eventuelt unntatt ett)
            public String StasjSted { get; set; }
            // [Required] // Kan ikke vaere required, uansett hvordan man gjor det met virtual og forskjellige navn osv.. Lager da kaskederende delete m.v.
            public virtual DbNett Nett { get; set; } // Trengs ikke nar det brukes List<DbHovedstrekning> Hovedstrekninger,
                                                     // da er nettet umiddelbart tilgjengelig fra alle elementene i lista.

            [Range(-90, 90, ErrorMessage = "Ugyldig koordinat; -90 <= Breddegrad <= 90")]
            public double Breddegrad { get; set; }
            [Range(-180, 180, ErrorMessage = "Ugyldig koordinat; -180 <= Lengdegrad <= 180")]
            public double Lengdegrad { get; set; }

            // Ha denne virtual eller ikke? I stedet vise til Hovedstrekningen med en fremmednokkel?
            // Nei, det kan vaere flere, matte da ha sammenknytningstabell. Kjekt a slippe det.
            public virtual List<DbHovedstrekning> Hovedstrekninger { get; set; } // De fleste stasjoner har en i denne lista.
            // De som har >= 2 utgjor sammenknytninger mellom DbHovedstrekninger,
            // og er samtidig endepunkt for de tilknyttede hovedstrekningene
            // NBNBNBNB!!! : Ringbaner bor fa satt begge endene i denne lista for at stisokingen skal fungere riktig,
            // altsa; samme stasjon (tilknytningsstasjonen) skal settes i lista to ganger. Mulig det lager basekroll, ma i sa
            // fall ordne det pa annen mate
            public DbStasjon(string navn, DbNett nett, string optStedNavn = "")
            {
                StasjNavn = navn;
                StasjSted = optStedNavn;
                Nett = nett;
                Hovedstrekninger = new List<DbHovedstrekning>();
            }
            public DbStasjon() { } // Ma ogsa ha en parameterlos konstruktor. Vet ikke hvorfor, men sann er det.

            // Dropper 
            // Ha denne virtual eller ikke? I stedet vise til Delstrekningen med en fremmednokkel?
            // Nei, det kan vaere flere, matte da ha sammenknytningstabell
            //public virtual List<DbDelstrekning> Delstrekninger { get; set; } // De fleste stasjoner har to i denne lista
            // De som har bare en er fysiske endestasjoner, de som har >= 3 utgjor sammenknytninger mellom DbHovedstrekninger,
            // og er samtidig endepunkt for de tilknyttede hovedstrekningene

            // Dropper disse forelopig:
            //public int AntPlattformer { get; set; }
            //public bool Betjent { get; set; }
            //public bool Toalett { get; set; }
            //public Apningstid Aapen { get; set; } // Apningstider ma lagres i en egen klasse.
        }

        // Tabeller for kunder og kjop:
        public DbSet<DbBillett> Billetter { get; set; } // Kunde, start- og stopp-stasjon, passasjertype, pris (med rabattsats)
        // stasjonsliste(?), distanse(?)
        // Merk: Ma lagre pris (og ev. rabatt og passasjertype?) i Billett-en, siden de kan endre seg etter kjopet.
        public DbSet<DbBillettKjop> Billettkjop { get; set; }
        public DbSet<DbKunde> Kunder { get; set; } // Navn, adresse, tlf o.l.
        public DbSet<DbPassasjertype> Passasjertyper { get; set; } // Ordinaer, student, honnor osv. med tilhorende rabatter

        // Klasser for kunder og kjop:
        public class DbPassasjertype
        {
            [Key]
            public int ptypId { get; set; }

            [Required]
            public String TypeNavn { get; set; } // Ordinaer, student, barn, honnor, verneplikt, avtale(navn) m/prosent, osv.
            [Range(0, 100, ErrorMessage = "Rabatt min. 0 %, max 100 %")] // Gjor dette verdien Required? Nei, sjekker bare non-NULL-verdier
            public double Rabatt { get; set; }
            public int OvreAldersgrense { get; set; }
            public int NedreAldersgrense { get; set; }
        }
        public class DbKunde
        {
            [Key]
            public int kId { get; set; }
            public virtual List<DbBillettKjop> Kjop { get; set; }

            [Required] // [EmailAddress] // ?
            public String Epost { get; set; } // Brukernavn, pakrevet (for a sende billett, og senere for palogging)
            public DateTime Foedt { get; set; } // For a sjekke aldersgrense, kanskje
            // [Required] // ?
            public String Fornavn { get; set; }
            // [Required]
            public String Etternavn { get; set; }
            // [Phone] // ?
            public String Mobilnr { get; set; }
            //public String[] Bet_kort { get; set; } // Gjor det enklere, lagrer bare ett:
            // [CreditCard] // ?
            public String Bet_kort { get; set; } // Skal lagres som sifferstreng uten mellomrom(?)
        }
        public class DbBillettKjop
        {
            [Key]
            public int bkId { get; set; }
            public int kndId { get; set; }
            public DbKunde Kunde { get; set; }
            public virtual List<DbBillett> Billetter { get; set; }

            // [CreditCard] // ?
            public String Bet_kort { get; set; } // Skal lagres som sifferstreng uten mellomrom(?). Kort til hvert kjop ma lagres, siden kundens kort kan forandre seg
            public DateTime Kjopsdato { get; set; }
        }
        public class DbBillett
        {
            [Key]
            public int bId { get; set; }
            public int kjopId { get; set; }
            public DbBillettKjop Kjop { get; set; }

            // Mest hensiksmessig a lagre tekstverdiene som skal sta pa billetten i DbBillett-en
            public String StartStasjon { get; set; }
            public String StoppStasjon { get; set; }
            public DateTime Avreisetid { get; set; }
            public DateTime Ankomsttid { get; set; }
            // Pris, passasjertype og rabattsats ma fikseres i billetten, siden de kan endre seg etter kjopet.
            public double Pris { get; set; }
            public String Passasjertype { get; set; }
            public double Rabattsats { get; set; }
        }
        public class DbPris
        {
            [Key]
            public int Id { get; set; }
            public double prisPrKm { get; set; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}