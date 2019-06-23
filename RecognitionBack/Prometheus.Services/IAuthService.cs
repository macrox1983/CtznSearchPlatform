using System.Security.Claims;
using System.Threading.Tasks;

namespace Prometheus.Services
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal> Authenticate(string login, string pass);
    }
}