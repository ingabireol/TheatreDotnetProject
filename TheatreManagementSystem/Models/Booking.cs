using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreManagementSystem.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
        public long UserId { get; set; }

        [Required]
        [ForeignKey("ScreeningId")]
        public Screening Screening { get; set; }
        public long ScreeningId { get; set; }

        [Required]
        public string BookingNumber { get; set; }

        [Required]
        public DateTime BookingTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double TotalAmount { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        public ICollection<string> BookedSeats { get; set; } = new HashSet<string>();
    }

    public enum PaymentStatus
    {
        PENDING,
        COMPLETED,
        CANCELLED,
        REFUNDED
    }
}
