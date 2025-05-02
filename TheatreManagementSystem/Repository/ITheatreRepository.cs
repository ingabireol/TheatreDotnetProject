using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Repositories
{
    public interface ITheatreRepository
    {
        Task<IEnumerable<Theatre>> GetAllAsync();
        Task<Theatre> GetByIdAsync(long id);
        Task<IEnumerable<Theatre>> GetByNameContainingAsync(string name);
        Task<IEnumerable<Theatre>> GetByAddressContainingAsync(string address);
        Task<IEnumerable<Theatre>> GetTheatresByMovieIdAsync(long movieId);
        Task<Theatre> AddAsync(Theatre theatre);
        Task UpdateAsync(Theatre theatre);
        Task DeleteAsync(long id);
    }
}
