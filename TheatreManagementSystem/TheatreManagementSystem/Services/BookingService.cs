using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IScreeningRepository _screeningRepository;
        private readonly ISeatRepository _seatRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IScreeningRepository screeningRepository,
            ISeatRepository seatRepository)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _screeningRepository = screeningRepository;
            _seatRepository = seatRepository;
        }

        public async Task<BookingDTO> CreateBookingAsync(long screeningId, string username, List<string> selectedSeats, string paymentMethod)
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found");

            var screening = await _screeningRepository.GetByIdAsync(screeningId)
                ?? throw new Exception("Screening not found");

            // Check if seats are available
            var bookedSeats = new HashSet<string>(await _bookingRepository.GetBookedSeatsByScreeningIdAsync(screeningId));
            foreach (var seat in selectedSeats)
            {
                if (bookedSeats.Contains(seat))
                {
                    throw new Exception($"Seat {seat} is already booked");
                }
            }

            // Calculate total price
            double totalPrice = await CalculateTotalPriceAsync(screeningId, selectedSeats);

            // Create booking
            var booking = new Booking
            {
                UserId = user.Id,
                User = user,
                ScreeningId = screening.Id,
                Screening = screening,
                BookingNumber = GenerateBookingNumber(),
                BookingTime = DateTime.Now,
                TotalAmount = totalPrice,
                PaymentStatus = PaymentStatus.COMPLETED, // Assuming payment is done immediately
                BookedSeats = new HashSet<string>(selectedSeats)
            };

            var savedBooking = await _bookingRepository.AddAsync(booking);
            return ConvertToDTO(savedBooking);
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(ConvertToDTO);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(long id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking != null ? ConvertToDTO(booking) : null;
        }

        public async Task<BookingDTO> GetBookingByNumberAsync(string bookingNumber)
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            return booking != null ? ConvertToDTO(booking) : null;
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(long userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username)
                ?? throw new Exception("User not found");

            return await GetBookingsByUserIdAsync(user.Id);
        }

        public async Task<HashSet<string>> GetBookedSeatsByScreeningIdAsync(long screeningId)
        {
            var bookedSeats = await _bookingRepository.GetBookedSeatsByScreeningIdAsync(screeningId);
            return new HashSet<string>(bookedSeats);
        }

        public async Task CancelBookingAsync(long id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id)
                ?? throw new Exception("Booking not found");

            // Check if the screening is in the future
            if (booking.Screening.StartTime < DateTime.Now)
            {
                throw new Exception("Cannot cancel past bookings");
            }

            booking.PaymentStatus = PaymentStatus.CANCELLED;
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task<double> CalculateTotalPriceAsync(long screeningId, List<string> selectedSeats)
        {
            var screening = await _screeningRepository.GetByIdAsync(screeningId)
                ?? throw new Exception("Screening not found");

            double basePrice = screening.BasePrice;
            double totalPrice = 0.0;

            // Get all seats from this theatre and screen
            var seats = await _seatRepository.GetByTheatreIdAndScreenNumberAsync(
                screening.Theatre.Id, screening.ScreenNumber);

            var seatMap = seats.ToDictionary(
                seat => seat.RowName + seat.SeatNumber,
                seat => seat
            );

            // Calculate price based on seat type
            foreach (var seatKey in selectedSeats)
            {
                if (seatMap.TryGetValue(seatKey, out var seat))
                {
                    totalPrice += basePrice * seat.PriceMultiplier;
                }
                else
                {
                    totalPrice += basePrice; // Default to base price if seat not found
                }
            }

            return totalPrice;
        }

        private string GenerateBookingNumber()
        {
            return "BK" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + new Random().Next(1000);
        }

        private BookingDTO ConvertToDTO(Booking booking)
        {
            return new BookingDTO
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                UserId = booking.User.Id,
                Username = booking.User.Username,
                UserEmail = booking.User.Email,
                ScreeningId = booking.Screening.Id,
                MovieTitle = booking.Screening.Movie.Title,
                MovieId = booking.Screening.Movie.Id,
                TheatreId = booking.Screening.Theatre.Id,
                MovieUrl = booking.Screening.Movie.TrailerUrl,
                TheatreName = booking.Screening.Theatre.Name,
                ScreeningTime = booking.Screening.StartTime,
                BookingTime = booking.BookingTime,
                TotalAmount = booking.TotalAmount,
                PaymentStatus = booking.PaymentStatus,
                BookedSeats = booking.BookedSeats,
                PaymentMethod = null // Payment method isn't stored in the entity
            };
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByScreeningIdAsync(long screeningId)
        {
            var bookings = await _bookingRepository.GetByScreeningIdAsync(screeningId);
            return bookings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByMovieIdAsync(long movieId)
        {
            var bookings = await _bookingRepository.GetByMovieIdAsync(movieId);
            return bookings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByTheatreIdAsync(long theatreId)
        {
            var bookings = await _bookingRepository.GetByTheatreIdAsync(theatreId);
            return bookings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByStatusAsync(PaymentStatus status)
        {
            var bookings = await _bookingRepository.GetByPaymentStatusAsync(status);
            return bookings.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var bookings = await _bookingRepository.GetByBookingTimeBetweenAsync(fromDate, toDate);
            return bookings.Select(ConvertToDTO);
        }

        public async Task UpdateBookingStatusAsync(long id, PaymentStatus status)
        {
            var booking = await _bookingRepository.GetByIdAsync(id)
                ?? throw new Exception("Booking not found with id: " + id);

            booking.PaymentStatus = status;
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task DeleteBookingAsync(long id)
        {
            await _bookingRepository.DeleteAsync(id);
        }
    }

    public interface IBookingService
    {
        Task<BookingDTO> CreateBookingAsync(long screeningId, string username, List<string> selectedSeats, string paymentMethod);
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO> GetBookingByIdAsync(long id);
        Task<BookingDTO> GetBookingByNumberAsync(string bookingNumber);
        Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(long userId);
        Task<IEnumerable<BookingDTO>> GetBookingsByUsernameAsync(string username);
        Task<HashSet<string>> GetBookedSeatsByScreeningIdAsync(long screeningId);
        Task CancelBookingAsync(long id);
        Task<double> CalculateTotalPriceAsync(long screeningId, List<string> selectedSeats);
        Task<IEnumerable<BookingDTO>> GetBookingsByScreeningIdAsync(long screeningId);
        Task<IEnumerable<BookingDTO>> GetBookingsByMovieIdAsync(long movieId);
        Task<IEnumerable<BookingDTO>> GetBookingsByTheatreIdAsync(long theatreId);
        Task<IEnumerable<BookingDTO>> GetBookingsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<BookingDTO>> GetBookingsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task UpdateBookingStatusAsync(long id, PaymentStatus status);
        Task DeleteBookingAsync(long id);
    }
}