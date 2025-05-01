using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<MovieDTO> CreateMovieAsync(MovieDTO movieDTO)
        {
            var movie = ConvertToEntity(movieDTO);
            var savedMovie = await _movieRepository.AddAsync(movie);
            return ConvertToDTO(savedMovie);
        }

        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllAsync();
            return movies.Select(ConvertToDTO);
        }

        public async Task<MovieDTO> GetMovieByIdAsync(long id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            return movie != null ? ConvertToDTO(movie) : null;
        }

        public async Task<IEnumerable<MovieDTO>> SearchMoviesByTitleAsync(string title)
        {
            var movies = await _movieRepository.GetByTitleContainingAsync(title);
            return movies.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<MovieDTO>> GetMoviesByGenreAsync(Genre genre)
        {
            var movies = await _movieRepository.GetByGenreAsync(genre);
            return movies.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<MovieDTO>> GetUpcomingMoviesAsync()
        {
            var movies = await _movieRepository.GetUpcomingMoviesAsync();
            return movies.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<MovieDTO>> GetCurrentlyPlayingMoviesAsync()
        {
            var movies = await _movieRepository.GetCurrentlyPlayingMoviesAsync();
            return movies.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<MovieDTO>> GetMoviesByTheatreAsync(long theatreId)
        {
            var movies = await _movieRepository.GetMoviesByTheatreIdAsync(theatreId);
            return movies.Select(ConvertToDTO);
        }

        public async Task<MovieDTO> UpdateMovieAsync(long id, MovieDTO movieDTO)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return null;
            }

            movie.Title = movieDTO.Title;
            movie.Description = movieDTO.Description;
            movie.DurationMinutes = movieDTO.DurationMinutes;
            movie.Genre = movieDTO.Genre;
            movie.Director = movieDTO.Director;
            movie.Cast = movieDTO.Cast;
            movie.ReleaseDate = movieDTO.ReleaseDate;
            movie.PosterImageUrl = movieDTO.PosterImageUrl;
            movie.TrailerUrl = movieDTO.TrailerUrl;
            movie.Rating = movieDTO.Rating;

            await _movieRepository.UpdateAsync(movie);
            return ConvertToDTO(movie);
        }

        public async Task DeleteMovieAsync(long id)
        {
            await _movieRepository.DeleteAsync(id);
        }

        private MovieDTO ConvertToDTO(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                Genre = movie.Genre,
                Director = movie.Director,
                Cast = movie.Cast,
                ReleaseDate = movie.ReleaseDate,
                PosterImageUrl = movie.PosterImageUrl,
                TrailerUrl = movie.TrailerUrl,
                Rating = movie.Rating
            };
        }

        private Movie ConvertToEntity(MovieDTO dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                Genre = dto.Genre,
                Director = dto.Director,
                Cast = dto.Cast,
                ReleaseDate = dto.ReleaseDate,
                PosterImageUrl = dto.PosterImageUrl,
                TrailerUrl = dto.TrailerUrl,
                Rating = dto.Rating
            };

            // Don't set ID for new entities
            if (dto.Id != 0)
            {
                movie.Id = dto.Id;
            }

            return movie;
        }

        public async Task<IEnumerable<MovieDTO>> GetMoviesByIdsAsync(IEnumerable<long> movieIds)
        {
            if (movieIds == null || !movieIds.Any())
            {
                return new List<MovieDTO>();
            }

            var movies = new List<Movie>();
            foreach (var id in movieIds)
            {
                var movie = await _movieRepository.GetByIdAsync(id);
                if (movie != null)
                {
                    movies.Add(movie);
                }
            }

            return movies.Select(ConvertToDTO);
        }
    }

    public interface IMovieService
    {
        Task<MovieDTO> CreateMovieAsync(MovieDTO movieDTO);
        Task<IEnumerable<MovieDTO>> GetAllMoviesAsync();
        Task<MovieDTO> GetMovieByIdAsync(long id);
        Task<IEnumerable<MovieDTO>> SearchMoviesByTitleAsync(string title);
        Task<IEnumerable<MovieDTO>> GetMoviesByGenreAsync(Genre genre);
        Task<IEnumerable<MovieDTO>> GetUpcomingMoviesAsync();
        Task<IEnumerable<MovieDTO>> GetCurrentlyPlayingMoviesAsync();
        Task<IEnumerable<MovieDTO>> GetMoviesByTheatreAsync(long theatreId);
        Task<MovieDTO> UpdateMovieAsync(long id, MovieDTO movieDTO);
        Task DeleteMovieAsync(long id);
        Task<IEnumerable<MovieDTO>> GetMoviesByIdsAsync(IEnumerable<long> movieIds);
    }
}
