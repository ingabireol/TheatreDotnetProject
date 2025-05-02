using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ROLE_ADMIN")]
    public class AdminController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMovieService _movieService;
        private readonly ITheatreService _theatreService;
        private readonly IBookingService _bookingService;
        private readonly IScreeningService _screeningService;

        public AdminController(
            IUserService userService,
            IMovieService movieService,
            ITheatreService theatreService,
            IBookingService bookingService,
            IScreeningService screeningService)
        {
            _userService = userService;
            _movieService = movieService;
            _theatreService = theatreService;
            _bookingService = bookingService;
            _screeningService = screeningService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                // Get counts for dashboard
                var users = await _userService.GetAllUsersAsync();
                var movies = await _movieService.GetAllMoviesAsync();
                var theatres = await _theatreService.GetAllTheatresAsync();
                var bookings = await _bookingService.GetAllBookingsAsync();
                var screenings = await _screeningService.GetAllScreeningsAsync();

                // Get recent items (for dashboard widgets)
                var recentBookings = bookings.OrderByDescending(b => b.BookingTime).Take(5);
                var newUsers = users.OrderByDescending(u => u.Id).Take(5);
                var upcomingScreenings = screenings
                    .Where(s => s.StartTime > DateTime.Now)
                    .OrderBy(s => s.StartTime)
                    .Take(5);

                // Get stats for charts
                var bookingsByStatus = bookings
                    .GroupBy(b => b.PaymentStatus)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() });

                // Return dashboard data
                return Ok(new
                {
                    counts = new
                    {
                        totalUsers = users.Count(),
                        totalMovies = movies.Count(),
                        totalTheatres = theatres.Count(),
                        totalBookings = bookings.Count(),
                        totalScreenings = screenings.Count()
                    },
                    recentData = new
                    {
                        recentBookings,
                        newUsers,
                        upcomingScreenings
                    },
                    statistics = new
                    {
                        bookingsByStatus
                    }
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("statistics/bookings")]
        public async Task<IActionResult> GetBookingStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                var bookings = await _bookingService.GetBookingsByDateRangeAsync(start, end);

                // Group bookings by date
                var bookingsByDate = bookings
                    .GroupBy(b => b.BookingTime.Date)
                    .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
                    .OrderBy(x => x.Date);

                // Group bookings by status
                var bookingsByStatus = bookings
                    .GroupBy(b => b.PaymentStatus)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() });

                // Calculate revenue
                var totalRevenue = bookings
                    .Where(b => b.PaymentStatus == Models.PaymentStatus.COMPLETED)
                    .Sum(b => b.TotalAmount);

                return Ok(new
                {
                    totalBookings = bookings.Count(),
                    totalRevenue,
                    bookingsByDate,
                    bookingsByStatus
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("statistics/users")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                // Group users by role
                var usersByRole = users
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key.ToString(), Count = g.Count() });

                return Ok(new
                {
                    totalUsers = users.Count(),
                    usersByRole
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("statistics/movies")]
        public async Task<IActionResult> GetMovieStatistics()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                var bookings = await _bookingService.GetAllBookingsAsync();

                // Group movies by genre
                var moviesByGenre = movies
                    .GroupBy(m => m.Genre)
                    .Select(g => new { Genre = g.Key.ToString(), Count = g.Count() });

                // Get current and upcoming counts
                var currentlyPlaying = await _movieService.GetCurrentlyPlayingMoviesAsync();
                var upcoming = await _movieService.GetUpcomingMoviesAsync();

                return Ok(new
                {
                    totalMovies = movies.Count(),
                    currentlyPlayingCount = currentlyPlaying.Count(),
                    upcomingCount = upcoming.Count(),
                    moviesByGenre
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}