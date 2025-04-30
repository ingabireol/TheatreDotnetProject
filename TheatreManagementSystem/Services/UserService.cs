using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = ConvertRoleStringToEnum(roles.FirstOrDefault() ?? "ROLE_USER");

                userDTOs.Add(new UserDTO
                {
                    Id = int.Parse(user.Id),
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Role = userRole
                });
            }

            return userDTOs;
        }

        public async Task<UserDTO> GetUserByIdAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = ConvertRoleStringToEnum(roles.FirstOrDefault() ?? "ROLE_USER");

            return new UserDTO
            {
                Id = int.Parse(user.Id),
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = userRole
            };
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = ConvertRoleStringToEnum(roles.FirstOrDefault() ?? "ROLE_USER");

            return new UserDTO
            {
                Id = int.Parse(user.Id),
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = userRole
            };
        }

        public async Task<UserDTO> UpdateUserAsync(long id, UserDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            // Update user properties
            user.Email = userDTO.Email;
            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.PhoneNumber = userDTO.PhoneNumber;

            // Update user
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Update password if provided
            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, userDTO.Password);
                if (!passwordResult.Succeeded)
                    throw new Exception($"Failed to update password: {string.Join(", ", passwordResult.Errors.Select(e => e.Description))}");
            }

            return await GetUserByIdAsync(id);
        }

        public async Task<UserDTO> UpdateUserRoleAsync(long id, Role role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            // Get current roles and remove them
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role
            var roleString = ConvertRoleEnumToString(role);
            if (!await _roleManager.RoleExistsAsync(roleString))
                await _roleManager.CreateAsync(new IdentityRole(roleString));

            await _userManager.AddToRoleAsync(user, roleString);

            return await GetUserByIdAsync(id);
        }

        public async Task DeleteUserAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new Exception("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        private Role ConvertRoleStringToEnum(string role)
        {
            return role switch
            {
                "ROLE_ADMIN" => Role.ROLE_ADMIN,
                "ROLE_MANAGER" => Role.ROLE_MANAGER,
                _ => Role.ROLE_USER,
            };
        }

        private string ConvertRoleEnumToString(Role role)
        {
            return role.ToString();
        }
    }

    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(long id);
        Task<UserDTO> GetUserByUsernameAsync(string username);
        Task<UserDTO> UpdateUserAsync(long id, UserDTO userDTO);
        Task<UserDTO> UpdateUserRoleAsync(long id, Role role);
        Task DeleteUserAsync(long id);
    }
}
