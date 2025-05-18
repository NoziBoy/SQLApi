using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Empresa
{
    public int EmpresaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public byte[]? Logo { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
