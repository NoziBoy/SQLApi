using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Producto
{
    public int Idproducto { get; set; }

    public string? NombreProducto { get; set; }

    public decimal? Precio { get; set; }

    public int? StockDisponible { get; set; }

    public string? CodigoDeBarras { get; set; }

    public string? Descripcion { get; set; }
}
