using Microsoft.EntityFrameworkCore;
using ProjectCinema.Entities;

namespace ProjectCinema.Data
{
    public class AplicationDBContext : DbContext
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options) : base(options) { }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieScreening> MovieScreenings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promocode> Promocodes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<StreamingAccess> StreamingAccesses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // configuration one-to-many relationship between entities Booking and User
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Booking and Promocode
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Promocode)
                .WithMany(promo => promo.Bookings)
                .HasForeignKey(b => b.PromocodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-one relationship between entities Payment and Booking 
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.Cascade);

            // configuration one-to-many relationship between entities Ticket and Booking
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Ticket and Showtime
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ShowTime)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.ShowTimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Ticket and Seat
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // restriction that allowed only unique combination beetween seat and showtime into ticket
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.SeatId, t.ShowTimeId })
                .IsUnique();

            // configuration one-to-many relationship between entities ShowTime and Hall
            modelBuilder.Entity<ShowTime>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.ShowTimes)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities ShowTime and MovieScreening
            modelBuilder.Entity<ShowTime>()
                .HasOne(s => s.MovieScreening)
                .WithMany(m => m.ShowTimes)
                .HasForeignKey(s => s.MovieScreeningId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Movie and MovieScreening
            modelBuilder.Entity<MovieScreening>()
                .HasOne(m => m.Movie)
                .WithMany(mov => mov.MovieScreenings)
                .HasForeignKey(m => m.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Movie and Cinema
            modelBuilder.Entity<MovieScreening>()
                .HasOne(m => m.Cinema)
                .WithMany(c => c.MovieScreenings)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            // restriction that allowed only unique combination beetween seat and Movie into Cinema
            modelBuilder.Entity<MovieScreening>()
                .HasIndex(mt => new { mt.MovieId, mt.CinemaId })
                .IsUnique();

            // configuration one-to-many relationship between entities Hall and Cinema
            modelBuilder.Entity<Hall>()
                .HasOne(h => h.Cinema)
                .WithMany(c => c.Halls)
                .HasForeignKey(h => h.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Hall and Seat
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Row)
                .WithMany(r => r.Seats)
                .HasForeignKey(s => s.RowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Row>()
                .HasOne(r => r.Hall)
                .WithMany(h => h.Rows)
                .HasForeignKey(h => h.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities StreamingAccess and User
            modelBuilder.Entity<StreamingAccess>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-one relationship between entities StreamingAccess and Payment
            modelBuilder.Entity<StreamingAccess>()
                .HasOne(s => s.Payment)
                .WithOne(p => p.StreamingAccess)
                .HasForeignKey<StreamingAccess>(s => s.PaymentId)
                .IsRequired(false)  // PaymentId is now nullable
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Movie and StreamingAccess
            modelBuilder.Entity<StreamingAccess>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.StreamingAccesses)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Notification and Booking
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Booking)
                .WithMany()
                .HasForeignKey(n => n.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Review and User
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // configuration one-to-many relationship between entities Review and Movie
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany()
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
