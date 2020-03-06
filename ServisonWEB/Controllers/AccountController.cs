using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Default.Models;
using Default.Models.AccountViewModels;
using Default.Services;
using Admin.Services;
using System.Reflection;
using System.Diagnostics;

namespace Default.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private Stopwatch s = new Stopwatch();

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name, MethodBase.
                GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.
                    Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return RedirectToAction(nameof(LoginWith2fa), new
                    {
                        returnUrl,
                        model.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name, MethodBase.
                        GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                        await _userManager.AccessFailedAsync(user);
                    ModelState.AddModelError(string.Empty, "Niepoprawne dane logowania.");
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return View(model);
                }
            }

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name, MethodBase.
                GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Nie udało się odnaleźć użytkownika.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name, MethodBase.
                GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
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

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Nie udało sie odnaleźć użytkow" +
                    $"nika o ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.
                Empty).Replace("-", string.Empty);

            var result = await _signInManager.
                TwoFactorAuthenticatorSignInAsync(authenticatorCode,
                rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2f" +
                    "a.", user.Id);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user" +
                    " with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Niepoprawne dane logowania.");
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Nie udało się odnaleźć użytkownika.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
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

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Nie udało się odnaleźć użytkownika.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a " +
                    "recovery code.", user.Id);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with" +
                    " ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Niepoprawny kod bezpieczeństwa.");
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    if (returnUrl == null)
                    {
                        IndexViewModel modelIndex = new IndexViewModel();
                        modelIndex.StatusMessage = "Konto zostało utworzone. Sprawdź maila, aby aktywować.";
                        s.Stop();
                        LoggerController.AddEndMethodLog(this.GetType().Name,
                            MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                        return RedirectToAction("Index","Home", modelIndex);
                    }
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }
            else
            {
                return View(model);
            }

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        s.Stop();
                        LoggerController.AddEndMethodLog(this.GetType().Name,
                            MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (userId == null || code == null)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    s.Stop();
                    LoggerController.AddEndMethodLog(this.GetType().Name,
                        MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Przywracanie hasła",
                   $"Aby zresetować hasło proszę kliknąć w <a href='{callbackUrl}'>link</a>");
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                s.Stop();
                LoggerController.AddEndMethodLog(this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
