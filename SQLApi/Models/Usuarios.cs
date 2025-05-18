using System;
using System.Collections.Generic;

namespace SQLApi.Models;

public partial class Usuarios
{
    public int UsuarioId { get; set; }

    public string? Usuario { get; set; }

    public string? Contraseña { get; set; }

    public string? Position { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }
}
