using AwsAspNetCoreLabs.Models;

namespace AwsAspNetCoreLabs.SES
{
    public interface INoteEmailService
    {
        void SendNote(string email, Note note);
    }
}