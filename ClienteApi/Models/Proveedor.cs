using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClienteApi.Models
{
    [Table("proveedores")]
    public class Proveedor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, StringLength(11)]
        [Column("ruc")]
        public string Ruc { get; set; } = string.Empty;

        [Required, StringLength(120)]
        [Column("razonsocial")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required, StringLength(150)]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [Required, StringLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Required, StringLength(150)]
        [Column("correo")]
        public string Correo { get; set; } = string.Empty;
    }
}
