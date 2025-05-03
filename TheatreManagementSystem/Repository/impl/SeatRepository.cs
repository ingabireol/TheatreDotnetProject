using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.Data;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Repositories.Impl
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Seat> AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task DeleteAsync(long id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null)
            {
                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Seat>> GetAllAsync()
        {
            return await _context.Seats
                .Include(s => s.Theatre)
                .ToListAsync();
        }

        public async Task<Seat> GetByIdAsync(long id)
        {
            return await _context.Seats
                .Include(s => s.Theatre)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAndRowNameAsync(long theatreId, int screenNumber, string rowName)
        {
            return await _context.Seats
                .Include(s => s.Theatre)
                .Where(s => s.TheatreId == theatreId &&
                       s.ScreenNumber == screenNumber &&
                       s.RowName == rowName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAndSeatTypeAsync(long theatreId, int screenNumber, SeatType seatType)
        {
            return await _context.Seats
                .Include(s => s.Theatre)
                .Where(s => s.TheatreId == theatreId &&
                       s.ScreenNumber == screenNumber &&
                       s.SeatType == seatType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAsync(long theatreId, int screenNumber)
        {
            return await _context.Seats
                .Include(s => s.Theatre)
                .Where(s => s.TheatreId == theatreId &&
                       s.ScreenNumber == screenNumber)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }
    }
}