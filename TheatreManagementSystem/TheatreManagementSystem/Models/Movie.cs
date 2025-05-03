using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreManagementSystem.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int DurationMinutes { get; set; }

        public Genre Genre { get; set; }

        [StringLength(255)]
        public string Director { get; set; }

        [StringLength(255)]
        [Column("movie_cast")]
        public string Cast { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(255)]
        public string PosterImageUrl { get; set; }

        [StringLength(255)]
        public string TrailerUrl { get; set; }

        public Rating Rating { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new HashSet<Screening>();
    }

    public enum Genre
    {
        ACTION,
        ADVENTURE,
        ANIMATION,
        COMEDY,
        CRIME,
        DOCUMENTARY,
        DRAMA,
        FAMILY,
        FANTASY,
        HORROR,
        MUSICAL,
        MYSTERY,
        ROMANCE,
        SCI_FI,
        THRILLER,
        WESTERN
    }

    public enum Rating
    {
        G,
        PG,
        PG13,
        R,
        NC17,
        UNRATED
    }
}

