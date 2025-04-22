using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Models;

public partial class StarTravelContext : DbContext
{
    public StarTravelContext()
    {
    }

    public StarTravelContext(DbContextOptions<StarTravelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightPassenger> FlightPassengers { get; set; }

    public virtual DbSet<FlightPath> FlightPaths { get; set; }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<Ship> Ships { get; set; }

    public virtual DbSet<StarSystem> StarSystems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Darkskub\\Desktop\\University\\Github\\C#\\StarTravelDispatcher\\DataBase\\Database1.mdf;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Flights__3214EC07DD5E27E3");

            entity.HasOne(d => d.Ship).WithMany(p => p.Flights)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flights__ShipId__3F466844");
        });

        modelBuilder.Entity<FlightPassenger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlightPa__3214EC075C99B71D");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightPassengers).HasConstraintName("FK__FlightPas__Fligh__45F365D3");

            entity.HasOne(d => d.Passenger).WithMany(p => p.FlightPassengers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightPas__Passe__46E78A0C");
        });

        modelBuilder.Entity<FlightPath>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlightPa__3214EC0774E47D1A");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightPaths).HasConstraintName("FK__FlightPat__Fligh__4222D4EF");

            entity.HasOne(d => d.StarSystem).WithMany(p => p.FlightPaths)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightPat__StarS__4316F928");
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Passenge__3214EC0725F87A31");
        });

        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ships__3214EC072B8248C9");
        });

        modelBuilder.Entity<StarSystem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StarSyst__3214EC07F26BE759");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07423F8C1B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
