using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;

namespace DealerLead.Web
{
    public static class AuthenticationHelper
    {
        private static DealerLeadDBContext _context;

        static AuthenticationHelper()
        {
            _context = new DealerLeadDBContext();
        }

        public static Guid GetOid(System.Security.Claims.ClaimsPrincipal principal)
        {
            var identity = principal.Identities.FirstOrDefault();
            var claims = identity.Claims;
            var thisClaim = claims.FirstOrDefault(claim => claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier");
            var oid = Guid.Parse(thisClaim.Value);
            return oid;
        }

        public async static Task OnTokenValidatedFunc(TokenValidatedContext context)
        {
            var oid = GetOid(context.Principal);

            var user = _context.DealerLeadUser.FirstOrDefault(u => u.AzureADId.Equals(oid));

            if (user == null)
            {
                var newUser = new DealerLeadUser() { AzureADId = oid };
                _context.Add(newUser);
                await _context.SaveChangesAsync();
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
