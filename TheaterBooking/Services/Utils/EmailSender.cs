using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheaterBooking.Models;
using TheaterBooking.Models.Users;

namespace TheaterBooking.Services
{
    public class EmailSender
    {
        private readonly EmailSenderOptions _options;
        private readonly HttpContextService _httpContextService;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Для переноса такста на новую строку.
        /// </summary>
        private const string NewLine = "<br />";

        public EmailSender(IOptions<EmailSenderOptions> options, HttpContextService httpContextService, UserManager<User> userManager)
        {
            _options = options.Value;
            _httpContextService = httpContextService;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlContent)
        {
            var apiKey = _options.ApiKey;

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_options.EmailAddress, _options.Name);

            var to = new EmailAddress(email);

            var plainTextContent = Regex.Replace(htmlContent, "<[^>]*>", "");

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(msg);

        }

        public async Task SendSuccessfullyAddedBookingEmail(string email)
        {
            // @TODO Fix text properly.
            const string subject = "Booked successfully";

            var content = $"Your booking is successfully added";

            await SendEmailAsync(email, subject, content);
        }

        public async Task SendBookingDateChanged (string email, string showTimeName, DateTime newDate)
        {
            const string subject = "Changed booking date";

            var content = $"Booking for {showTimeName} has its date changed to {newDate.ToString()}";

            await SendEmailAsync(email, subject, content);
        }


        public async Task SendBookingCanceled(string email, string showTimeName, DateTime showDate)
        {
            const string subject = "Show cancelled";

            var content = $"Booking for {showTimeName} has been cancelled at {showDate}.";

            await SendEmailAsync(email, subject, content);
        }

        public async Task SendConfirmationEmail(User user, string code)
        {
            const string Subject = "Successfully registered to Theater Booking";

            var uri = _httpContextService.GetHostUrl();

            var relativePath = "/ConfirmEmail";

            uri += relativePath;

            uri = QueryHelpers.AddQueryString(uri, "userId", user.Id);
            uri = QueryHelpers.AddQueryString(uri, "token", code);

            var content = $"Connfirm your email. Click on link. {NewLine} " +
                $"</a> {uri} </a>";

            await SendEmailAsync(user.Email, Subject, content);

        }
    }
}

