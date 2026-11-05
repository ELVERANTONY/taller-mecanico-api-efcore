using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.Api.Models;

public partial class Vehiculo
{
    public int VehiculoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un cliente válido.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(10, MinimumLength = 6, ErrorMessage = "La placa debe tener entre 6 y 10 caracteres.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "La placa solo admite mayúsculas, números y guiones.")]
    public string Placa { get; set; } = null!;

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La marca debe tener entre 2 y 50 caracteres.")]
    public string Marca { get; set; } = null!;

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El modelo debe tener entre 1 y 50 caracteres.")]
    public string Modelo { get; set; } = null!;

    [Range(1950, 2100, ErrorMessage = "El año debe estar entre 1950 y 2100.")]
    public short Anio { get; set; }

    [StringLength(30, ErrorMessage = "El color no puede superar los 30 caracteres.")]
    public string? Color { get; set; }

    [Range(0, 2000000, ErrorMessage = "El kilometraje debe estar entre 0 y 2 000 000.")]
    public int Kilometraje { get; set; }

    // Navegación opcional: el POST solo necesita enviar ClienteId.
    public virtual Cliente? Cliente { get; set; }
}
