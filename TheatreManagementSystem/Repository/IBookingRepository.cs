using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(long id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(long userId);
        Task<IEnumerable<Booking>> GetByScreeningIdAsync(long screeningId);
        Task<Booking> GetByBookingNumberAsync(string bookingNumber);
        Task<IEnumerable<Booking>> GetByUserIdAndBookingTimeAfterAsync(long userId, DateTime date);
        Task<IEnumerable<Booking>> GetByMovieIdAsync(long movieId);
        Task<IEnumerable<Booking>> GetByTheatreIdAsync(long theatreId);
        Task<IEnumerable<string>> GetBookedSeatsByScreeningIdAsync(long screeningId);
        Task<IEnumerable<Booking>> GetByPaymentStatusAsync(PaymentStatus status);
        Task<IEnumerable<Booking>> GetByBookingTimeBetweenAsync(DateTime fromDate, DateTime toDate);
        Task<Booking> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(long id);
    }
}
