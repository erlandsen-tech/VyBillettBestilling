using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        // Opplisting av stasjoner, ev. med data om plattformer, geografisk plassering og annet (betjent?, toalett? osv.)
        // Ogsa data om Hovedstrekning(-er) den tilhorer. (Geo.pos. gir opplosning pa <= 1,1 m med 5 desimaler, det er nok.)
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
            public virtual List<DbStasjon> Stasjoner { get; set; }

            public DbHovedstrekning(string navn, DbNett nett, string kortnavn)
            {
                HovstrNavn = navn;
                HovstrKortNavn = kortnavn;
                Nett = nett;
                Stasjoner = new List<DbStasjon>();
            }
            public DbHovedstrekning() { } // Ma ogsa ha en parameterlos konstruktor. Vet ikke hvorfor, men sann er det.
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
            public DbStasjon(string navn, DbNett nett, string optStedNavn="")
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}