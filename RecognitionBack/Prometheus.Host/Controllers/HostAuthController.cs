using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Prometheus.Presentation;
using Prometheus.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Host.Controllers
{
    public class AuthOptions
    {
        public const string ISSUER = "Prometheus.Component.Authentication"; // издатель токена
        public const string AUDIENCE = "https://localhost/"; // потребитель токена
        const string KEY = "HJfd/sgh/fGFghf3l;k/l4DHF!ds.,Clksdl";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }

    //[Microsoft.AspNetCore.Mvc.Route("api")]
    public class HostAuthController : Controller
    {
        private readonly IAuthService _authenticationService;

        public HostAuthController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.Route("Login")]
        public async Task Login(LoginVm model)
        {
            if (ModelState.IsValid)
            {
                var identity = await _authenticationService.Authenticate(model.Login, model.Password);

                if (identity == null)
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Invalid username or password.");
                    return;
                }

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = model.Login
                };

                // сериализация ответа
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, claimPrincipal);
                //return RedirectToAction("Index", "Home");
            }
            //return View(model);
        }        
    }
}
