using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/theatres")]
    [ApiController]
    public class TheatreController : ApiControllerBase
    {
        private readonly ITheatreService _theatreService;
        private readonly IScreeningService _screeningService;
        private readonly ISeatService _seatService;

        public TheatreController(
            ITheatreService theatreService,
            IScreeningService screeningService,
            ISeatService seatService)
        {
            _theatreService = theatreService;
            _screeningService = screeningService;
            _seatService = seatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTheatres([FromQuery] string name)
        {
            try
            {
                IEnumerable<TheatreDTO> theatres;

                if (!string.IsNullOrEmpty(name))
                {
                    theatres = await _theatreService.SearchTheatresByNameAsync(name);
                }
                else
                {
                    theatres = await _theatreService.GetAllTheatresAsync();
                }

                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTheatreById(long id)
        {
            try
            {
                var theatre = await _theatreService.GetTheatreByIdAsync(id);
                if (theatre == null)
                {
                    return NotFound($"Theatre with ID {id} not found");
                }

                return Ok(theatre);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-address")]
        public async Task<IActionResult> GetTheatresByAddress([FromQuery] string address)
        {
            try
            {
                var theatres = await _theatreService.SearchTheatresByAddressAsync(address);
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-movie/{movieId}")]
        public async Task<IActionResult> GetTheatresByMovie(long movieId)
        {
            try
            {
                var theatres = await _theatreService.GetTheatresByMovieAsync(movieId);
                return Ok(theatres);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> CreateTheatre([FromBody] TheatreDTO theatreDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTheatre = await _theatreService.CreateTheatreAsync(theatreDTO);
                return Created(nameof(GetTheatreById), new { id = createdTheatre.Id }, createdTheatre);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> UpdateTheatre(long id, [FromBody] TheatreDTO theatreDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTheatre = await _theatreService.UpdateTheatreAsync(id, theatreDTO);
                if (updatedTheatre == null)
                {
                    return NotFound($"Theatre with ID {id} not found");
                }

                return Ok(updatedTheatre);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteTheatre(long id)
        {
            try
            {
                await _theatreService.DeleteTheatreAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/screenings")]
        public async Task<IActionResult> GetTheatreScreenings(long id)
        {
            try
            {
                var screenings = await _screeningService.GetScreeningsByTheatreAsync(id);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetTheatreSeats(long id, [FromQuery] int screenNumber)
        {
            try
            {
                var seats = await _seatService.GetSeatsByTheatreAndScreenAsync(id, screenNumber);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/seat-map")]
        public async Task<IActionResult> GetTheatreSeatMap(long id, [FromQuery] int screenNumber)
        {
            try
            {
                var seatMap = await _seatService.GetSeatMapByTheatreAndScreenAsync(id, screenNumber);
                return Ok(seatMap);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/seats/initialize")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> InitializeSeats(
            long id,
            [FromQuery] int screenNumber,
            [FromQuery] int rows,
            [FromQuery] int seatsPerRow)
        {
            try
            {
                await _seatService.InitializeSeatsForTheatreAsync(id, screenNumber, rows, seatsPerRow);
                return Ok(new { message = $"Successfully initialized seats for screen {screenNumber}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}/seats")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteScreenSeats(long id, [FromQuery] int screenNumber)
        {
            try
            {
                var count = await _seatService.DeleteScreenSeatsAsync(id, screenNumber);
                return Ok(new { message = $"Successfully deleted {count} seats from screen {screenNumber}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}