using System.Linq;
using System.Security.Claims;

namespace WebApplication1.Controllers.Utils
{
    public static class ControllerClaimsExtensions
    {
        public static string GetUid(this ClaimsPrincipal user) => user.Claims
                                                                .Where(c => c.Type == "uid")
                                                                .Select(c => c.Value)
                                                                .FirstOrDefault();
    }
}
