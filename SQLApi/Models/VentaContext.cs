using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SQLApi.Models;

public partial class VentaContext : DbContext
{
    public VentaContext()
    {
    }

    public VentaContext(DbContextOptions<VentaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Comidum> Comida { get; set; }

    public virtual DbSet<DetalleTicket> DetalleTickets { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=DB_VENTA;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.HasKey(e => e.AbonoId).HasName("PK__Abonos__0038E86AA02CC235");

            entity.Property(e => e.AbonoId).HasColumnName("AbonoID");
            entity.Property(e => e.AlumnoId).HasColumnName("AlumnoID");
            entity.Property(e => e.FechaAbono)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Observaciones).HasMaxLength(255);
            entity.Property(e => e.TicketId).HasColumnName("TicketID");

            entity.HasOne(d => d.Alumno).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.AlumnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abonos__AlumnoID__245D67DE");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abonos__TicketID__236943A5");
        });

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.AlumnoId).HasName("PK__Alumnos__90A6AA338167CD93");

            entity.HasIndex(e => e.Matricula, "UC_Alumno").IsUnique();

            entity.Property(e => e.AlumnoId).HasColumnName("AlumnoID");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Grupo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Matricula)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TelefonoTutor)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comidum>(entity =>
        {
            entity.HasKey(e => e.Idcomida).HasName("PK__Comida__0A2DB122156D6533");

            entity.Property(e => e.Idcomida).HasColumnName("IDComida");
            entity.Property(e => e.CodigoBarras)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.NombreComida)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<DetalleTicket>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK__DetalleT__6E19D6FA3F8FDD24");

            entity.Property(e => e.DetalleId).HasColumnName("DetalleID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Producto).HasMaxLength(100);
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([Cantidad]*[Precio])", false)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.TicketId).HasColumnName("TicketID");

            entity.HasOne(d => d.Ticket).WithMany(p => p.DetalleTickets)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleTi__Ticke__1F98B2C1");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpresaId).HasName("PK__Empresa__7B9F2136330353A7");

            entity.ToTable("Empresa");

            entity.Property(e => e.EmpresaId).HasColumnName("EmpresaID");
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Idproducto).HasName("PK__Producto__ABDAF2B472D594AE");

            entity.HasIndex(e => e.CodigoDeBarras, "UQ__Producto__F6D2C3DBCE0914E9").IsUnique();

            entity.Property(e => e.Idproducto).HasColumnName("IDProducto");
            entity.Property(e => e.CodigoDeBarras)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC6270897BEB0");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.AlumnoId).HasColumnName("AlumnoID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Abierto");
            entity.Property(e => e.FechaApertura)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaCierre).HasColumnType("datetime");
            entity.Property(e => e.FechaCierreProgramada).HasColumnType("datetime");
            entity.Property(e => e.SaldoPendiente)
                .HasComputedColumnSql("([Total]-[TotalPagado])", false)
                .HasColumnType("decimal(11, 2)");
            entity.Property(e => e.Semana).HasMaxLength(10);
            entity.Property(e => e.Total)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPagado)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Alumno).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.AlumnoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__AlumnoI__1BC821DD");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798460FBD6E");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Contraseña).HasMaxLength(510);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Usuario)
                .HasMaxLength(100)
                .HasColumnName("Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
