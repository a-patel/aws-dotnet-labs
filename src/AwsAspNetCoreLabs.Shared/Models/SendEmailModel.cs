using System;
using System.ComponentModel;

namespace AwsAspNetCoreLabs.Models
{
    public class SendEmailModel
    {
        public Guid NoteId { get; set; }

        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
    }
}
