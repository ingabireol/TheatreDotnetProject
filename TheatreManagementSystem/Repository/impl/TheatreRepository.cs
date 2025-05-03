using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.Data;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Repositories.Impl
{
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Theatre> AddAsync(Theatre theatre)
        {
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();
            return theatre;
        }

        public async Task DeleteAsync(long id)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre != null)
            {
                _context.Theatres.Remove(theatre);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Theatre>> GetAllAsync()
        {
            return await _context.Theatres
                .Include(t => t.Screenings)
                .Include(t => t.Seats)
                .ToListAsync();
        }

        public async Task<IEnumerable<Theatre>> GetByAddressContainingAsync(string address)
        {
            return await _context.Theatres
                .Include(t => t.Screenings)
                .Include(t => t.Seats)
                .Where(t => t.Address.Contains(address))
                .ToListAsync();
        }

        public async Task<Theatre> GetByIdAsync(long id)
        {
            return await _context.Theatres
                .Include(t => t.Screenings)
                .Include(t => t.Seats)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Theatre>> GetByNameContainingAsync(string name)
        {
            return await _context.Theatres
                .Include(t => t.Screenings)
                .Include(t => t.Seats)
                .Where(t => t.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Theatre>> GetTheatresByMovieIdAsync(long movieId)
        {
            return await _context.Theatres
                .Include(t => t.Screenings)
                .Include(t => t.Seats)
                .Where(t => t.Screenings.Any(s => s.MovieId == movieId))
                .ToListAsync();
        }

        public async Task UpdateAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);
            await _context.SaveChangesAsync();
        }
    }
}