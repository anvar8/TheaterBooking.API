using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheaterBooking.Data;
using TheaterBooking.Models.Users;

namespace TheaterBooking.Services
{
    public class UsersService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly HttpContextService _httpContext;

        private readonly EmailSender _emailSender;

        public UsersService(ApplicationDbContext context, UserManager<User> userManager,
            HttpContextService httpContext, EmailSender emailSender
            )
        {
            _context = context;
            _userManager = userManager;
            _httpContext = httpContext;
            _emailSender = emailSender;
        }

        public async Task RegisterAsync(UserDto userDto)
        {
            var user = new User
            {
                Email = userDto.Email,
                UserName = userDto.Email 
            };

            if (await _userManager.FindByEmailAsync(user.Email) != null)
                throw new ArgumentException("User exists with this login");

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ArgumentException("Registration failed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _userManager.AddToRoleAsync(user, AuthConstants.Client);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailSender.SendConfirmationEmail(user, code);
        }

        public async Task<TokenResponse> LoginAsync(UserDto login)
        {
            var identity = await GetIdentityAsync(login);
            if (identity == null)
                throw new ArgumentException("Login failed. Check your login and password");

            var user = await _userManager.FindByEmailAsync(login.Email);

            // check if email is confirmed
            //var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            //if (!emailConfirmed)
            //    throw new ArgumentException("Please confirm email");

            var now = DateTime.Now;

            var jwt = new JwtSecurityToken(
                AuthOptions.ISSUER,
                AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new TokenResponse
            {
                Id = user.Id,
                AccessToken = encodedJwt,
                Email = identity.Name,
                User = user,
                UserId = user.Id
            };
            return token;
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(UserDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
                return null;

            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!passwordVerificationResult)
                return null;

            var userClaims = await _userManager.GetClaimsAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(AuthConstants.IdClaimType, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            claims.AddRange(userRoles.Select(c => new Claim(ClaimsIdentity.DefaultRoleClaimType, c)));

            user.Roles = userRoles.ToArray();

            claims.AddRange(userClaims);

            return new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        }

        public async Task UpdateUserRolesAsync(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found");

            // remove all roles
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }

            // add role
            var addResult = await _userManager.AddToRoleAsync(user, role);

            if (!addResult.Succeeded)
                throw new ArgumentException("Failed to assign role");
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync(string userName, string role)
        {
            var users = _userManager.Users.AsQueryable();

            users = userName == null ? users : users.Where(u => u.Email.Contains(userName));

            var usersInRole = role == null ? null : await _userManager.GetUsersInRoleAsync(role);

            if (!_httpContext.IsAdmin())
                throw new UnauthorizedAccessException("Unauthorized.");

            if (usersInRole != null)
            {
                users = users.Intersect(usersInRole);
            }
            return users.ToArray();
        }

        public async Task ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                throw new InvalidOperationException("Email confirmation failed");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (!user.EmailConfirmed)
            {
                var confirmationResult = await _userManager.ConfirmEmailAsync(user, token);
                if (!confirmationResult.Succeeded)
                    throw new InvalidOperationException("Email confirmation failed.");
            }

        }
    }
}
