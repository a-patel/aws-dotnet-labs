using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAspNetCoreLabs.Models
{
    public enum EventType
    {
        NoteCreated,
        NoteEdited,
        NoteViewed,
        NoteDeleted,
        NoteEmailed
    }
}
