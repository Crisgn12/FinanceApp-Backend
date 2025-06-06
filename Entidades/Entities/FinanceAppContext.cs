using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Entidades.Entities;

public partial class FinanceAppContext : DbContext
{
    public FinanceAppContext()
    {
    }

    public FinanceAppContext(DbContextOptions<FinanceAppContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Ahorro> Ahorros { get; set; }
    public virtual DbSet<AporteMetaAhorro> AporteMetaAhorros { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Informe> Informes { get; set; }

    public virtual DbSet<Notificacion> Notificaciones { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Presupuesto> Presupuestos { get; set; }

    public virtual DbSet<Transaccion> Transacciones { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ahorro>(entity =>
        {
            entity.HasKey(e => e.AhorroId).HasName("PK__Ahorros__2EC110D59B3D52AA");

            entity.Property(e => e.AhorroId).HasColumnName("AhorroID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MontoActual)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Monto_Actual");
            entity.Property(e => e.MontoObjetivo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Monto_Objetivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.UltimaNotificacion)
               .HasColumnType("datetime")
               .IsRequired(false);
            entity.Property(e => e.FechaMeta)
               .HasColumnType("datetime")
               .HasColumnName("Fecha_Meta");
            entity.HasMany(e => e.Aportes)
               .WithOne(e => e.Ahorro)
               .HasForeignKey(e => e.MetaAhorroId)
               .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Ignore<Usuario>();

        modelBuilder.Entity<AporteMetaAhorro>(entity =>
        {
            entity.HasKey(e => e.AporteId).HasName("PK__AporteMetaAhorro");
            entity.ToTable("AporteMetaAhorro");

            entity.Property(e => e.AporteId).HasColumnName("AporteId");
            entity.Property(e => e.MetaAhorroId).HasColumnName("MetaAhorroId");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .IsUnicode();

            // CONFIGURACIÓN CORREGIDA DE LA RELACIÓN
            entity.HasOne(d => d.Ahorro)
                .WithMany(p => p.Aportes) 
                .HasForeignKey(d => d.MetaAhorroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AporteMetaAhorro_Ahorro");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C5D8D883CF");

            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Categoria)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Usuario");
        });

        modelBuilder.Entity<Informe>(entity =>
        {
            entity.HasKey(e => e.InformeId).HasName("PK__Informes__5B458746F8E94FA3");

            entity.Property(e => e.InformeId).HasColumnName("InformeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.FechaGeneracion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Fecha_generacion");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UrlPdf)
                .HasColumnType("text")
                .HasColumnName("Url_PDF");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Informes)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Informe_Usuario");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.NotificacionId).HasName("PK__Notifica__BCC120C49704C36D");

            entity.Property(e => e.NotificacionId).HasColumnName("NotificacionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Mensaje).HasColumnType("text");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificacion_Usuario");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.PagoId).HasName("PK__Pagos__F00B6158C7E59D39");

            entity.Property(e => e.PagoId).HasColumnName("PagoID");
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime")
                  .HasColumnName("Created_at");

            entity.Property(e => e.Descripcion).HasColumnType("text");

            entity.Property(e => e.Estado)
                  .HasMaxLength(10)
                  .IsUnicode(false);

            entity.Property(e => e.FechaVencimiento)
                  .HasColumnName("Fecha_Vencimiento")
                  .HasColumnType("date");

            entity.Property(e => e.Monto)
                  .HasColumnType("decimal(10, 2)");

            entity.Property(e => e.Titulo)
                  .HasMaxLength(100)
                  .IsUnicode(false);

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.Property(e => e.EsProgramado)
                  .HasColumnName("EsProgramado")
                  .HasColumnType("bit")
                  .HasDefaultValueSql("((0))");

            entity.Property(e => e.Frecuencia)
                  .HasColumnName("Frecuencia")
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.FechaInicio)
                  .HasColumnName("FechaInicio")
                  .HasColumnType("date");

            entity.Property(e => e.FechaFin)
                  .HasColumnName("FechaFin")
                  .HasColumnType("date");

            entity.Property(e => e.ProximoVencimiento)
                  .HasColumnName("ProximoVencimiento")
                  .HasColumnType("date");

            entity.Property(e => e.Activo)
                  .HasColumnName("Activo")
                  .HasColumnType("bit")
                  .HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pagos)
                  .HasForeignKey(d => d.UsuarioId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Pago_Usuario");
        });


        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.HasKey(e => e.PresupuestoId).HasName("PK__Presupue__E2E3631F709E075F");

            entity.Property(e => e.PresupuestoId).HasColumnName("PresupuestoID");
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Limite).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Periodo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Presupuestos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Presupuesto_Categoria");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Presupuestos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Presupuesto_Usuario");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.TransaccionId).HasName("PK__Transacc__86A849DE0B2D4EF4");

            entity.Property(e => e.TransaccionId).HasColumnName("TransaccionID");
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Transacciones)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_Categoria");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Transacciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798161526AD");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105346E23CDA5").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Creacion");
            entity.Property(e => e.LlaveHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Password_hash");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
