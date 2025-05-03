using System;
using System.ComponentModel.DataAnnotations;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.DTOs
{
    public class MovieDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Duration must be a positive number")]
        public int DurationMinutes { get; set; }

        public Genre Genre { get; set; }

        [StringLength(255, ErrorMessage = "Director name cannot exceed 255 characters")]
        public string Director { get; set; }

        [StringLength(255, ErrorMessage = "Cast info cannot exceed 255 characters")]
        public string Cast { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(255, ErrorMessage = "Poster URL cannot exceed 255 characters")]
        public string PosterImageUrl { get; set; }

        [StringLength(255, ErrorMessage = "Trailer URL cannot exceed 255 characters")]
        public string TrailerUrl { get; set; }

        public Rating Rating { get; set; }
    }
}

