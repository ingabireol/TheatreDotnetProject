using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/screenings")]
    [ApiController]
    public class ScreeningController : ApiControllerBase
    {
        private readonly IScreeningService _screeningService;
        private readonly IBookingService _bookingService;

        public ScreeningController(IScreeningService screeningService, IBookingService bookingService)
        {
            _screeningService = screeningService;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScreenings()
        {
            try
            {
                var screenings = await _screeningService.GetAllScreeningsAsync();
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScreeningById(long id)
        {
            try
            {
                var screening = await _screeningService.GetScreeningByIdAsync(id);
                if (screening == null)
                {
                    return NotFound($"Screening with ID {id} not found");
                }

                return Ok(screening);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-movie/{movieId}")]
        public async Task<IActionResult> GetScreeningsByMovie(long movieId)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByMovieAsync(movieId);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-theatre/{theatreId}")]
        public async Task<IActionResult> GetScreeningsByTheatre(long theatreId)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByTheatreAsync(theatreId);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-movie-theatre")]
        public async Task<IActionResult> GetScreeningsByMovieAndTheatre(
            [FromQuery] long movieId,
            [FromQuery] long theatreId)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByMovieAndTheatreAsync(movieId, theatreId);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetScreeningsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByDateRangeAsync(startDate, endDate);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableScreenings(
            [FromQuery] long movieId,
            [FromQuery] long theatreId,
            [FromQuery] DateTime startDate)
        {
            try
            {
                var screenings = await _screeningService.GetAvailableScreeningsAsync(movieId, theatreId, startDate);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingScreenings([FromQuery] DateTime? fromDate)
        {
            try
            {
                var date = fromDate ?? DateTime.Now;
                var screenings = await _screeningService.GetUpcomingScreeningsAsync(date);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> CreateScreening([FromBody] ScreeningDTO screeningDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdScreening = await _screeningService.CreateScreeningAsync(screeningDTO);
                return Created(nameof(GetScreeningById), new { id = createdScreening.Id }, createdScreening);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> UpdateScreening(long id, [FromBody] ScreeningDTO screeningDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedScreening = await _screeningService.UpdateScreeningAsync(id, screeningDTO);
                if (updatedScreening == null)
                {
                    return NotFound($"Screening with ID {id} not found");
                }

                return Ok(updatedScreening);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> DeleteScreening(long id)
        {
            try
            {
                await _screeningService.DeleteScreeningAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/booked-seats")]
        public async Task<IActionResult> GetBookedSeatsByScreening(long id)
        {
            try
            {
                var bookedSeats = await _bookingService.GetBookedSeatsByScreeningIdAsync(id);
                return Ok(bookedSeats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/bookings")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> GetBookingsByScreening(long id)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByScreeningIdAsync(id);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}