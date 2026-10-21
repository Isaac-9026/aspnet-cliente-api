using ClienteApi.Data;
using ClienteApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Data;
using System.Text;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; //Acceso BD
        private readonly IConfiguration _config; //Configuracion

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //Registrar
        //POST: api/auth/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
        {
            //Si el usuario ya existe, cancelamos el proceso
            if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username))
            {
                //Server code 409
                return Conflict(new { mensaje = "El usuario ya existe" });
            }

            //Si no existe, creamos uno nuevo utilizando AuthDtos
            var usuario = new Usuario
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Rol = "Usuario"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registradi correctamente"});

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync( U => U.Username == request.Username );
            
            //¿La contraseña se desencripta?
            //NO, se utiliza un algoritmo matemático para verificar su autenticidad
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            {
                //Server code 401
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrecto" });
            }

            //var => declaración dinámica objetos
            //Deserealización
            var (token, expira) = GenerarToken(usuario);

            return Ok(new { token, expira });
        }

        //El método retornará:
        //token     :cadena larga de caracteres
        //fecha hora: vigencia del token
        private (string token, DateTime expira) GenerarToken(Usuario usuario)
        {
            //Json Web token
            var jwt = _config.GetSection("Jwt");

            //Nora: Los parámetros ["Param] vienen del archivo de configuración (JSON)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var expira = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!));
            
            //Configuración del algoritmo que emite el certificado y la firma de seguridad
            //algoritmos criptográficos
            var descriptor = new SecurityTokenDescriptor
            {   
                //IdUsuario, Nombre, Rol, Permiso
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Username),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                Expires = expira,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expira);
        }
    }
}
