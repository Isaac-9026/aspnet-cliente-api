using ClienteApi.Data;
using ClienteApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/productos
        // Listar todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Listar()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .ToListAsync();

            return Ok(productos);
        }


        // GET: api/productos/1
        // Buscar un producto por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> ObtenerPorId(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = $"No existe el producto con ID: {id}"
                });
            }

            return Ok(producto);
        }


        // POST: api/productos
        // Registrar un nuevo producto
        [HttpPost]
        public async Task<ActionResult<Producto>> Registrar(
            [FromBody] Producto producto)
        {
            //Evitar que se envié un producto con ID manualmente
            producto.Id = 0;

            //Procedemos al registro
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            //Server Code 201 (Save OK) - Ejecutamos en segundo plano el buscado por ID, Pasamos el ID entrada, modelo
            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }

        //PUT: api/productos/1 -> actualizar
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Producto producto)
        {
            //Validación 1: Coincidencia de ID's
            if (id != producto.Id)
            {
                //Server Code 400
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo" });
            }

            //Validación 2: ¿Existe el ID?
            var existente = await _context.Productos.FindAsync(id); //FindAsync() buscador por KEY (primary key)

            if (existente == null)
            {
                //Server Code 404
                return NotFound(new { mensaje = $"No existe producto con id: {id}" });
            }

            //Procedemos  a realizar la actualización...
            existente.Nombre = producto.Nombre;
            existente.Descripcion = producto.Descripcion;
            existente.Precio = producto.Precio;
            existente.Garantia = producto.Garantia;
            existente.NumSerie = producto.NumSerie;

            //Ahora confirmemos los cambios utilizado el contenedor "existente"
            await _context.SaveChangesAsync(); //Está enlazado a <<FindAsync>>

            //Server Code 204
            return NoContent();
        }

        //DELETE: api/productoss/1 -> eliminar
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                //Server Code 404
                return NotFound(new { mensaje = $"No existe producto con ID: {id}" });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            //Server Code 204 (acción ejecutada correctamente)
            return NoContent();
        }
    }
}
