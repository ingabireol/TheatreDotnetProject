using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Repositories
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<Seat> GetByIdAsync(long id);
        Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAsync(long theatreId, int screenNumber);
        Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAndSeatTypeAsync(long theatreId, int screenNumber, SeatType seatType);
        Task<IEnumerable<Seat>> GetByTheatreIdAndScreenNumberAndRowNameAsync(long theatreId, int screenNumber, string rowName);
        Task<Seat> AddAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task DeleteAsync(long id);
    }
}
