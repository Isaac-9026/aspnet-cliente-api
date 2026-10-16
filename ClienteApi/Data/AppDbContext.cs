using ClienteApi.Models; //Modelo (definiciÃ³n tabla)
using Microsoft.EntityFrameworkCore; //Acceder BD

namespace ClienteApi.Data
{
    //Herencia => subClase : superclase
    public class AppDbContext : DbContext
    {
        //Constructor (inicializar)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //ORM tendrÃ¡ acceso al modelo cliente
        public DbSet<Cliente> Clientes { get; set; } 
    }
}
