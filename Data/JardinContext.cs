using System;
using System.Collections.Generic;
using Backend_Jardin.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Backend_Jardin.Data;

public partial class JardinContext : DbContext
{
    public JardinContext()
    {
    }

    public JardinContext(DbContextOptions<JardinContext> options)
        : base(options)
    {
    }

    public virtual DbSet<comentario> comentarios { get; set; }

    public virtual DbSet<ejemplar> ejemplars { get; set; }

    public virtual DbSet<especie> especies { get; set; }

    public virtual DbSet<persona> personas { get; set; }

    public virtual DbSet<rol> rols { get; set; }

    public virtual DbSet<ubicacion> ubicacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=BD_Jardin_Botanico;user id=root;password=root;treattinyasboolean=true", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"), x => x.UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<comentario>(entity =>
        {
            entity.HasKey(e => e.id_comentario).HasName("PRIMARY");

            entity.ToTable("comentario");

            entity.HasIndex(e => e.fk_ejemplar, "idx_comentario_ejemplar");

            entity.HasIndex(e => e.estado, "idx_comentario_estado");

            entity.Property(e => e.contenido_comentario).HasColumnType("text");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.fecha_comentario)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.fk_ejemplarNavigation).WithMany(p => p.comentarios)
                .HasForeignKey(d => d.fk_ejemplar)
                .HasConstraintName("fk_comentario_ejemplar");
        });

        modelBuilder.Entity<ejemplar>(entity =>
        {
            entity.HasKey(e => e.id_ejemplar).HasName("PRIMARY");

            entity.ToTable("ejemplar");

            entity.HasIndex(e => e.fk_especie, "idx_ejemplar_especie");

            entity.HasIndex(e => e.estado, "idx_ejemplar_estado");

            entity.HasIndex(e => e.fk_ubicacion, "idx_ejemplar_ubicacion");

            entity.HasIndex(e => e.codigo_interno_ejemplar, "uq_ejemplar_codigo").IsUnique();

            entity.Property(e => e.codigo_interno_ejemplar).HasMaxLength(50);
            entity.Property(e => e.coordenadas_ejemplar).HasMaxLength(100);
            entity.Property(e => e.detalle_ubicacion).HasMaxLength(255);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.fecha_registro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.fk_especieNavigation).WithMany(p => p.ejemplars)
                .HasForeignKey(d => d.fk_especie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ejemplar_especie");

            entity.HasOne(d => d.fk_ubicacionNavigation).WithMany(p => p.ejemplars)
                .HasForeignKey(d => d.fk_ubicacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ejemplar_ubicacion");
        });

        modelBuilder.Entity<especie>(entity =>
        {
            entity.HasKey(e => e.id_especie).HasName("PRIMARY");

            entity.ToTable("especie");

            entity.HasIndex(e => e.estado, "idx_especie_estado");

            entity.Property(e => e.descripcion_especie).HasColumnType("text");
            entity.Property(e => e.distribucion_caqueta_especie).HasMaxLength(255);
            entity.Property(e => e.distribucion_colombia_especie).HasMaxLength(255);
            entity.Property(e => e.distribucion_mundial_especie).HasMaxLength(255);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.fenologia_especie).HasMaxLength(255);
            entity.Property(e => e.nombre_cientifico_especie).HasMaxLength(255);
            entity.Property(e => e.nombre_comun_especie).HasMaxLength(255);
            entity.Property(e => e.origen_especie).HasMaxLength(255);
            entity.Property(e => e.uso_especie).HasMaxLength(255);
        });

        modelBuilder.Entity<persona>(entity =>
        {
            entity.HasKey(e => e.id_persona).HasName("PRIMARY");

            entity.ToTable("persona");

            entity.HasIndex(e => e.estado, "idx_persona_estado");

            entity.HasIndex(e => e.fk_rol, "idx_persona_rol");

            entity.HasIndex(e => e.correo_persona, "uq_persona_correo").IsUnique();

            entity.Property(e => e.contrasena_persona).HasMaxLength(255);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombre_completo_persona).HasMaxLength(255);
            entity.Property(e => e.telefono_persona).HasMaxLength(20);

            entity.HasOne(d => d.fk_rolNavigation).WithMany(p => p.personas)
                .HasForeignKey(d => d.fk_rol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_persona_rol");
        });

        modelBuilder.Entity<rol>(entity =>
        {
            entity.HasKey(e => e.id_rol).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.HasIndex(e => e.estado, "idx_rol_estado");

            entity.Property(e => e.descripcion_rol).HasColumnType("text");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombre_rol).HasMaxLength(100);
        });

        modelBuilder.Entity<ubicacion>(entity =>
        {
            entity.HasKey(e => e.id_ubicacion).HasName("PRIMARY");

            entity.ToTable("ubicacion");

            entity.HasIndex(e => e.estado, "idx_ubicacion_estado");

            entity.Property(e => e.descripcion_ubicacion).HasColumnType("text");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombre_ubicacion).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
