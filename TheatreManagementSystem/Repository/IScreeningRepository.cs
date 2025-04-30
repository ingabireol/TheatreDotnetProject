using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Repositories
{
    public interface IScreeningRepository
    {
        Task<IEnumerable<Screening>> GetAllAsync();
        Task<Screening> GetByIdAsync(long id);
        Task<IEnumerable<Screening>> GetByMovieIdAsync(long movieId);
        Task<IEnumerable<Screening>> GetByTheatreIdAsync(long theatreId);
        Task<IEnumerable<Screening>> GetByMovieIdAndTheatreIdAsync(long movieId, long theatreId);
        Task<IEnumerable<Screening>> GetByStartTimeRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Screening>> GetAvailableScreeningsAsync(long movieId, long theatreId, DateTime startDate);
        Task<long> CountBookingsByScreeningIdAsync(long screeningId);
        Task<IEnumerable<Screening>> GetUpcomingScreeningsAsync(DateTime startTime);
        Task<Screening> AddAsync(Screening screening);
        Task UpdateAsync(Screening screening);
        Task DeleteAsync(long id);
    }
}