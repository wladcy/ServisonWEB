using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Default.Models;
using Default.Models.ManageViewModels;
using Default.Services;
using Admin.Services;
using System.Reflection;
using System.Diagnostics;

namespace Default.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        private Stopwatch s = new Stopwatch();

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nie odnaleziono użytkownika o ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new Models.ManageViewModels.IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Models.ManageViewModels.IndexViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nie odnaleziono użytkownika o ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Profil został zaktualizowany";
            
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(Models.ManageViewModels.IndexViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Mail weryfikacyjny został wysłany. Sprawdź swojego maila.";
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Hasło zostało zmienione.";

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (!ModelState.IsValid)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Hasło zostało ustawione.";

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "Połączenie z zewnętrznym systemem zostało dodane.";
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Połączenie z zewnętrznym systemem zostało usunięte.";
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Kod weryfikacyjny jest niepoprawny.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled.");
            }
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(nameof(ShowRecoveryCodes), model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Default"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}
