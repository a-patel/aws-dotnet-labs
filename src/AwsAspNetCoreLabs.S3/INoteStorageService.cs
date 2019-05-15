using AwsAspNetCoreLabs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwsAspNetCoreLabs.S3
{
    public interface INoteStorageService
    {
        Task<Note> GetNote(string username, Guid noteId);
        Task SaveNote(Note note);
        Task DeleteNote(string username, Guid noteId);
        Task<List<NoteSummary>> GetNoteList(string username);
    }
}
