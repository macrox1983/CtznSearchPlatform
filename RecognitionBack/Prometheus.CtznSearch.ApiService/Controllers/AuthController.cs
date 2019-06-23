using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Presentation;
using Prometheus.Services;

namespace Prometheus.Controllers
{
    //public class AuthController : Controller
    //{
    //    private readonly IAuthService _authenticationService;

    //    public AuthController(IAuthService authenticationService)
    //    {
    //        _authenticationService = authenticationService??throw new ArgumentNullException(nameof(authenticationService));
    //    }

    //    [HttpGet]
    //    public IActionResult Login()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Login(LoginVm model)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var claimPrincipal = await _authenticationService.Authenticate(model.Login, model.Password);
    //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);
    //            return RedirectToAction("Index", "Home");
    //        }
    //        return View(model);
    //    }



    //    [Authorize]
    //    public async Task<IActionResult> Logout()
    //    {
    //        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //        return RedirectToAction("Login", "Auth");
    //    }

    //    public override RedirectResult Redirect(string url)
    //    {
    //        if (new Uri(url).IsAbsoluteUri)
    //            throw new Exception("Неверный url для перенапраления");
    //        return base.Redirect(url);
    //    }
    //}
}