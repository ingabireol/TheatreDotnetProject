using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreManagementSystem.Models
{
    public class Theatre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalScreens { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new HashSet<Screening>();

        public ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();
    }
}
