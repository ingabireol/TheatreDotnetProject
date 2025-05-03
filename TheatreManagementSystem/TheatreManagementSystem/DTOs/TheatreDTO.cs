using System.ComponentModel.DataAnnotations;

namespace TheatreManagementSystem.DTOs
{
    public class TheatreDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Theatre name is required")]
        [StringLength(100, ErrorMessage = "Theatre name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Email must be valid")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Total screens must be a positive number")]
        public int TotalScreens { get; set; }

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        public string ImageUrl { get; set; }
    }
}
