using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.ViewModels
{
    public class HovedstrekningCreateView
    {

        [Required(ErrorMessage = "Hovedstrekningen må ha et navn")]
        [Display(Name = "Navn")]
        public string hovstr_navn { get; set; }
        [Required(ErrorMessage = "Hovedstrekningen trenger et kort identifikator navn")]
        [Display(Name = "Kortnavn")]
        public string hovstr_kortnavn { get; set; }
        [Required(ErrorMessage = "Hovedstrekningen må knyttes til et nett")]
        [Display(Name = "Nett ID")]
        public String nettid { get; set; }
        public List<string> stasjonsliste{ get; set; }
    }
}