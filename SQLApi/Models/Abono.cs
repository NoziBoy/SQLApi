using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Abono
{
    public int AbonoId { get; set; }

    public int TicketId { get; set; }

    public int AlumnoId { get; set; }

    public DateTime? FechaAbono { get; set; }

    public decimal Monto { get; set; }

    public string? Observaciones { get; set; }

    public virtual Alumno Alumno { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
