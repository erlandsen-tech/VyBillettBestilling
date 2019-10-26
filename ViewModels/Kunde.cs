using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.ViewModels
{
    public class Kunde : IdentityUser
    {
        [Display(Name = "E-Post")]
        [EmailAddress(ErrorMessage = "En gyldig E-post adresse må oppgis")]
        [Required(ErrorMessage = "En gyldig E-post adresse må oppgis")]
        public String Epost { get; set; }
        [Display(Name = "Fødselsdato")]
        [Required(ErrorMessage = "Fødselsdato må oppgis")]
        public String Foedt { get; set; }
        [Display(Name = "Fornavn")]
        [Required(ErrorMessage = "Fornavn må oppgis")]
        public String Fornavn { get; set; }
        [Display(Name = "Etternavn")]
        [Required(ErrorMessage = "Etternavn må oppgis")]
        public String Etternavn { get; set; }
        [Display(Name = "Telefonnummer")]
        [Required(ErrorMessage = "Vi trenger ditt telefonnummer")]
        public String Mobilnr { get; set; }
        [Display(Name = "Betalingskort")]
        [Required(ErrorMessage = "Et gyldig betlaingskort må oppgis")]
        public String Bet_kort { get; set; }
    }
}