using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/seats")]
    [ApiController]
    public class SeatController : ApiControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeatById(long id)
        {
            try
            {
                var seat = await _seatService.GetSeatByIdAsync(id);
                if (seat == null)
                {
                    return NotFound($"Seat with ID {id} not found");
                }

                return Ok(seat);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-theatre-screen")]
        public async Task<IActionResult> GetSeatsByTheatreAndScreen(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber)
        {
            try
            {
                var seats = await _seatService.GetSeatsByTheatreAndScreenAsync(theatreId, screenNumber);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("map")]
        public async Task<IActionResult> GetSeatMap(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber)
        {
            try
            {
                var seatMap = await _seatService.GetSeatMapByTheatreAndScreenAsync(theatreId, screenNumber);
                return Ok(seatMap);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-type")]
        public async Task<IActionResult> GetSeatsByType(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber,
            [FromQuery] SeatType seatType)
        {
            try
            {
                var seats = await _seatService.GetSeatsByTypeAsync(theatreId, screenNumber, seatType);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateSeat(
            long id,
            [FromQuery] SeatType seatType,
            [FromQuery] double priceMultiplier)
        {
            try
            {
                await _seatService.UpdateSeatTypeAsync(id, seatType, priceMultiplier);
                return Ok(new { message = "Seat updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("update-row")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateSeatRow(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber,
            [FromQuery] string rowName,
            [FromQuery] SeatType seatType,
            [FromQuery] double priceMultiplier)
        {
            try
            {
                await _seatService.UpdateSeatRowTypeAsync(theatreId, screenNumber, rowName, seatType, priceMultiplier);
                return Ok(new { message = $"Row {rowName} updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("bulk-update")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> BulkUpdateSeats(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber,
            [FromBody] BulkUpdateSeatsRequest request)
        {
            try
            {
                int count = await _seatService.BulkUpdateSeatsAsync(
                    request.SeatIds,
                    theatreId,
                    screenNumber,
                    request.SeatType,
                    request.PriceMultiplier);

                return Ok(new { message = $"{count} seats updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("initialize")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> InitializeSeats(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber,
            [FromQuery] int rows,
            [FromQuery] int seatsPerRow)
        {
            try
            {
                await _seatService.InitializeSeatsForTheatreAsync(theatreId, screenNumber, rows, seatsPerRow);
                return Ok(new { message = $"Successfully initialized seats for screen {screenNumber}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("screen")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteScreenSeats(
            [FromQuery] long theatreId,
            [FromQuery] int screenNumber)
        {
            try
            {
                int count = await _seatService.DeleteScreenSeatsAsync(theatreId, screenNumber);
                return Ok(new { message = $"Successfully deleted {count} seats from screen {screenNumber}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }

    public class BulkUpdateSeatsRequest
    {
        public List<string> SeatIds { get; set; }
        public SeatType SeatType { get; set; }
        public double PriceMultiplier { get; set; }
    }
}