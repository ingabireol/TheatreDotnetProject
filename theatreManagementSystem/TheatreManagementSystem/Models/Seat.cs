using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreManagementSystem.Models
{
    public class Seat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey("TheatreId")]
        public Theatre Theatre { get; set; }
        public long TheatreId { get; set; }

        [Required]
        public int ScreenNumber { get; set; }

        [Required]
        public string RowName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SeatNumber { get; set; }

        [Required]
        public SeatType SeatType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double PriceMultiplier { get; set; }
    }

    public enum SeatType
    {
        STANDARD,
        PREMIUM,
        VIP,
        ACCESSIBLE
    }
}