using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheaterBooking.Models.Users;

namespace TheaterBooking.Services
{
    public class HttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<User> _userManager;

        private HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public HttpContextService(IHttpContextAccessor httpContext, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContext;
            _userManager = userManager;
        }

        public async Task<IdentityUser> GetUserByContextAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user == null)
                throw new ArgumentException("Not Authorized.");

            return user;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles;

        }

        public bool IsClient()
        {
            return _httpContextAccessor.HttpContext.User.IsInRole(AuthConstants.Client);
        }


        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext.User.IsInRole(AuthConstants.Administrator);
        }

        public string GetUserId()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return null;

            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == AuthConstants.IdClaimType)?.Value;

            return id;
        }

        public string GetUserEmail()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return null;

            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            return email;
        }

        public async Task<string> GetClientEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user.Email;
        }

        public string GetHostUrl()
        {
            var url = UriHelper.GetDisplayUrl(HttpContext.Request);
            return url;
        }

    }
}
