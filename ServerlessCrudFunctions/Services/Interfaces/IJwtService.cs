using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerlessCrudFunctions.Services.Interfaces
{
    public interface IJwtService
    {
        Task<ClaimsPrincipal> GetClaimsPrincipalAsync(HttpRequest request);
    }
}
