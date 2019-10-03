using System;
using System.ComponentModel.DataAnnotations;

namespace VyBillettBestilling.Models
{
    public class Stasjon
    {
        public int id { get; set; }

        [Display(Name = "Stasjonnavn")]
        public String stasjon_navn { get; set; }
        [Display(Name = "Stasjonsted")]
        public String stasjon_sted { get; set; }

        [Display(Name = "Breddegrad")]
        [Range(-90, 90, ErrorMessage = "Ugyldig koordinat; -90 <= Breddegrad <= 90")]
        public double breddegrad { get; set; }
        [Display(Name = "Lengdegrad")]
        [Range(-180, 180, ErrorMessage = "Ugyldig koordinat; -180 <= Lengdegrad <= 180")]
        public double lengdegrad { get; set; }
    }

}