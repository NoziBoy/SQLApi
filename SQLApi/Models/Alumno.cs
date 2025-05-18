using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Alumno
{
    public int AlumnoId { get; set; }

    public string Matricula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int Grado { get; set; }

    public string Grupo { get; set; } = null!;

    public string TelefonoTutor { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
