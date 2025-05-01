using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ApiControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IScreeningService _screeningService;
        private readonly ITheatreService _theatreService;
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public HomeController(
            IMovieService movieService,
            IScreeningService screeningService,
            ITheatreService theatreService,
            IBookingService bookingService,
            IUserService userService)
        {
            _movieService = movieService;
            _screeningService = screeningService;
            _theatreService = theatreService;
            _bookingService = bookingService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Root()
        {
            return Ok(new { message = "Theatre Management System API is running" });
        }

        [HttpGet("now-playing")]
        public async Task<IActionResult> GetNowPlayingMovies()
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

        [HttpGet("upcoming-movies")]
        public async Task<IActionResult> GetUpcomingMovies()
        {
            try
            {
                var upcomingMovies = await _movieService.GetUpcomingMoviesAsync();
                return Ok(upcomingMovies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("upcoming-screenings")]
        public async Task<IActionResult> GetUpcomingScreenings()
        {
            try
            {
                var screenings = await _screeningService.GetUpcomingScreeningsAsync(DateTime.Now);
                return Ok(screenings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                // Get counts for dashboard
                var movies = await _movieService.GetAllMoviesAsync();
                var theatres = await _theatreService.GetAllTheatresAsync();
                var bookings = await _bookingService.GetAllBookingsAsync();
                var users = await _userService.GetAllUsersAsync();

                // Get recent bookings (last 5)
                var recentBookings = bookings.OrderByDescending(b => b.BookingTime).Take(5);

                // Get popular movies (could be refined with booking statistics)
                var popularMovies = movies.Take(5);

                // Get booking statistics by status
                var bookingsByStatus = bookings.GroupBy(b => b.PaymentStatus)
                    .Select(g => new { Status = g.Key, Count = g.Count() });

                // Return dashboard data
                return Ok(new
                {
                    counts = new
                    {
                        totalMovies = movies.Count(),
                        totalTheatres = theatres.Count(),
                        totalBookings = bookings.Count(),
                        totalUsers = users.Count()
                    },
                    recentBookings,
                    popularMovies,
                    bookingsByStatus
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}