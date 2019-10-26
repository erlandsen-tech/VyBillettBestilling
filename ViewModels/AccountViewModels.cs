using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VyBillettBestilling.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Husk meg på denne maskinen")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Display(Name = "Husk meg på denne maskinen")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [EmailAddress(ErrorMessage = "En gyldig E-post adresse må oppgis")]
        [Required(ErrorMessage = "En gyldig E-post adresse må oppgis")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fornavn må oppgis")]
        [Display(Name = "Fornavn")]
        public string Fornavn { get; set; }

        [Required(ErrorMessage = "Etternavn må oppgis")]
        [Display(Name = "Etternavn")]
        public string Etternavn { get; set; }

        [Required(ErrorMessage = "Vi trenger din fødselsdato for å gi deg rabatt")]
        [Display(Name = "Fødselsdato")]
        public String Fodselsdato { get; set; }

        [Display(Name = "Betalingskort")]
        public string Betalingskort { get; set; }

        [Display(Name = "Mobilnummer")]
        public string Mobilnummer { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "{0}et må være minst {2} tegn langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Gjenta passord")]
        [Compare("Password", ErrorMessage = "Passordene er ikke like.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}et må være minst {2} tegn langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Gjenta passord")]
        [Compare("Password", ErrorMessage = "Passordene er ikke like.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }
}
