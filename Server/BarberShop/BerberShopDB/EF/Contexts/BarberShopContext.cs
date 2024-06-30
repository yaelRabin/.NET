using System;
using System.Collections.Generic;
using BarberShopDB.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberShopDB.EF.Contexts;

public partial class BarberShopContext : DbContext
{
    public BarberShopContext()
    {
    }

    public BarberShopContext(DbContextOptions<BarberShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC29D61341E");

            entity.ToTable("Appointment");

            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");
            entity.Property(e => e.RequestTime).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__UserI__05D8E0BE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C646F8A0C");

            entity.ToTable("User");

            entity.HasIndex(e => e.Phone, "UQ__User__5C7E359EDB9AA2AB").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__User__C9F284567956B673").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(35)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(35)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
