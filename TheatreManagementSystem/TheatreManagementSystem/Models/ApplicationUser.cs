using Microsoft.AspNetCore.Identity;
using TheatreManagementSystem.Models;
using System.Collections.Generic;

namespace TheatreManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation property for bookings
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }
}