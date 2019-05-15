using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsAspNetCoreLabs.Models;
using AwsAspNetCoreLabs.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Amazon.SimpleEmail;
using Amazon;
using Amazon.SimpleEmail.Model;

namespace AwsAspNetCoreLabs.SES
{
    public class SESNoteEmailService : INoteEmailService
    {
        private readonly SESSettings _settings;
        private readonly ILogger<SESNoteEmailService> _logger;
        private readonly AmazonSimpleEmailServiceClient _client;
        private readonly string _fromAddress;

        public SESNoteEmailService(IOptions<SESSettings> settings, ILogger<SESNoteEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            try
            {
                _client = new AmazonSimpleEmailServiceClient(RegionEndpoint.GetBySystemName(settings.Value.AWSRegion));

                _fromAddress = $"\"{_settings.FromName}\" <{_settings.FromAddress}>";
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't create an instance of SESNoteEmailService");
                throw;
            }
        }

        public async void SendNote(string email, Note note)
        {
            try
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = _fromAddress,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { email },
                    },
                    Message = new Message
                    {
                        Subject = new Content($"AwsAspNetCoreLabs. - {note.Title}"),
                        Body = new Body
                        {
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = note.Content
                            }
                        }
                    },
                    ConfigurationSetName = _settings.ConfigurationSet
                };

                var response = await _client.SendEmailAsync(sendRequest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't send an email");
                throw;
            }
        }
    }
}
