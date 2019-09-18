using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Stasjon
    {
        public int id { get; set; }

        // [Required(ErrorMessage = "")]
        [Display(Name = "Stasjonnavn")]
        public String stasjon_navn { get; set; }
        // Kombinasjonen av stasjonnavn og stasjonsted forutsettes unik pa hvert nett.
        // Ved like StasjNavn pa et nett ma det tilfoyes StasjSted pa alle (eventuelt unntatt ett)
        [Display(Name = "Stasjonsted")]
        public String stasjon_sted { get; set; }

        [Display(Name = "Breddegrad")]
        [Range(-90, 90, ErrorMessage = "Ugyldig koordinat; -90 <= Breddegrad <= 90")]
        public double breddegrad { get; set; }
        [Display(Name = "Lengdegrad")]
        [Range(-180, 180, ErrorMessage = "Ugyldig koordinat; -180 <= Lengdegrad <= 180")]
        public double lengdegrad { get; set; }

        public int nett_id { get; set; }
        public String nett_navn { get; set; }
        public IEnumerable<int> hovedstrekninger { get; set; } // De fleste stasjoner har en i denne lista.
        // De som har >= 2 utgjor sammenknytninger mellom DbHovedstrekninger,
        // og er samtidig endepunkt for de tilknyttede hovedstrekningene
    }
}