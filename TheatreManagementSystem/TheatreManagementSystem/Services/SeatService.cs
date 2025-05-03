using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Services
{
    public interface ISeatService
    {
        Task<Seat> GetSeatByIdAsync(long id);
        Task InitializeSeatsForTheatreAsync(long theatreId, int screenNumber, int rows, int seatsPerRow);
        Task<IEnumerable<Seat>> GetSeatsByTheatreAndScreenAsync(long theatreId, int screenNumber);
        Task<Dictionary<string, List<Seat>>> GetSeatMapByTheatreAndScreenAsync(long theatreId, int screenNumber);
        Task<IEnumerable<Seat>> GetSeatsByTypeAsync(long theatreId, int screenNumber, SeatType seatType);
        Task UpdateSeatTypeAsync(long seatId, SeatType seatType, double priceMultiplier);
        Task UpdateSeatRowTypeAsync(long theatreId, int screenNumber, string rowName, SeatType seatType, double priceMultiplier);
        Task<int> BulkUpdateSeatsAsync(List<string> seatIds, long theatreId, int screenNumber, SeatType seatType, double priceMultiplier);
        Task<int> DeleteScreenSeatsAsync(long theatreId, int screenNumber);
    }

    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly ITheatreRepository _theatreRepository;

        public SeatService(ISeatRepository seatRepository, ITheatreRepository theatreRepository)
        {
            _seatRepository = seatRepository;
            _theatreRepository = theatreRepository;
        }

        public async Task<Seat> GetSeatByIdAsync(long id)
        {
            return await _seatRepository.GetByIdAsync(id);
        }

        public async Task InitializeSeatsForTheatreAsync(long theatreId, int screenNumber, int rows, int seatsPerRow)
        {
            var theatre = await _theatreRepository.GetByIdAsync(theatreId);
            if (theatre == null)
            {
                throw new Exception($"Theatre not found with id: {theatreId}");
            }

            // Check if seats already exist for this screen
            var existingSeats = await _seatRepository.GetByTheatreIdAndScreenNumberAsync(theatreId, screenNumber);
            if (existingSeats.Any())
            {
                throw new Exception("Seats already exist for this screen. Delete them first before reinitializing.");
            }

            var seats = new List<Seat>();

            for (int row = 0; row < rows; row++)
            {
                // Convert row number to letter (A, B, C, ...)
                string rowName = ((char)('A' + row)).ToString();

                for (int seatNum = 1; seatNum <= seatsPerRow; seatNum++)
                {
                    var seat = new Seat
                    {
                        Theatre = theatre,
                        TheatreId = theatreId,
                        ScreenNumber = screenNumber,
                        RowName = rowName,
                        SeatNumber = seatNum
                    };

                    // Assign seat types based on position
                    if (row < 2)
                    {
                        // Front rows are standard
                        seat.SeatType = SeatType.STANDARD;
                        seat.PriceMultiplier = 1.0;
                    }
                    else if (row >= 2 && row < rows - 2)
                    {
                        // Middle rows are premium
                        seat.SeatType = SeatType.PREMIUM;
                        seat.PriceMultiplier = 1.2;
                    }
                    else
                    {
                        // Back rows are VIP
                        seat.SeatType = SeatType.VIP;
                        seat.PriceMultiplier = 1.5;
                    }

                    // Mark some seats as accessible
                    if (row == rows / 2 && (seatNum == 1 || seatNum == seatsPerRow))
                    {
                        seat.SeatType = SeatType.ACCESSIBLE;
                        seat.PriceMultiplier = 1.0;
                    }

                    seats.Add(seat);
                }
            }

            foreach (var seat in seats)
            {
                await _seatRepository.AddAsync(seat);
            }
        }

        public async Task<IEnumerable<Seat>> GetSeatsByTheatreAndScreenAsync(long theatreId, int screenNumber)
        {
            return await _seatRepository.GetByTheatreIdAndScreenNumberAsync(theatreId, screenNumber);
        }

        public async Task<Dictionary<string, List<Seat>>> GetSeatMapByTheatreAndScreenAsync(long theatreId, int screenNumber)
        {
            var seats = await _seatRepository.GetByTheatreIdAndScreenNumberAsync(theatreId, screenNumber);

            // Group seats by row for easier display
            return seats
                .GroupBy(s => s.RowName)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderBy(s => s.SeatNumber).ToList()
                )
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public async Task<IEnumerable<Seat>> GetSeatsByTypeAsync(long theatreId, int screenNumber, SeatType seatType)
        {
            return await _seatRepository.GetByTheatreIdAndScreenNumberAndSeatTypeAsync(theatreId, screenNumber, seatType);
        }

        public async Task UpdateSeatTypeAsync(long seatId, SeatType seatType, double priceMultiplier)
        {
            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat != null)
            {
                seat.SeatType = seatType;
                seat.PriceMultiplier = priceMultiplier;
                await _seatRepository.UpdateAsync(seat);
            }
        }

        public async Task UpdateSeatRowTypeAsync(long theatreId, int screenNumber, string rowName, SeatType seatType, double priceMultiplier)
        {
            var rowSeats = await _seatRepository.GetByTheatreIdAndScreenNumberAndRowNameAsync(theatreId, screenNumber, rowName);

            if (!rowSeats.Any())
            {
                throw new Exception("No seats found for the specified row");
            }

            foreach (var seat in rowSeats)
            {
                seat.SeatType = seatType;
                seat.PriceMultiplier = priceMultiplier;
                await _seatRepository.UpdateAsync(seat);
            }
        }

        public async Task<int> BulkUpdateSeatsAsync(List<string> seatIds, long theatreId, int screenNumber, SeatType seatType, double priceMultiplier)
        {
            var seats = await _seatRepository.GetByTheatreIdAndScreenNumberAsync(theatreId, screenNumber);

            var seatsToUpdate = seats.Where(seat =>
                seatIds.Contains(seat.RowName + seat.SeatNumber.ToString())).ToList();

            foreach (var seat in seatsToUpdate)
            {
                seat.SeatType = seatType;
                seat.PriceMultiplier = priceMultiplier;
                await _seatRepository.UpdateAsync(seat);
            }

            return seatsToUpdate.Count;
        }

        public async Task<int> DeleteScreenSeatsAsync(long theatreId, int screenNumber)
        {
            var seats = await _seatRepository.GetByTheatreIdAndScreenNumberAsync(theatreId, screenNumber);
            int count = seats.Count();

            foreach (var seat in seats)
            {
                await _seatRepository.DeleteAsync(seat.Id);
            }

            return count;
        }
    }
}