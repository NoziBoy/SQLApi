namespace SQLApi.Models
{
    public class DetalleTicketAlumnoDTO
    {
        public int NumeroTicket { get; set; }
        public int AlumnoID { get; set; } 
        public decimal TotalTicket { get; set; }
        public decimal TotalAbonado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string EstadoTicket { get; set; }
        public string EstadoPago { get; set; }
        public DateTime FechaCreacion { get; set; }


        public bool IsSeleccionado { get; set; } = false;
    }
}
