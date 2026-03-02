using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class KeycloakRoleTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null) return Task.FromResult(principal);

            var realmAccessClaim = principal.FindFirst("realm_access");
            if (realmAccessClaim != null)
            {
                var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
                if (realmAccess.RootElement.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                    }
                }
            }

            return Task.FromResult(principal);
        }
    }
}