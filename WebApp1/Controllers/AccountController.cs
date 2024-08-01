using Microsoft.AspNetCore.Mvc;
using Login.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApp1.Controllers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Iemailsender.Services;
using Emailsender.Services;

using System.Text.Encodings.Web;
using ResetPassword.Models;// Replace with your actual namespace
using ForgotPassword.Models;// Replace with your actual namespace

namespace Login.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICustomEmailSender _emailSender;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ICustomEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmEmailConfirmation()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Craft the email body with a company intro and a confirmation link
                    var emailBody = $@"
                <h1>Welcome to Tech Mart!</h1>
                <p>Dear {model.Email},</p>
                <p>Discover the latest in electronics, from cutting-edge gadgets to everyday essentials. Enjoy great deals and fast shipping!</p>
                <p>To complete your registration, please confirm your email address by clicking the link below:</p>
                <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here to confirm your email</a></p>
                <p>Best regards,<br>The Tech Mart Team</p>";

                    await _emailSender.SendEmailAsync(model.Email, "Register with Tech Mart", emailBody);

                    return RedirectToAction(nameof(RegisterConfirmation));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ConfirmEmailConfirmation));
            }
            return RedirectToAction(nameof(Index), "Home");
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    
                    TempData["UserEmail"] = user?.Email;

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Do not reveal that the user does not exist or email is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                // Craft the email body with a company intro and a password reset link
                var emailBody = $@"
            <h1>Welcome to Tech Mart!</h1>
            <p>Dear {model.Email},</p>
            <p>Discover the latest in electronics, from cutting-edge gadgets to everyday essentials. Enjoy great deals and fast shipping!</p>
            <p>To reset your password, please click the link below:</p>
            <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here to reset your password</a></p>
            <p>If you did not request a password reset, please ignore this email.</p>
            <p>Best regards,<br>The Tech Mart Team</p>";

                await _emailSender.SendEmailAsync(model.Email, "Reset your password", emailBody);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }




        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? BadRequest("A code must be supplied for password reset.") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
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
    }
}
