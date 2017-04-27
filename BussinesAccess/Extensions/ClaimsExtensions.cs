using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BussinesAccess.Extensions
{
    public static class ClaimsExtensions
    {

        public static string Find(this ClaimsPrincipal principal ,string type)
        {
            var claim = principal.Claims.FirstOrDefault(c => c.Type == type);

            return claim != null ? claim.Value : string.Empty;


        }
    }
}
