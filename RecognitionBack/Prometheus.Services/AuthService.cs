using Prometheus.Infrastructure.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Prometheus.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository??throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ClaimsPrincipal> Authenticate(string login, string pass)
        {
            var user = await _userRepository.FindByLogin(login);
            if (user == null)
                throw new Exception("Некорректный пользователь или пароль");
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                //new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.RoleName)
            };
            // создаем объект ClaimsIdentity
            //ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            ClaimsIdentity id = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return new ClaimsPrincipal(id);            
        }
    }
}
