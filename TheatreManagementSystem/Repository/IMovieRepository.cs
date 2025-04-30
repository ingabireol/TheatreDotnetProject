using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(long id);
        Task<IEnumerable<Movie>> GetByTitleContainingAsync(string title);
        Task<IEnumerable<Movie>> GetByGenreAsync(Genre genre);
        Task<IEnumerable<Movie>> GetByReleaseDateAfterAsync(DateTime date);
        Task<IEnumerable<Movie>> GetMoviesByTheatreIdAsync(long theatreId);
        Task<IEnumerable<Movie>> GetCurrentlyPlayingMoviesAsync();
        Task<IEnumerable<Movie>> GetUpcomingMoviesAsync();
        Task<Movie> AddAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(long id);
    }
}