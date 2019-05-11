using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAspNetCoreLabs.Models
{
    public class NoteSummary
    {
        public string UserId { get; set; }
        public Guid? NoteId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
