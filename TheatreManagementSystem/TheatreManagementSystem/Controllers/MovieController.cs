using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ApiControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IScreeningService _screeningService;
        private readonly ITheatreService _theatreService;

        public MovieController(
            IMovieService movieService,
            IScreeningService screeningService,
            ITheatreService theatreService)
        {
            _movieService = movieService;
            _screeningService = screeningService;
            _theatreService = theatreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies([FromQuery] string title, [FromQuery] Genre? genre)
        {
            try
            {
                IEnumerable<MovieDTO> movies;

                if (!string.IsNullOrEmpty(title))
                {
                    movies = await _movieService.SearchMoviesByTitleAsync(title);
                }
                else if (genre.HasValue)
                {
                    movies = await _movieService.GetMoviesByGenreAsync(genre.Value);
                }
                else
                {
                    movies = await _movieService.GetAllMoviesAsync();
                }

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(long id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found");
                }

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("current-playing")]
        public async Task<IActionResult> GetCurrentlyPlayingMovies()
        {
            try
            {
                var movies = await _movieService.GetCurrentlyPlayingMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMovies()
        {
            try
            {
                var movies = await _movieService.GetUpcomingMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-theatre/{theatreId}")]
        public async Task<IActionResult> GetMoviesByTheatre(long theatreId)
        {
            try
            {
                var movies = await _movieService.GetMoviesByTheatreAsync(theatreId);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieDTO movieDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdMovie = await _movieService.CreateMovieAsync(movieDTO);
                return Created(nameof(GetMovieById), new { id = createdMovie.Id }, createdMovie);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> UpdateMovie(long id, [FromBody] MovieDTO movieDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedMovie = await _movieService.UpdateMovieAsync(id, movieDTO);
                if (updatedMovie == null)
                {
                    return NotFound($"Movie with ID {id} not found");
                }

                return Ok(updatedMovie);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            try
            {
                await _movieService.DeleteMovieAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/screenings")]
        public async Task<IActionResult> GetMovieScreenings(long id)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByMovieAsync(id);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/theatres")]
        public async Task<IActionResult> GetMovieTheatres(long id)
        {
            try
            {
                var theatres = await _theatreService.GetTheatresByMovieAsync(id);
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}