using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Models;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController : ControllerBase
{
    private readonly TallerContext _context;

    public VehiculosController(TallerContext context) => _context = context;

    // GET api/vehiculos?clienteId=1&marca=Toyota&buscar=corolla&anio=2019
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> Get(
        int? clienteId, string? marca, string? buscar, short? anio)
    {
        var consulta = _context.Vehiculos
            .Include(v => v.Cliente)
            .AsNoTracking();

        if (clienteId.HasValue)
            consulta = consulta.Where(v => v.ClienteId == clienteId);

        if (!string.IsNullOrWhiteSpace(marca))
            consulta = consulta.Where(v => v.Marca == marca);

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            consulta = consulta.Where(v => v.Placa.Contains(texto) || v.Modelo.Contains(texto));
        }

        if (anio.HasValue)
            consulta = consulta.Where(v => v.Anio == anio);

        var vehiculos = await consulta
            .OrderBy(v => v.Placa)
            .ToListAsync();

        return Ok(vehiculos);
    }

    // GET api/vehiculos/7
    [HttpGet("{id}")]
    public async Task<ActionResult<Vehiculo>> Get(int id)
    {
        var vehiculo = await _context.Vehiculos
            .Include(v => v.Cliente)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VehiculoId == id);

        if (vehiculo is null)
            return NotFound(new { mensaje = $"No existe el vehículo con id {id}." });

        return Ok(vehiculo);
    }

    // POST api/vehiculos
    [HttpPost]
    public async Task<ActionResult<Vehiculo>> Post(Vehiculo vehiculo)
    {
        // La clave foránea debe existir: 400, no 404.
        if (!await _context.Clientes.AnyAsync(c => c.ClienteId == vehiculo.ClienteId))
        {
            ModelState.AddModelError(nameof(vehiculo.ClienteId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        // Regla de negocio: placa única.
        if (await _context.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa))
            return Conflict(new { mensaje = $"Ya existe un vehículo con la placa {vehiculo.Placa}." });

        vehiculo.Cliente = null;

        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = vehiculo.VehiculoId }, vehiculo);
    }

    // PUT api/vehiculos/7
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Vehiculo vehiculo)
    {
        if (id != vehiculo.VehiculoId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo." });

        if (!await _context.Vehiculos.AnyAsync(v => v.VehiculoId == id))
            return NotFound(new { mensaje = $"No existe el vehículo con id {id}." });

        if (!await _context.Clientes.AnyAsync(c => c.ClienteId == vehiculo.ClienteId))
        {
            ModelState.AddModelError(nameof(vehiculo.ClienteId), "El cliente indicado no existe.");
            return ValidationProblem(ModelState);
        }

        if (await _context.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa && v.VehiculoId != id))
            return Conflict(new { mensaje = $"Ya existe otro vehículo con la placa {vehiculo.Placa}." });

        vehiculo.Cliente = null;
        _context.Entry(vehiculo).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/vehiculos/7
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id);
        if (vehiculo is null)
            return NotFound(new { mensaje = $"No existe el vehículo con id {id}." });

        _context.Vehiculos.Remove(vehiculo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
