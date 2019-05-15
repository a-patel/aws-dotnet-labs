using AwsAspNetCoreLabs.Models;
using AwsAspNetCoreLabs.Models.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;

namespace AwsAspNetCoreLabs.SES
{
    public class SmtpNoteEmailService : INoteEmailService
    {
        private SmtpSettings _settings;
        private readonly ILogger<SmtpNoteEmailService> _logger;

        public SmtpNoteEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpNoteEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public void SendNote(string email, Note note)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
                emailMessage.To.Add(new MailboxAddress(email));
                emailMessage.Subject = $"AwsAspNetCoreLabs. - {note.Title}";
                emailMessage.Body = new TextPart("plain") { Text = note.Content };

                using (var client = new SmtpClient())
                {
                    client.LocalDomain = _settings.LocalDomain;
                    client.Connect(_settings.SmtpHost, _settings.Port, SecureSocketOptions.StartTls);
                    client.Authenticate(_settings.Username, _settings.Password);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't send an email");
                throw;
            }
        }
    }
}
