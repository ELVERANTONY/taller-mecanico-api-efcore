using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TallerMecanico.Api.Models;

public partial class TallerContext : DbContext
{
    public TallerContext()
    {
    }

    public TallerContext(DbContextOptions<TallerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PRIMARY");

            entity.HasIndex(e => e.Documento, "UQ_Clientes_Documento").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_Clientes_Email").IsUnique();

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Apellido).HasMaxLength(80);
            entity.Property(e => e.Direccion).HasMaxLength(150);
            entity.Property(e => e.Documento).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(80);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.VehiculoId).HasName("PRIMARY");

            entity.HasIndex(e => e.ClienteId, "IX_Vehiculos_ClienteId");

            entity.HasIndex(e => e.Placa, "UQ_Vehiculos_Placa").IsUnique();

            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Placa).HasMaxLength(10);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Vehiculos_Clientes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
