﻿using System;
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

        public DbSet<DbBillett> Billetter { get; set; } // Kunde, start- og stopp-stasjon, passasjertype, pris (med rabattsats)
        // stasjonsliste(?), distanse(?)
        // Merk: Ma lagre pris (og ev. rabatt og passasjertype?) i Billett-en, siden de kan endre seg etter kjopet.

        public DbSet<DbKunde> Kunder { get; set; } // Navn, adresse, tlf o.l.
        public DbSet<DbPassasjertype> Passasjertyper { get; set; } // Ordinaer, student, honnor osv. med tilhorende rabatter
        
        // Tabeller for linjenettet:
        public DbSet<DbStasjon> Stasjoner { get; set; }
        // Opplisting av stasjoner, ev. med data om plattformer, geografisk plassering og annet (betjent?, toalett? osv.)
        // Ogsa data om Hovedstrekning(-er) den tilhorer. (Geo.pos. gir opplosning pa <= 1,1 m med 5 desimaler, det er nok.)
        public DbSet<DbDelstrekning> Delstrekninger { get; set; } // Avstand (og pris?) mellom to nabostasjoner. Forelopig ikke i bruk.
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

        public class DbNett
        {
            [Key]
            public int NettId { get; set; }
            public String Nettnavn { get; set; } // Sor-Norge, Ofoten, Japan osv.
            public virtual List<DbHovedstrekning> Hovedstrekninger { get; set; }
            public virtual List<DbStasjon> Stasjoner { get; set; }
        }
        
        public class DbHovedstrekning
        {
            [Key]
            public int HovstrId { get; set; }
            [Required]
            public virtual DbNett Nett { get; set; } // DbHovedstrekning-er pa samme Nett er forbundet med hverandre med et antall DbHovedstrekninger
            public virtual List<DbStasjon> Stasjoner { get; set; }
            public virtual List<DbDelstrekning> Delstrekninger { get; set; }
        }

        public class DbDelstrekning
        {
            [Key]
            public int DelstrId { get; set; }
            [Required]
            public double Distanse { get; set; }
            [Required]
            public virtual DbHovedstrekning Hovedstrekning { get; set; }
            [Required]
            public virtual DbStasjon Start { get; set; }
            [Required]
            public virtual DbStasjon Stopp { get; set; }
        }

        public class DbStasjon
        {
            [Key]
            public int StasjonId { get; set; }
            [Required]
            public String StasjNavn { get; set; }
            [Range(-90, 90)]
            public double Breddegrad { get; set; }
            [Range(-180, 180)]
            public double Lengdegrad { get; set; }

            //public int NettId { get; set; } // virtual DbNett, og ha en liste av stasjoner i DbNett?
            public virtual DbNett nettet { get; set; }


            // Ha denne virtual eller ikke? I stedet vise til Hovedstrekningen med en fremmednokkel?
            // Nei, det kan vaere flere, matte da ha sammenknytningstabell
            //public virtual List<DbHovedstrekning> Hovedstrekninger { get; set; } // De fleste stasjoner har en i denne lista.
            // De som har >= 2 utgjor sammenknytninger mellom DbHovedstrekninger,
            // og er samtidig endepunkt for de tilknyttede hovedstrekningene

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


        public class DbPassasjertype
        {
            [Key]
            public int PasstypId { get; set; }
            [Required] 
            public String TypeNavn { get; set; } // Ordinaer, student, barn, honnor, verneplikt, avtale(navn) m/prosent, osv.
            [Range(0, 100, ErrorMessage = "Rabatt min. 0 %, max 100 %")] // Gjor dette verdien Required ?
            public double Rabatt { get; set; }
            public int OvreAldersgrense { get; set; }
            public int NedreAldersgrense { get; set; }
        }

        public class DbKunde
        {
            [Key]
            public int Id { get; set; }
            //[Required] // [EmailAddress] // ?
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
            public String Bet_kort { get; set; } // Skal lagres som sifferstreng, uten mellomrom (?)
            public virtual List<DbBillett> Billetter { get; set; }
        }

        public class DbBillett
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}