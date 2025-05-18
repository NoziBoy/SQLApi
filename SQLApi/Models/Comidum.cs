using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Comidum
{
    public int Idcomida { get; set; }

    public string CodigoBarras { get; set; } = null!;

    public string NombreComida { get; set; } = null!;

    public decimal Precio { get; set; }

    public string? Descripcion { get; set; }

    public byte[]? CodigoImagen { get; set; }
}
