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
        public DbSet<DbPassasjertype> Passasjertyper { get; set; } // Ordinaer, student, honnor osv. med tilhorende rabatter
        public DbSet<DbStasjon> Stasjoner { get; set; }
        public class DbStasjon
        {
            [Key]
            public int StasjonId { get; set; }

            [Required]
            public String StasjNavn { get; set; }
            // Kombinasjonen av stasjonnavn og stasjonsted forutsettes unik pa hvert nett.
            // Ved like StasjNavn pa et nett ma det tilfoyes StasjSted pa alle (eventuelt unntatt ett)
            public String StasjSted { get; set; }
            [Range(-90, 90, ErrorMessage = "Ugyldig koordinat; -90 <= Breddegrad <= 90")]
            public double Breddegrad { get; set; }
            [Range(-180, 180, ErrorMessage = "Ugyldig koordinat; -180 <= Lengdegrad <= 180")]
            public double Lengdegrad { get; set; }
        }

        public class DbPassasjertype
        {
            [Key]
            public int PasstypId { get; set; }

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
            public int KundeId { get; set; }
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
            public int BillKjopId { get; set; }
            [Required]
            public int KundeId { get; set; }
            public virtual DbKunde Kunde { get; set; }
            public virtual List<DbBillett> Billetter { get; set; }

            // [CreditCard] // ?
            public String Bet_kort { get; set; } // Skal lagres som sifferstreng uten mellomrom(?). Kort til hvert kjop ma lagres, siden kundens kort kan forandre seg
            public DateTime Kjopsdato { get; set; }
        }

        public class DbBillett
        {
            [Key]
            public int BillId { get; set; }
            [Required]
            public int BillKjopId { get; set; }
            public virtual DbBillettKjop Kjop { get; set; }

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
        //public class DbBillett
        //{
        //    [Key]
        //    public int BillId { get; set; }
        //    [Required]
        //    public int KundeId { get; set; }
        //    public virtual DbKunde Kunde { get; set; }

        //    // Mest hensiksmessig a lagre tekstverdiene som skal sta pa billetten i DbBillett-en
        //    public String StartStasjon { get; set; }
        //    public String StoppStasjon { get; set; }
        //    // Pris, passasjertype og rabattsats ma fikseres i billetten, siden de kan endre seg etter kjopet.
        //    public double Pris { get; set; }
        //    public String Passasjertype { get; set; }
        //    public double Rabattsats { get; set; }
        //    public DateTime Kjopsdato { get; set; }

        //}


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}