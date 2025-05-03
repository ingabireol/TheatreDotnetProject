using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ApiControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> GetAllBookings(
            [FromQuery] long? userId,
            [FromQuery] long? movieId,
            [FromQuery] long? theatreId,
            [FromQuery] string bookingNumber,
            [FromQuery] PaymentStatus? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                IEnumerable<BookingDTO> bookings;

                if (userId.HasValue)
                {
                    bookings = await _bookingService.GetBookingsByUserIdAsync(userId.Value);
                }
                else if (movieId.HasValue)
                {
                    bookings = await _bookingService.GetBookingsByMovieIdAsync(movieId.Value);
                }
                else if (theatreId.HasValue)
                {
                    bookings = await _bookingService.GetBookingsByTheatreIdAsync(theatreId.Value);
                }
                else if (!string.IsNullOrEmpty(bookingNumber))
                {
                    var booking = await _bookingService.GetBookingByNumberAsync(bookingNumber);
                    bookings = booking != null ? new List<BookingDTO> { booking } : new List<BookingDTO>();
                }
                else if (status.HasValue)
                {
                    bookings = await _bookingService.GetBookingsByStatusAsync(status.Value);
                }
                else if (fromDate.HasValue && toDate.HasValue)
                {
                    bookings = await _bookingService.GetBookingsByDateRangeAsync(fromDate.Value, toDate.Value);
                }
                else
                {
                    bookings = await _bookingService.GetAllBookingsAsync();
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(long id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found");
                }

                // Security check: ensure the user can only view their own bookings unless they're admin/manager
                if (!IsAdminOrManager() && booking.Username != User.Identity.Name)
                {
                    return Forbidden("You do not have permission to view this booking");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-user")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUser()
        {
            try
            {
                string username = User.Identity.Name;
                var bookings = await _bookingService.GetBookingsByUsernameAsync(username);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-number/{bookingNumber}")]
        [Authorize]
        public async Task<IActionResult> GetBookingByNumber(string bookingNumber)
        {
            try
            {
                var booking = await _bookingService.GetBookingByNumberAsync(bookingNumber);
                if (booking == null)
                {
                    return NotFound($"Booking with number {bookingNumber} not found");
                }

                // Security check: ensure the user can only view their own bookings unless they're admin/manager
                if (!IsAdminOrManager() && booking.Username != User.Identity.Name)
                {
                    return Forbidden("You do not have permission to view this booking");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking(
            [FromQuery] long screeningId,
            [FromBody] CreateBookingRequest request)
        {
            if (request.SelectedSeats == null || request.SelectedSeats.Count == 0)
            {
                return BadRequest("At least one seat must be selected");
            }

            try
            {
                string username = User.Identity.Name;
                var booking = await _bookingService.CreateBookingAsync(
                    screeningId,
                    username,
                    request.SelectedSeats,
                    request.PaymentMethod);

                return Created(nameof(GetBookingById), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(long id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return NotFound($"Booking with ID {id} not found");
                }

                // Security check: ensure the user can only cancel their own bookings unless they're admin/manager
                if (!IsAdminOrManager() && booking.Username != User.Identity.Name)
                {
                    return Forbidden("You do not have permission to cancel this booking");
                }

                await _bookingService.CancelBookingAsync(id);
                return Ok(new { message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/update-status")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> UpdateBookingStatus(long id, [FromQuery] PaymentStatus status)
        {
            try
            {
                await _bookingService.UpdateBookingStatusAsync(id, status);
                return Ok(new { message = "Booking status updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            try
            {
                await _bookingService.DeleteBookingAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("calculate-price")]
        [Authorize]
        public async Task<IActionResult> CalculatePrice(
            [FromQuery] long screeningId,
            [FromQuery] List<string> selectedSeats)
        {
            try
            {
                double totalPrice = await _bookingService.CalculateTotalPriceAsync(screeningId, selectedSeats);
                return Ok(new { totalPrice });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private bool IsAdminOrManager()
        {
            return User.IsInRole("ROLE_ADMIN") || User.IsInRole("ROLE_MANAGER");
        }
    }

    public class CreateBookingRequest
    {
        public List<string> SelectedSeats { get; set; }
        public string PaymentMethod { get; set; }
    }
}