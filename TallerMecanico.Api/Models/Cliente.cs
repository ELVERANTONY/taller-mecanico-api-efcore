using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TallerMecanico.Api.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "El documento debe tener entre 8 y 15 caracteres.")]
    [RegularExpression(@"^\d{8,15}$", ErrorMessage = "El documento solo admite números (8 a 15 dígitos).")]
    public string Documento { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 80 caracteres.")]
    public string Apellido { get; set; } = null!;

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "El teléfono contiene caracteres no válidos.")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(120, ErrorMessage = "El correo no puede superar los 120 caracteres.")]
    public string? Email { get; set; }

    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string? Direccion { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    // Evita el ciclo Cliente -> Vehiculos -> Cliente al serializar.
    [JsonIgnore]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
