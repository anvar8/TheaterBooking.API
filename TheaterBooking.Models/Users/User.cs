using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheaterBooking.Models.Users
{
    public class User : IdentityUser
    {
        [NotMapped]
        public string[] Roles { get; set; }

        [JsonIgnore]
        public override string NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }

        [JsonIgnore]
        public override string NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }

        [JsonIgnore]
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        [JsonIgnore]
        public override string SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }

        [JsonIgnore]
        public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }

        [JsonIgnore]
        public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }

        [JsonIgnore]
        public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }

        [JsonIgnore]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }

        [JsonIgnore]
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }

        [JsonIgnore]
        public override bool TwoFactorEnabled { get => base.TwoFactorEnabled; set => base.TwoFactorEnabled = value; }
    }
}
