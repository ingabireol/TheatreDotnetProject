using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.Data;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Repositories.Impl
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreeningRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Screening> AddAsync(Screening screening)
        {
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();
            return screening;
        }

        public async Task<long> CountBookingsByScreeningIdAsync(long screeningId)
        {
            return await _context.Bookings
                .Where(b => b.ScreeningId == screeningId && b.PaymentStatus != PaymentStatus.CANCELLED)
                .CountAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                _context.Screenings.Remove(screening);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Screening>> GetAllAsync()
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screening>> GetAvailableScreeningsAsync(long movieId, long theatreId, DateTime startDate)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.MovieId == movieId &&
                       s.TheatreId == theatreId &&
                       s.StartTime >= startDate)
                .ToListAsync();
        }

        public async Task<Screening> GetByIdAsync(long id)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Screening>> GetByMovieIdAndTheatreIdAsync(long movieId, long theatreId)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.MovieId == movieId && s.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screening>> GetByMovieIdAsync(long movieId)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screening>> GetByStartTimeRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screening>> GetByTheatreIdAsync(long theatreId)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screening>> GetUpcomingScreeningsAsync(DateTime startTime)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.StartTime >= startTime)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(Screening screening)
        {
            _context.Screenings.Update(screening);
            await _context.SaveChangesAsync();
        }
    }
}