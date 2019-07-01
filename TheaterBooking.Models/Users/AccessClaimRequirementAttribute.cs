using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace TheaterBooking.Models.Users
{
    public class AccessClaimRequirementAttribute : TypeFilterAttribute
    {
        public AccessClaimRequirementAttribute(string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(AuthConstants.ClaimType, claimValue) };
        }
    }
}
