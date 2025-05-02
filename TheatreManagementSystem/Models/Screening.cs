using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreManagementSystem.Models
{
    public class Screening
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        public long MovieId { get; set; }

        [Required]
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
        public long TheatreId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int ScreenNumber { get; set; }

        [Required]
        public ScreeningFormat Format { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double BasePrice { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }

    public enum ScreeningFormat
    {
        STANDARD,
        IMAX,
        DOLBY_ATMOS,
        THREE_D,
        FOUR_D
    }
}