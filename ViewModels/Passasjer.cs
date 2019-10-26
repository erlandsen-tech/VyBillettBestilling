using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.ViewModels
{
    public class Passasjer
    {
        public int ptypId { get; set; }
        [Display(Name = "Rabattprosen")]
        [Required(ErrorMessage = "Det må settes en verdi.")]
        [Range(0, 100, ErrorMessage = "Sett inn en rabattsats mellom 0 og 100")]
        public double rabatt { get; set; }
        [Display(Name = "Navn")]
        [Required(ErrorMessage = "Rabattkategorien må ha et navn.")]
        public string typenavn { get; set; }
        [Display(Name = "Øvre aldersgrense")]
        [Required(ErrorMessage = "Det må settes en øvre aldersgrense")]
        [Range(0, 1000, ErrorMessage = "Sett inn en alder mellom 0 og 1000")]
        public int ovreAlder { get; set; }
        [Display(Name = "Nedre aldersgrense")]
        [Required(ErrorMessage = "Det må settes en nedre aldersgrense.")]
        [Range(0, 1000, ErrorMessage = "Sett inn en alder mellom 0 og 1000")]
        public int nedreAlder { get; set; }
    }
}