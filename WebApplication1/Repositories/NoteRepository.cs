using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(string userId);
        Task<Note> GetNoteByIdAsync(int id);
        Task AddNoteAsync(Note note);
        Task UpdateNoteAsync(Note note);
        Task DeleteNoteAsync(int id);
    }

    public class NoteRepository: INoteRepository
    {
        private ApplicationContext _context;

        public Task AddNoteAsync(Note note)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNoteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetNoteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Note>> GetNotesByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateNoteAsync(Note note)
        {
            throw new NotImplementedException();
        }
    }
}
