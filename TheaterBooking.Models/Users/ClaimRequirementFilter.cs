using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace TheaterBooking.Models.Users
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasAdminClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == AuthConstants.Administrator);
            if (hasAdminClaim) 
                return;

            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim) 
            {
                context.Result = new UnauthorizedResult();                
            } 
        }


    }

}


