using System;
using System.Collections.Generic;
using System.Text;

namespace TheaterBooking.Models.Users
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string Email { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        public string Id { get; set; }
    }
}
