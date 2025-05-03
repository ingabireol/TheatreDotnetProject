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
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task DeleteAsync(long id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetByGenreAsync(Genre genre)
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.Genre == genre)
                .ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(long id)
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Movie>> GetByReleaseDateAfterAsync(DateTime date)
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.ReleaseDate >= date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetByTitleContainingAsync(string title)
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.Title.Contains(title))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetCurrentlyPlayingMoviesAsync()
        {
            var today = DateTime.Today;

            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.Screenings.Any(s => s.StartTime.Date >= today))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByTheatreIdAsync(long theatreId)
        {
            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.Screenings.Any(s => s.TheatreId == theatreId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetUpcomingMoviesAsync()
        {
            var today = DateTime.Today;
            var oneWeekFromNow = today.AddDays(7);

            return await _context.Movies
                .Include(m => m.Screenings)
                .Where(m => m.ReleaseDate >= today && m.ReleaseDate <= oneWeekFromNow)
                .ToListAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}