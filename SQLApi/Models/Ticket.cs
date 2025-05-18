using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int AlumnoId { get; set; }

    public string? Estado { get; set; }

    public decimal? Total { get; set; }

    public decimal? TotalPagado { get; set; }

    public decimal? SaldoPendiente { get; set; }

    public DateTime? FechaApertura { get; set; }

    public DateTime? FechaCierre { get; set; }

    public DateTime? FechaCierreProgramada { get; set; }

    public string Semana { get; set; } = null!;

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual Alumno Alumno { get; set; } = null!;

    public virtual ICollection<DetalleTicket> DetalleTickets { get; set; } = new List<DetalleTicket>();
}
