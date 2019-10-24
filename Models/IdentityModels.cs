using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrackerEnabledDbContext.Identity;

namespace VyBillettBestilling.Models
{
    [TrackChanges]
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Fornavn", this.Fornavn));
            userIdentity.AddClaim(new Claim("Etternavn", this.Etternavn));
            userIdentity.AddClaim(new Claim("Fodselsdato", this.Fodselsdato.ToString()));
            if (this.Betalingskort != null)
            {
                userIdentity.AddClaim(new Claim("Betalingskort", this.Betalingskort));
            }
            else
            {
                userIdentity.AddClaim(new Claim("Betalingskort", ""));
            }
            if (this.Mobilnummer != null)
            {
                userIdentity.AddClaim(new Claim("Mobilnummer", this.Mobilnummer));
            }
            else
            {
                userIdentity.AddClaim(new Claim("Mobilnummer", ""));
            }
            if (this.Betalingskort != null)
            {
                userIdentity.AddClaim(new Claim("Betalingskort", this.Betalingskort));
            }
            else
            {

                userIdentity.AddClaim(new Claim("Betalingskort", ""));
            }
            return userIdentity;
        }
        public String Fodselsdato { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Mobilnummer { get; set; }
        public string Betalingskort { get; set; }
    }

    public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("BrukerBase")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}