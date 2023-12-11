using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SanoshAirlines.Models;

public partial class AirlineDbContext : DbContext
{
    public AirlineDbContext()
    {
    }

    public AirlineDbContext(DbContextOptions<AirlineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<ConnectionFlightTicket> ConnectionFlightTickets { get; set; }

    public virtual DbSet<FlightDetail> FlightDetails { get; set; }

    public virtual DbSet<FlightSchedule> FlightSchedules { get; set; }

    public virtual DbSet<FlightTicket> FlightTickets { get; set; }

    public virtual DbSet<PartnerBooking> PartnerBookings { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PSILENL040;Database=SanoshAirlinesDb;Trusted_Connection=True;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__Airports__E3DBE0EAE16127A6");

            entity.Property(e => e.AirportId).HasMaxLength(255);
            entity.Property(e => e.AirportName).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.State).HasMaxLength(255);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.BookingId).ValueGeneratedNever();
            entity.Property(e => e.BookingType).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
        });

        modelBuilder.Entity<ConnectionFlightTicket>(entity =>
        {
            entity.HasKey(e => e.TicketNo);

            entity.Property(e => e.AirlineName).HasMaxLength(255);
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.DestinationAirportId).HasMaxLength(255);
            entity.Property(e => e.FlightName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SeatNo).HasMaxLength(255);
            entity.Property(e => e.SourceAirportId).HasMaxLength(255);

            /*entity.HasOne(d => d.Booking).WithMany(p => p.ConnectionFlightTickets).HasForeignKey(d => d.BookingId);

            entity.HasOne(d => d.DestinationAirport).WithMany(p => p.ConnectionFlightTicketDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.SourceAirport).WithMany(p => p.ConnectionFlightTicketSourceAirports).HasForeignKey(d => d.SourceAirportId);*/
        });

        modelBuilder.Entity<FlightDetail>(entity =>
        {
            entity.HasKey(e => e.FlightName).HasName("PK__FlightDe__32AC470FCEFACDCC");

            entity.Property(e => e.FlightName).HasMaxLength(255);
            entity.Property(e => e.FlightId).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

        modelBuilder.Entity<FlightSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);

            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.DestinationAirportId).HasMaxLength(255);
            entity.Property(e => e.FlightName).HasMaxLength(255);
            entity.Property(e => e.SourceAirportId).HasMaxLength(255);

          /*  entity.HasOne(d => d.DestinationAirport).WithMany(p => p.FlightScheduleDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FlightNameNavigation).WithMany(p => p.FlightSchedules).HasForeignKey(d => d.FlightName);

            entity.HasOne(d => d.SourceAirport).WithMany(p => p.FlightScheduleSourceAirports)
                .HasForeignKey(d => d.SourceAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull);*/
        });

        modelBuilder.Entity<FlightTicket>(entity =>
        {
            entity.HasKey(e => e.TicketNo);

            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SeatNo).HasMaxLength(255);

           /* entity.HasOne(d => d.Booking).WithMany(p => p.FlightTickets).HasForeignKey(d => d.BookingId);

            entity.HasOne(d => d.Schedule).WithMany(p => p.FlightTickets).HasForeignKey(d => d.ScheduleId);*/
        });

        modelBuilder.Entity<PartnerBooking>(entity =>
        {
            entity.HasKey(e => new { e.TicketNo, e.AirlineName });

            entity.Property(e => e.AirlineName).HasMaxLength(255);
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.DestinationAirportId).HasMaxLength(255);
            entity.Property(e => e.FlightName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.SeatNo).HasMaxLength(255);
            entity.Property(e => e.SourceAirportId).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);

           /* entity.HasOne(d => d.DestinationAirport).WithMany(p => p.PartnerBookingDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.SourceAirport).WithMany(p => p.PartnerBookingSourceAirports).HasForeignKey(d => d.SourceAirportId);*/
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => new { e.ScheduleId, e.SeatNumber });

            entity.Property(e => e.SeatNumber).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);

          /*  entity.HasOne(d => d.Schedule).WithMany(p => p.Seats).HasForeignKey(d => d.ScheduleId);*/
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users");

            entity.ToTable("User");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
