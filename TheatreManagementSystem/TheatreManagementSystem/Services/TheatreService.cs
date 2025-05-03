using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Repositories;

namespace TheatreManagementSystem.Services
{
    public interface ITheatreService
    {
        Task<TheatreDTO> CreateTheatreAsync(TheatreDTO theatreDTO);
        Task<IEnumerable<TheatreDTO>> GetAllTheatresAsync();
        Task<TheatreDTO> GetTheatreByIdAsync(long id);
        Task<IEnumerable<TheatreDTO>> SearchTheatresByNameAsync(string name);
        Task<IEnumerable<TheatreDTO>> SearchTheatresByAddressAsync(string address);
        Task<IEnumerable<TheatreDTO>> GetTheatresByMovieAsync(long movieId);
        Task<TheatreDTO> UpdateTheatreAsync(long id, TheatreDTO theatreDTO);
        Task DeleteTheatreAsync(long id);
        Task<Theatre> GetTheatreEntityByIdAsync(long id);
    }

    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        public async Task<TheatreDTO> CreateTheatreAsync(TheatreDTO theatreDTO)
        {
            var theatre = ConvertToEntity(theatreDTO);
            var savedTheatre = await _theatreRepository.AddAsync(theatre);
            return ConvertToDTO(savedTheatre);
        }

        public async Task<IEnumerable<TheatreDTO>> GetAllTheatresAsync()
        {
            var theatres = await _theatreRepository.GetAllAsync();
            return theatres.Select(ConvertToDTO);
        }

        public async Task<TheatreDTO> GetTheatreByIdAsync(long id)
        {
            var theatre = await _theatreRepository.GetByIdAsync(id);
            return theatre != null ? ConvertToDTO(theatre) : null;
        }

        public async Task<IEnumerable<TheatreDTO>> SearchTheatresByNameAsync(string name)
        {
            var theatres = await _theatreRepository.GetByNameContainingAsync(name);
            return theatres.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<TheatreDTO>> SearchTheatresByAddressAsync(string address)
        {
            var theatres = await _theatreRepository.GetByAddressContainingAsync(address);
            return theatres.Select(ConvertToDTO);
        }

        public async Task<IEnumerable<TheatreDTO>> GetTheatresByMovieAsync(long movieId)
        {
            var theatres = await _theatreRepository.GetTheatresByMovieIdAsync(movieId);
            return theatres.Select(ConvertToDTO);
        }

        public async Task<TheatreDTO> UpdateTheatreAsync(long id, TheatreDTO theatreDTO)
        {
            var existingTheatre = await _theatreRepository.GetByIdAsync(id);
            if (existingTheatre == null)
            {
                return null;
            }

            // Update properties
            existingTheatre.Name = theatreDTO.Name;
            existingTheatre.Address = theatreDTO.Address;
            existingTheatre.PhoneNumber = theatreDTO.PhoneNumber;
            existingTheatre.Email = theatreDTO.Email;
            existingTheatre.Description = theatreDTO.Description;
            existingTheatre.TotalScreens = theatreDTO.TotalScreens;
            existingTheatre.ImageUrl = theatreDTO.ImageUrl;

            await _theatreRepository.UpdateAsync(existingTheatre);
            return ConvertToDTO(existingTheatre);
        }

        public async Task DeleteTheatreAsync(long id)
        {
            await _theatreRepository.DeleteAsync(id);
        }

        public async Task<Theatre> GetTheatreEntityByIdAsync(long id)
        {
            return await _theatreRepository.GetByIdAsync(id);
        }

        // Helper methods for conversion
        private TheatreDTO ConvertToDTO(Theatre theatre)
        {
            return new TheatreDTO
            {
                Id = (int)theatre.Id,
                Name = theatre.Name,
                Address = theatre.Address,
                PhoneNumber = theatre.PhoneNumber,
                Email = theatre.Email,
                Description = theatre.Description,
                TotalScreens = theatre.TotalScreens,
                ImageUrl = theatre.ImageUrl
            };
        }

        private Theatre ConvertToEntity(TheatreDTO dto)
        {
            return new Theatre
            {
                Id = dto.Id > 0 ? dto.Id : 0, // Don't set ID for new entities
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Description = dto.Description,
                TotalScreens = dto.TotalScreens,
                ImageUrl = dto.ImageUrl
            };
        }
    }
}