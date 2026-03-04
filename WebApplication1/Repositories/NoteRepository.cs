using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationContext _context;

        public NoteRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
        }

        public async Task<Note> GetNoteByIdAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddNoteAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNoteAsync(Note note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }
    }
}
