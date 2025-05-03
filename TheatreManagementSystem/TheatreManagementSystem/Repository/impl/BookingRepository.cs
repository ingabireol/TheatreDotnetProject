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
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task DeleteAsync(long id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .ToListAsync();
        }

        public async Task<Booking> GetByBookingNumberAsync(string bookingNumber)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<IEnumerable<Booking>> GetByBookingTimeBetweenAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.BookingTime >= fromDate && b.BookingTime <= toDate)
                .ToListAsync();
        }

        public async Task<Booking> GetByIdAsync(long id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetByMovieIdAsync(long movieId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.Screening.MovieId == movieId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByPaymentStatusAsync(PaymentStatus status)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.PaymentStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByScreeningIdAsync(long screeningId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.ScreeningId == screeningId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByTheatreIdAsync(long theatreId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.Screening.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAndBookingTimeAfterAsync(long userId, DateTime date)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.UserId == userId && b.BookingTime >= date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(long userId)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Theatre)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetBookedSeatsByScreeningIdAsync(long screeningId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.ScreeningId == screeningId && b.PaymentStatus != PaymentStatus.CANCELLED)
                .ToListAsync();

            var bookedSeats = new HashSet<string>();
            foreach (var booking in bookings)
            {
                foreach (var seat in booking.BookedSeats)
                {
                    bookedSeats.Add(seat);
                }
            }

            return bookedSeats;
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}