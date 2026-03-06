using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly EmailHelper _emailHelper;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EmailHelper emailHelper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailHelper = emailHelper;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User { Email = model.Email, UserName = model.Email, Year = model.Year };
            // добавляем пользователя
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // установка куки
                //await _signInManager.SignInAsync(user, false);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                var emailResult = await _emailHelper.SendEmailRegistrationConfirm(user.Email, confirmationLink);
                if (!emailResult)
                {
                    return RedirectToAction(nameof(Error));
                }


                return RedirectToAction("Index", "Home");
            }


            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return View(model);
    }

    [HttpGet]

    public async Task<IActionResult> Login(string returnUrl = null)

    {

        LoginViewModel model = new LoginViewModel

        {

            ReturnUrl = returnUrl,

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

        };

        return View(model);

    }


    //[HttpGet]
    //public IActionResult Login(string returnUrl = null)
    //{
    //    return View(new LoginViewModel { ReturnUrl = returnUrl });
    //}

    [HttpPost]
    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });

        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // проверяем, принадлежит ли URL приложению
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Invalid login attempt");
            }
            //else if (!await _userManager.IsEmailConfirmedAsync(new User { Email = model.Email }))
            //{
            //    ModelState.AddModelError("", " You have not confirmed the Email address.");
            //}
            else
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
        }
        return View(model);
    }
    public async Task<IActionResult>
            ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        LoginViewModel loginViewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins =
                    (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        };

        if (remoteError != null)
        {
            ModelState
                .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

            return View("Login", loginViewModel);
        }

        // Get the login information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState
                .AddModelError(string.Empty, "Error loading external login information.");

            return View("Login", loginViewModel);
        }

        // If the user already has a login (i.e if there is a record in AspNetUserLogins
        // table) then sign-in the user with this external login provider
        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
            info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        // If there is no record in AspNetUserLogins table, the user may not have
        // a local account
        else
        {
            // Get the email claim value
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                // Create a new user without password if we do not have a user already
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };

                    await _userManager.CreateAsync(user);
                }

                // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
            ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("Error");
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return View(result.Succeeded ? nameof(ConfirmEmail) : nameof(Error));
    }

    [HttpGet]
    public IActionResult SuccessRegistration()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Error()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword([Required] string email)
    {
        if (!ModelState.IsValid)
            return View(model: email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return RedirectToAction(nameof(ForgotPassword));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

        bool emailResponse = _emailHelper.SendEmailPasswordReset(user.Email, link);

        if (emailResponse)
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        else
        {
            // log email failed 
            ModelState.AddModelError("", "Произошла ошибка, попробуйте позже.");
        }
        return View(model: email);
    }



    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    public IActionResult ResetPassword(string token, string email)
    {
        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
    {
        if (!ModelState.IsValid)
            return View(resetPassword);

        var user = await _userManager.FindByEmailAsync(resetPassword.Email);
        if (user == null)
            RedirectToAction(nameof(ResetPasswordConfirmation));

        var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
        if (!resetPassResult.Succeeded)
        {
            foreach (var error in resetPassResult.Errors)
                ModelState.AddModelError(error.Code, error.Description);
            return View(resetPassword);
        }

        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // удаляем аутентификационные куки
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

