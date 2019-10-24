﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Passordet er endret."
                : message == ManageMessageId.Error ? "Det har skjedd en feil. =("
                : "";

            var userId = User.Identity.GetUserId();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                Fornavn = user.Fornavn,
                Fodselsdato = user.Fodselsdato.ToString(),
                Etternavn = user.Etternavn,
                Mobilnummer = user.Mobilnummer,
                Betalingskort = user.Betalingskort
            };
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult> Administrasjon()
        {
            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
            };
            return View(model);
        }
        [Authorize(Roles="Administrator")]
        [HttpGet]
        public ActionResult PriserOgPassasjerer()
        {
            var dbt = new VyDbTilgang();
            var viewModel = new Models.View.PrisOgBillett();
            viewModel.Passasjerer = dbt.HentPassasjerTyper();
            viewModel.Pris = dbt.HentPris();
            return View(viewModel);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StrekningsListe()
        {
            var dbt = new VyDbTilgang();
            var strekninger = dbt.HentAlleHovedstrekninger();
            return View(strekninger);

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StrekningCreate()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult StrekningCreate(Hovedstrekning hvst)
        {
            if (ModelState.IsValid)
            {

                var dbt = new VyDbTilgang();
                dbt.leggTilHovedstrekning(hvst);
                return RedirectToAction("StrekningsListe", "Manage");
            }
            else
                return View(hvst);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StrekningEdit(int Id)
        {
            var dbt = new VyDbTilgang();
            var strekning = dbt.HentHovedstrekning(Id);
            return View(strekning);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult StrekningEdit(Hovedstrekning str, int Id)
        {
            if (ModelState.IsValid)
            {
                var dbt = new VyDbTilgang();
                dbt.fjernHovedstrekning(Id);
                str.id = Id;
                dbt.leggTilHovedstrekning(str);
                return RedirectToAction("StrekningsListe", "Manage");
            }
            else
                return View(str);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StrekningDetails(int Id)
        {
            var dbt = new VyDbTilgang();
            var strekning = dbt.HentHovedstrekning(Id);
            return View(strekning);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StrekningDelete(int Id)
        {
            var dbt = new VyDbTilgang();
            dbt.fjernHovedstrekning(Id);
            return RedirectToAction("StrekningsListe", "Manage");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult NettListe()
        {
            var dbt = new VyDbTilgang();
            var alleNett = dbt.HentAlleNett();
            return View(alleNett);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult NettEdit()
        {
            return View();
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult NettCreate()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult NettCreate(Nett nett)
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult NettDelete()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StasjonsListe()
        {
            var dbt = new VyDbTilgang();
            var alleStasjoner = dbt.HentAlleStasjoner();
            return View(alleStasjoner);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult StasjonDetails(int Id)
        {
            var dbt = new VyDbTilgang();
            return View(dbt.HentStasjon(Id));
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}