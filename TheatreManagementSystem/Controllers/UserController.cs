using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;

        public UserController(IUserService userService, IBookingService bookingService)
        {
            _userService = userService;
            _bookingService = bookingService;
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-username/{username}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return NotFound($"User with username {username} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                string username = User.Identity.Name;
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return NotFound("User profile not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, userDTO);
                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string username = User.Identity.Name;
                var currentUser = await _userService.GetUserByUsernameAsync(username);
                if (currentUser == null)
                {
                    return NotFound("User profile not found");
                }

                // Ensure users can only update their own profile
                userDTO.Id = currentUser.Id;

                var updatedUser = await _userService.UpdateUserAsync(currentUser.Id, userDTO);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateUserRole(long id, [FromQuery] Role role)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserRoleAsync(id, role);
                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/bookings")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetUserBookings(long id)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(id);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("profile/bookings")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserBookings()
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
    }
}