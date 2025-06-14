﻿// <auto-generated />
using System;
using Entidades.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entidades.Migrations.FinanceApp
{
    [DbContext(typeof(FinanceAppContext))]
    partial class FinanceAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entidades.Entities.Ahorro", b =>
                {
                    b.Property<int>("AhorroId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AhorroID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AhorroId"));

                    b.Property<bool>("Completado")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("FechaMeta")
                        .HasColumnType("datetime")
                        .HasColumnName("Fecha_Meta");

                    b.Property<decimal>("MontoActual")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("Monto_Actual");

                    b.Property<decimal>("MontoObjetivo")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("Monto_Objetivo");

                    b.Property<string>("Nombre")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("UltimaNotificacion")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("UsuarioID");

                    b.Property<int?>("UsuarioId1")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioId2")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AhorroId")
                        .HasName("PK__Ahorros__2EC110D59B3D52AA");

                    b.HasIndex("UsuarioId");

                    b.HasIndex("UsuarioId1");

                    b.HasIndex("UsuarioId2");

                    b.ToTable("Ahorros");
                });

            modelBuilder.Entity("Entidades.Entities.AporteMetaAhorro", b =>
                {
                    b.Property<int>("AporteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AporteId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AporteId"));

                    b.Property<DateTime>("Fecha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("MetaAhorroId")
                        .HasColumnType("int")
                        .HasColumnName("MetaAhorroId");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Observaciones")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("AporteId")
                        .HasName("PK__AporteMetaAhorro");

                    b.HasIndex("MetaAhorroId");

                    b.ToTable("AporteMetaAhorro", (string)null);
                });

            modelBuilder.Entity("Entidades.Entities.Categoria", b =>
                {
                    b.Property<int>("CategoriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CategoriaID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoriaId"));

                    b.Property<bool>("EsPredeterminada")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("CategoriaId")
                        .HasName("PK__Categori__F353C1C5D8D883CF");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("Entidades.Entities.Informe", b =>
                {
                    b.Property<int>("InformeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("InformeID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InformeId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime>("FechaGeneracion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Fecha_generacion")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("UrlPdf")
                        .HasColumnType("text")
                        .HasColumnName("Url_PDF");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("InformeId")
                        .HasName("PK__Informes__5B458746F8E94FA3");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Informes");
                });

            modelBuilder.Entity("Entidades.Entities.Notificacion", b =>
                {
                    b.Property<int>("NotificacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("NotificacionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificacionId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<bool>("Leida")
                        .HasColumnType("bit");

                    b.Property<string>("Mensaje")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("NotificacionId")
                        .HasName("PK__Notifica__BCC120C49704C36D");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Notificaciones");
                });

            modelBuilder.Entity("Entidades.Entities.Pago", b =>
                {
                    b.Property<int>("PagoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PagoID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PagoId"));

                    b.Property<bool>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasColumnName("Activo")
                        .HasDefaultValueSql("((1))");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("EsProgramado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasColumnName("EsProgramado")
                        .HasDefaultValueSql("((0))");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<DateOnly?>("FechaFin")
                        .HasColumnType("date")
                        .HasColumnName("FechaFin");

                    b.Property<DateOnly?>("FechaInicio")
                        .HasColumnType("date")
                        .HasColumnName("FechaInicio");

                    b.Property<DateOnly>("FechaVencimiento")
                        .HasColumnType("date")
                        .HasColumnName("Fecha_Vencimiento");

                    b.Property<string>("Frecuencia")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Frecuencia");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateOnly?>("ProximoVencimiento")
                        .HasColumnType("date")
                        .HasColumnName("ProximoVencimiento");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("PagoId")
                        .HasName("PK__Pagos__F00B6158C7E59D39");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Pagos");
                });

            modelBuilder.Entity("Entidades.Entities.Presupuesto", b =>
                {
                    b.Property<int>("PresupuestoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PresupuestoID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PresupuestoId"));

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int")
                        .HasColumnName("CategoriaID");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<decimal>("Limite")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Periodo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("PresupuestoId")
                        .HasName("PK__Presupue__E2E3631F709E075F");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Presupuestos");
                });

            modelBuilder.Entity("Entidades.Entities.Transaccion", b =>
                {
                    b.Property<int>("TransaccionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("TransaccionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransaccionId"));

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int")
                        .HasColumnName("CategoriaID");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Descripcion")
                        .HasColumnType("text");

                    b.Property<DateOnly>("Fecha")
                        .HasColumnType("date");

                    b.Property<decimal>("Monto")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("TransaccionId")
                        .HasName("PK__Transacc__86A849DE0B2D4EF4");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Transacciones");
                });

            modelBuilder.Entity("Entidades.Entities.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioId"));

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTime?>("FechaCreacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("Fecha_Creacion")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("LlaveHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Password_hash");

                    b.HasKey("UsuarioId")
                        .HasName("PK__Usuarios__2B3DE798161526AD");

                    b.HasIndex(new[] { "Email" }, "UQ__Usuarios__A9D105346E23CDA5")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IdentityUser");
                });

            modelBuilder.Entity("Entidades.Entities.Ahorro", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Ahorro_AspNetUsers");

                    b.HasOne("Entidades.Entities.Usuario", null)
                        .WithMany("Ahorros")
                        .HasForeignKey("UsuarioId1");

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.AporteMetaAhorro", b =>
                {
                    b.HasOne("Entidades.Entities.Ahorro", "Ahorro")
                        .WithMany("Aportes")
                        .HasForeignKey("MetaAhorroId")
                        .IsRequired()
                        .HasConstraintName("FK_AporteMetaAhorro_Ahorro");

                    b.Navigation("Ahorro");
                });

            modelBuilder.Entity("Entidades.Entities.Categoria", b =>
                {
                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Categoria")
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_Categoria_Usuario");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Informe", b =>
                {
                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Informes")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Informe_Usuario");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Notificacion", b =>
                {
                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Notificaciones")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Notificacion_Usuario");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Pago", b =>
                {
                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Pagos")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Pago_Usuario");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Presupuesto", b =>
                {
                    b.HasOne("Entidades.Entities.Categoria", "Categoria")
                        .WithMany("Presupuestos")
                        .HasForeignKey("CategoriaId")
                        .IsRequired()
                        .HasConstraintName("FK_Presupuesto_Categoria");

                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Presupuestos")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Presupuesto_Usuario");

                    b.Navigation("Categoria");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Transaccion", b =>
                {
                    b.HasOne("Entidades.Entities.Categoria", "Categoria")
                        .WithMany("Transacciones")
                        .HasForeignKey("CategoriaId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaccion_Categoria");

                    b.HasOne("Entidades.Entities.Usuario", "Usuario")
                        .WithMany("Transacciones")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaccion_Usuario");

                    b.Navigation("Categoria");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Entidades.Entities.Ahorro", b =>
                {
                    b.Navigation("Aportes");
                });

            modelBuilder.Entity("Entidades.Entities.Categoria", b =>
                {
                    b.Navigation("Presupuestos");

                    b.Navigation("Transacciones");
                });

            modelBuilder.Entity("Entidades.Entities.Usuario", b =>
                {
                    b.Navigation("Ahorros");

                    b.Navigation("Categoria");

                    b.Navigation("Informes");

                    b.Navigation("Notificaciones");

                    b.Navigation("Pagos");

                    b.Navigation("Presupuestos");

                    b.Navigation("Transacciones");
                });
#pragma warning restore 612, 618
        }
    }
}
