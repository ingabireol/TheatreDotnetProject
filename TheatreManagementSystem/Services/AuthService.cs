using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(UserDTO userDTO)
        {
            var userExists = await _userManager.FindByNameAsync(userDTO.Username);
            if (userExists != null)
                return (false, "Username already exists");

            var emailExists = await _userManager.FindByEmailAsync(userDTO.Email);
            if (emailExists != null)
                return (false, "Email already exists");

            var user = new ApplicationUser
            {
                UserName = userDTO.Username,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                PhoneNumber = userDTO.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, userDTO.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign default role (user)
            if (!await _roleManager.RoleExistsAsync("ROLE_USER"))
                await _roleManager.CreateAsync(new IdentityRole("ROLE_USER"));

            await _userManager.AddToRoleAsync(user, "ROLE_USER");

            return (true, "User registered successfully");
        }

        public async Task<(bool Success, string Token, string Message)> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return (false, null, "User not found");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                return (false, null, "Invalid password");

            var token = await GenerateJwtTokenAsync(user);
            return (true, token, "Login successful");
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterUserAsync(UserDTO userDTO);
        Task<(bool Success, string Token, string Message)> LoginAsync(string username, string password);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
