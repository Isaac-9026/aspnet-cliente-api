using ClienteApi.Data;
using ClienteApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor
        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        // Listar todos los proveedores con soporte de filtro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> Listar([FromQuery] string? buscar)
        {
            var consulta = _context.Proveedores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();
                consulta = consulta.Where(p =>
                    p.Ruc.Contains(texto) ||
                    p.RazonSocial.Contains(texto) ||
                    p.Direccion.Contains(texto) ||
                    p.Correo.Contains(texto) ||
                    p.Telefono.Contains(texto)
                );
            }

            var lista = await consulta
                .OrderBy(p => p.RazonSocial)
                .Take(200)
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/proveedores/1
        // Buscar un proveedor por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Proveedor>> ObtenerPorId(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new
                {
                    mensaje = $"No existe el proveedor con ID: {id}"
                });
            }

            return Ok(proveedor);
        }

        // POST: api/proveedores
        // Registrar un nuevo proveedor
        [HttpPost]
        public async Task<ActionResult<Proveedor>> Registrar([FromBody] Proveedor proveedor)
        {
            // Evitar que se envíe un ID manualmente
            proveedor.Id = 0;

            // Validar que el RUC no esté repetido
            if (await _context.Proveedores.AnyAsync(p => p.Ruc == proveedor.Ruc))
            {
                return Conflict(new { mensaje = "Ya existe un proveedor con ese RUC" });
            }

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedores/1 -> actualizar
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Proveedor proveedor)
        {
            // Validación 1: Coincidencia de ID's
            if (id != proveedor.Id)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo" });
            }

            // Validación 2: ¿Existe el ID?
            var existente = await _context.Proveedores.FindAsync(id);
            if (existente == null)
            {
                return NotFound(new { mensaje = $"No existe proveedor con ID: {id}" });
            }

            // Validación 3: RUC repetido perteneciente a otro registro
            if (await _context.Proveedores.AnyAsync(p => p.Ruc == proveedor.Ruc && p.Id != id))
            {
                return Conflict(new { mensaje = "Ya existe un proveedor con ese RUC" });
            }

            // Actualización de campos
            existente.Ruc = proveedor.Ruc;
            existente.RazonSocial = proveedor.RazonSocial;
            existente.Direccion = proveedor.Direccion;
            existente.Telefono = proveedor.Telefono;
            existente.Correo = proveedor.Correo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/proveedores/1 -> eliminar
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound(new { mensaje = $"No existe proveedor con ID: {id}" });
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
