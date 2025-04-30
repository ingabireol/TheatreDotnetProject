using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _screeningRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ITheatreRepository _theatreRepository;

        public ScreeningService(
            IScreeningRepository screeningRepository,
            IMovieRepository movieRepository,
            ITheatreRepository theatreRepository)
        {
            _screeningRepository = screeningRepository;
            _movieRepository = movieRepository;
            _theatreRepository = theatreRepository;
        }

        public async Task<ScreeningDTO> CreateScreeningAsync(ScreeningDTO screeningDTO)
        {
            var movie = await _movieRepository.GetByIdAsync(screeningDTO.MovieId)
                ?? throw new Exception($"Movie not found with id: {screeningDTO.MovieId}");

            var theatre = await _theatreRepository.GetByIdAsync(screeningDTO.TheatreId)
                ?? throw new Exception($"Theatre not found with id: {screeningDTO.TheatreId}");

            // Calculate end time based on movie duration
            var endTime = screeningDTO.StartTime.AddMinutes(movie.DurationMinutes);

            // Check for scheduling conflicts
            var theatreScreenings = await _screeningRepository.GetByTheatreIdAsync(theatre.Id);
            var hasConflict = theatreScreenings.Any(s =>
                s.ScreenNumber == screeningDTO.ScreenNumber &&
                s.StartTime < endTime &&
                s.EndTime > screeningDTO.StartTime);

            if (hasConflict)
            {
                throw new Exception("There is a scheduling conflict with another screening");
            }

            var screening = new Screening
            {
                MovieId = movie.Id,
                Movie = movie,
                TheatreId = theatre.Id,
                Theatre = theatre,
                StartTime = screeningDTO.StartTime,
                EndTime = endTime,
                ScreenNumber = screeningDTO.ScreenNumber,
                Format = screeningDTO.Format,
                BasePrice = screeningDTO.BasePrice
            };

            var savedScreening = await _screeningRepository.AddAsync(screening);
            return ConvertToDTO(savedScreening);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetAllScreeningsAsync()
        {
            var screenings = await _screeningRepository.GetAllAsync();
            return screenings.Select(ConvertToDTO);
        }

        public async Task<ScreeningDTO> GetScreeningByIdAsync(long id)
        {
            var screening = await _screeningRepository.GetByIdAsync(id);
            return screening != null ? ConvertToDTO(screening) : null;
        }

        public async Task<Screening> GetScreeningEntityByIdAsync(long id)
        {
            return await _screeningRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetScreeningsByMovieAsync(long movieId)
        {
            var screenings = await _screeningRepository.GetByMovieIdAsync(movieId);
            return screenings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetScreeningsByTheatreAsync(long theatreId)
        {
            var screenings = await _screeningRepository.GetByTheatreIdAsync(theatreId);
            return screenings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetScreeningsByMovieAndTheatreAsync(long movieId, long theatreId)
        {
            var screenings = await _screeningRepository.GetByMovieIdAndTheatreIdAsync(movieId, theatreId);
            return screenings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetScreeningsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var screenings = await _screeningRepository.GetByStartTimeRangeAsync(startDate, endDate);
            return screenings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<ScreeningDTO>> GetAvailableScreeningsAsync(long movieId, long theatreId, DateTime startDate)
        {
            var screenings = await _screeningRepository.GetAvailableScreeningsAsync(movieId, theatreId, startDate);
            return screenings.Select(ConvertToDTO);
        }

        public async Task<ScreeningDTO> UpdateScreeningAsync(long id, ScreeningDTO screeningDTO)
        {
            var screening = await _screeningRepository.GetByIdAsync(id);
            if (screening == null)
            {
                return null;
            }

            // Don't change movie and theatre for existing screenings
            screening.StartTime = screeningDTO.StartTime;

            // Recalculate end time based on the movie duration
            screening.EndTime = screeningDTO.StartTime.AddMinutes(screening.Movie.DurationMinutes);

            screening.ScreenNumber = screeningDTO.ScreenNumber;
            screening.Format = screeningDTO.Format;
            screening.BasePrice = screeningDTO.BasePrice;

            await _screeningRepository.UpdateAsync(screening);
            return ConvertToDTO(screening);
        }

        public async Task DeleteScreeningAsync(long id)
        {
            await _screeningRepository.DeleteAsync(id);
        }

        private ScreeningDTO ConvertToDTO(Screening screening)
        {
            return new ScreeningDTO
            {
                Id = screening.Id,
                MovieId = screening.Movie.Id,
                MovieTitle = screening.Movie.Title,
                TheatreId = screening.Theatre.Id,
                TheatreName = screening.Theatre.Name,
                StartTime = screening.StartTime,
                EndTime = screening.EndTime,
                ScreenNumber = screening.ScreenNumber,
                Format = screening.Format,
                BasePrice = screening.BasePrice
            };
        }

        public async Task<IEnumerable<ScreeningDTO>> GetUpcomingScreeningsAsync(DateTime fromDateTime)
        {
            var screenings = await _screeningRepository.GetUpcomingScreeningsAsync(fromDateTime);
            return screenings.Select(ConvertToDTO);
        }
    }

    public interface IScreeningService
    {
        Task<ScreeningDTO> CreateScreeningAsync(ScreeningDTO screeningDTO);
        Task<IEnumerable<ScreeningDTO>> GetAllScreeningsAsync();
        Task<ScreeningDTO> GetScreeningByIdAsync(long id);
        Task<Screening> GetScreeningEntityByIdAsync(long id);
        Task<IEnumerable<ScreeningDTO>> GetScreeningsByMovieAsync(long movieId);
        Task<IEnumerable<ScreeningDTO>> GetScreeningsByTheatreAsync(long theatreId);
        Task<IEnumerable<ScreeningDTO>> GetScreeningsByMovieAndTheatreAsync(long movieId, long theatreId);
        Task<IEnumerable<ScreeningDTO>> GetScreeningsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ScreeningDTO>> GetAvailableScreeningsAsync(long movieId, long theatreId, DateTime startDate);
        Task<ScreeningDTO> UpdateScreeningAsync(long id, ScreeningDTO screeningDTO);
        Task DeleteScreeningAsync(long id);
        Task<IEnumerable<ScreeningDTO>> GetUpcomingScreeningsAsync(DateTime fromDateTime);
    }
}
