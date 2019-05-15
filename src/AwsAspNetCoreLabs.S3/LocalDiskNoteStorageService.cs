using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsAspNetCoreLabs.Models;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using AwsAspNetCoreLabs.Models.Settings;

namespace AwsAspNetCoreLabs.S3
{
    public class LocalDiskNoteStorageService : INoteStorageService
    {
        private string _storagePath;
        private IDistributedCache _cache;
        private DistributedCacheEntryOptions _cacheEntryOptions;

        public LocalDiskNoteStorageService(IOptions<LocalDiskStorageSettings> storageSettings, IDistributedCache cache, DistributedCacheEntryOptions cacheEntryOptions)
        {
            _storagePath = storageSettings.Value.LocalStoragePath;
            _cache = cache;
            _cacheEntryOptions = cacheEntryOptions;
        }

        public async Task SaveNote(Note note)
        {
            if (note == null) throw new ArgumentNullException("note");

            if (string.IsNullOrWhiteSpace(note.UserId)) throw new ArgumentException("The note provided didn't have a user ID assigned");

            Guid noteId = note.NoteId ?? Guid.NewGuid();
            string notePath = GetNotePath(note.UserId, noteId);

            // check that the directory exists
            if (!Directory.Exists(Path.GetDirectoryName(notePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(notePath));
            }

            using (StreamWriter file = File.CreateText(notePath))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(note));

                // Save data in cache.
                _cache.SetString(notePath, JsonConvert.SerializeObject(note), _cacheEntryOptions);
            }

            // Update summary
            List<NoteSummary> notes = await GetNoteList(note.UserId);
            NoteSummary oldNote = notes.Where(n => n.NoteId == noteId).SingleOrDefault();

            if (notes != null)
            {
                notes.Remove(oldNote);
            }

            notes.Add(new NoteSummary { NoteId = note.NoteId, UserId = note.UserId, Title = note.Title, CreatedAt = note.CreatedAt });

            await SaveNoteList(note.UserId, notes);
        }

        public async Task DeleteNote(string username, Guid noteId)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException("username");
            if (noteId == null) throw new ArgumentNullException("noteId");

            string notePath = GetNotePath(username, noteId);

            if (File.Exists(notePath))
            {
                File.Delete(notePath);

                // Remove data from cache.
                _cache.Remove(notePath);
            }

            // Update summary
            List<NoteSummary> notes = await GetNoteList(username);
            NoteSummary oldNote = notes.Where(n => n.NoteId == noteId).SingleOrDefault();

            if (notes != null)
            {
                notes.Remove(oldNote);

                // Remove data from cache.
                _cache.Remove(notePath);
            }

            await SaveNoteList(username, notes);
        }

        public async Task<Note> GetNote(string username, Guid noteId)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException("username");
            if (noteId == null) throw new ArgumentNullException("noteId");

            string notePath = GetNotePath(username, noteId);

            // Look for cache key.
            string cacheEntry = _cache.GetString(notePath);

            if (string.IsNullOrEmpty(cacheEntry))
            {
                // Key not in cache, so get data.
                if (File.Exists(notePath))
                {
                    using (StreamReader file = File.OpenText(notePath))
                    {
                        Note note = JsonConvert.DeserializeObject<Note>(await file.ReadToEndAsync());

                        // Save data in cache.
                        _cache.SetString(notePath, JsonConvert.SerializeObject(note), _cacheEntryOptions);

                        return note;
                    }
                }

                // The note was not found
                return null;
            }

            // the note was found in cache
            return JsonConvert.DeserializeObject<Note>(cacheEntry);
        }

        public async Task<List<NoteSummary>> GetNoteList(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException("username");

            string summaryPath = GetSummaryPath(username);

            // Look for cache key.
            string cacheEntry = _cache.GetString(summaryPath);

            if (string.IsNullOrEmpty(cacheEntry))
            {
                // Key not in cache, so get data.
                if (File.Exists(summaryPath))
                {
                    using (StreamReader file = File.OpenText(summaryPath))
                    {
                        List<NoteSummary> notes = JsonConvert.DeserializeObject<List<NoteSummary>>(await file.ReadToEndAsync());

                        // Save data in cache.
                        _cache.SetString(summaryPath, JsonConvert.SerializeObject(notes), _cacheEntryOptions);

                        return notes;
                    }
                }

                return new List<NoteSummary>();
            }

            return JsonConvert.DeserializeObject<List<NoteSummary>>(cacheEntry);
        }

        private async Task SaveNoteList(string username, List<NoteSummary> notes)
        {
            if (notes == null) throw new ArgumentNullException("notes");

            if (notes.Any())
            {
                // check that notes are for a single user
                var groups = notes.GroupBy(n => n.UserId);

                if (groups.Count() > 1) throw new ArgumentException("The list of note refers to more than one user.");
            }

            string summaryPath = GetSummaryPath(username);

            // check that the directory exists
            if (!Directory.Exists(Path.GetDirectoryName(summaryPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(summaryPath));
            }

            using (StreamWriter file = File.CreateText(summaryPath))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(notes.OrderByDescending(n => n.CreatedAt)));

                // Save data in cache.
                _cache.SetString(summaryPath, JsonConvert.SerializeObject(notes), _cacheEntryOptions);
            }
        }

        private string GetNotePath(string username, Guid noteId)
        {
            return Path.Combine(_storagePath, username, $"{noteId.ToString()}.json");
        }

        private string GetSummaryPath(string username)
        {
            return Path.Combine(_storagePath, username, $"summary.json");
        }
    }
}
