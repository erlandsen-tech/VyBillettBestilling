using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Pris
    {
        public int Id { get; set; }
        [Display(Name = "Pris pr km")]
        [Required(ErrorMessage = "Det må settes en pris.")]
        [Range(1, double.MaxValue, ErrorMessage = "Vennligst sett inn en pris over 1 og mindre enn 1.79769313486232E+308")]
        public double prisPrKm { get; set; }
    }
}