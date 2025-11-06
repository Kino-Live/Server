using Microsoft.Extensions.Options;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Settings;
using System.Net;
using System.Net.Mail;

namespace ProjectCinema.BLL.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public SmtpEmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.From))
            {
                throw new InvalidOperationException("Email.From is not configured");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            // SmtpClient doesn't support CancellationToken directly; best-effort only
            await client.SendMailAsync(message);
        }
    }
}


