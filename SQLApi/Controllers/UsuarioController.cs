using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLApi.Models;
using Microsoft.EntityFrameworkCore;
namespace SQLApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly VentaContext _context;

        public UsuarioController(VentaContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }
    }
}
