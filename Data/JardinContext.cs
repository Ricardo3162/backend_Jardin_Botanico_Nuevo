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

    public virtual DbSet<especie> especies { get; set; }

    public virtual DbSet<especie_ubicacion> especie_ubicacions { get; set; }

    public virtual DbSet<estadoconservacion> estadoconservacions { get; set; }

    public virtual DbSet<persona> personas { get; set; }

    public virtual DbSet<rol> rols { get; set; }

    public virtual DbSet<ubicacion> ubicacions { get; set; }

    public virtual DbSet<uso> usos { get; set; }

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

            entity.HasIndex(e => e.fk_especie, "idx_comentario_especie");

            entity.HasIndex(e => e.fecha_comentario, "idx_comentario_fecha");

            entity.Property(e => e.comentario1)
                .HasColumnType("text")
                .HasColumnName("comentario");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.fecha_comentario)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.fk_especieNavigation).WithMany(p => p.comentarios)
                .HasForeignKey(d => d.fk_especie)
                .HasConstraintName("fk_comentario_especie");
        });

        modelBuilder.Entity<especie>(entity =>
        {
            entity.HasKey(e => e.id_especie).HasName("PRIMARY");

            entity.ToTable("especie");

            entity.HasIndex(e => e.estado, "idx_especie_estado");

            entity.HasIndex(e => e.fk_estado_conservacion, "idx_especie_estado_conserv");

            entity.HasIndex(e => e.fecha_actualizacion, "idx_especie_fecha_actualizacion");

            entity.HasIndex(e => e.fecha_creacion, "idx_especie_fecha_creacion");

            entity.HasIndex(e => e.fk_uso, "idx_especie_uso");

            entity.HasIndex(e => e.codigo_interno_especie, "uq_especie_codigo").IsUnique();

            entity.Property(e => e.codigo_interno_especie).HasMaxLength(50);
            entity.Property(e => e.descripcion_especie).HasColumnType("text");
            entity.Property(e => e.distribucion_caqueta_especie).HasMaxLength(255);
            entity.Property(e => e.distribucion_colombia_especie).HasMaxLength(255);
            entity.Property(e => e.distribucion_mundial_especie).HasMaxLength(255);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.familia_especie).HasMaxLength(255);
            entity.Property(e => e.fecha_actualizacion)
                .HasMaxLength(6)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.fecha_creacion)
                .HasMaxLength(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.fenologia_especie).HasMaxLength(255);
            entity.Property(e => e.nombre_cientifico_especie).HasMaxLength(255);
            entity.Property(e => e.nombre_comun_especie).HasMaxLength(255);
            entity.Property(e => e.observacion_especie).HasColumnType("text");
            entity.Property(e => e.origen_especie).HasMaxLength(255);

            entity.HasOne(d => d.fk_estado_conservacionNavigation).WithMany(p => p.especies)
                .HasForeignKey(d => d.fk_estado_conservacion)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_especie_estado_conserv");

            entity.HasOne(d => d.fk_usoNavigation).WithMany(p => p.especies)
                .HasForeignKey(d => d.fk_uso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_especie_uso");
        });

        modelBuilder.Entity<especie_ubicacion>(entity =>
        {
            entity.HasKey(e => new { e.fk_especie, e.fk_ubicacion })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("especie_ubicacion");

            entity.HasIndex(e => e.fk_ubicacion, "fk_eu_ubicacion");

            entity.HasIndex(e => e.estado, "idx_eu_estado");

            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");

            entity.HasOne(d => d.fk_especieNavigation).WithMany(p => p.especie_ubicacions)
                .HasForeignKey(d => d.fk_especie)
                .HasConstraintName("fk_eu_especie");

            entity.HasOne(d => d.fk_ubicacionNavigation).WithMany(p => p.especie_ubicacions)
                .HasForeignKey(d => d.fk_ubicacion)
                .HasConstraintName("fk_eu_ubicacion");
        });

        modelBuilder.Entity<estadoconservacion>(entity =>
        {
            entity.HasKey(e => e.id_estado).HasName("PRIMARY");

            entity.ToTable("estadoconservacion");

            entity.HasIndex(e => e.estado, "idx_estado_estado");

            entity.HasIndex(e => e.codigo_iucn, "uq_estado_codigo").IsUnique();

            entity.Property(e => e.codigo_iucn).HasMaxLength(10);
            entity.Property(e => e.descripcion_estado).HasColumnType("text");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombre_estado).HasMaxLength(100);
        });

        modelBuilder.Entity<persona>(entity =>
        {
            entity.HasKey(e => e.id_persona).HasName("PRIMARY");

            entity.ToTable("persona");

            entity.HasIndex(e => e.estado, "idx_persona_estado");

            entity.HasIndex(e => e.fk_rol, "idx_persona_rol");

            entity.HasIndex(e => e.correo_persona, "uq_persona_correo").IsUnique();

            entity.Property(e => e.apellidos_persona).HasMaxLength(150);
            entity.Property(e => e.contrasena_persona).HasMaxLength(255);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombres_persona).HasMaxLength(150);
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

        modelBuilder.Entity<uso>(entity =>
        {
            entity.HasKey(e => e.id_uso).HasName("PRIMARY");

            entity.ToTable("uso");

            entity.HasIndex(e => e.nombre_uso, "uq_uso_nombre").IsUnique();

            entity.Property(e => e.id_uso).ValueGeneratedNever();
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'ACTIVO'")
                .HasColumnType("enum('ACTIVO','INACTIVO')");
            entity.Property(e => e.nombre_uso).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
