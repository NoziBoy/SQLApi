using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class DetalleTicket
{
    public int DetalleId { get; set; }

    public int TicketId { get; set; }

    public string Producto { get; set; } = null!;

    public int Cantidad { get; set; }

    public int Total { get; set; }

    public decimal Precio { get; set; }

    public decimal? Subtotal { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
