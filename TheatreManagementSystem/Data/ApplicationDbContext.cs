using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheatreManagementSystem.Models;

namespace TheatreManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Movie entity
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("Movies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.DurationMinutes).IsRequired();
                entity.Property(e => e.Genre).HasConversion<string>();
                entity.Property(e => e.Director).HasMaxLength(255);
                entity.Property(e => e.Cast).HasMaxLength(255).HasColumnName("movie_cast");
                entity.Property(e => e.ReleaseDate);
                entity.Property(e => e.PosterImageUrl).HasMaxLength(255);
                entity.Property(e => e.TrailerUrl).HasMaxLength(255);
                entity.Property(e => e.Rating).HasConversion<string>();
            });

            // Configure Theatre entity
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.ToTable("Theatres");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.TotalScreens);
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
            });

            // Configure Screening entity
            modelBuilder.Entity<Screening>(entity =>
            {
                entity.ToTable("Screenings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.ScreenNumber).IsRequired();
                entity.Property(e => e.Format).HasConversion<string>().IsRequired();
                entity.Property(e => e.BasePrice).IsRequired();

                entity.HasOne(e => e.Movie)
                    .WithMany(m => m.Screenings)
                    .HasForeignKey(e => e.MovieId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Theatre)
                    .WithMany(t => t.Screenings)
                    .HasForeignKey(e => e.TheatreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Seat entity
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.ToTable("Seats");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ScreenNumber).IsRequired();
                entity.Property(e => e.RowName).IsRequired();
                entity.Property(e => e.SeatNumber).IsRequired();
                entity.Property(e => e.SeatType).HasConversion<string>().IsRequired();
                entity.Property(e => e.PriceMultiplier).IsRequired();

                entity.HasOne(e => e.Theatre)
                    .WithMany(t => t.Seats)
                    .HasForeignKey(e => e.TheatreId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookingNumber).IsRequired();
                entity.Property(e => e.BookingTime).IsRequired();
                entity.Property(e => e.TotalAmount).IsRequired();
                entity.Property(e => e.PaymentStatus).HasConversion<string>().IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Screening)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(e => e.ScreeningId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure BookedSeats collection
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookedSeats)
                .HasConversion(
                    v => string.Join(',', v),
                    v => new HashSet<string>(v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                );
        }
    }
}
