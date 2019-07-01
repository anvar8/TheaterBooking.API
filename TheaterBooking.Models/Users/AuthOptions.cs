using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheaterBooking.Models.Users
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // token issuer
        public const string AUDIENCE = "http://localhost:3000/"; // token consumer
        private const string KEY = "8871fdea-9bc4-4abe-a732-857b65c449eb"; // secret key
        public const int LIFETIME = 1; // expires in 1 days

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
